using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DuelController {
	private Player _currentPlayer;
	private CardInstance _currentCard;
	private readonly TurnManager _turnManager;
	private readonly RulesEngine _rulesEngine;
	private readonly ChainManager _chainManager;
	private readonly ActionUIHandler _actionUIHandler;
	private readonly UIController _uiController;

	public event Action<Player> OnCardDrawn;
	public event Action<Player> OnCardChanged;
	public event Action<Player> OnFieldChanged;
	public event Action<Player, CardInstance> OnMonsterNormalSummoned;
	public event Action<Player, CardInstance, CardZone, CardZone> OnCardMoved;
	public event Func<Player, CardInstance, Task> OnSpellActivated;

	public Phase CurrentPhase => _turnManager.CurrentPhase;

	public DuelController(TurnManager turnManager, RulesEngine rulesEngine, ChainManager chainManager, ActionUIHandler actionUIHandler, UIController uiController) {
		_uiController = uiController;
		_turnManager = turnManager;
		_rulesEngine = rulesEngine;
		_chainManager = chainManager;
		_actionUIHandler = actionUIHandler;
		_actionUIHandler.OnActionSelected += HandleActionSelected;
		_rulesEngine.OnFieldChanged += HandleFieldChanged;
		_rulesEngine.OnCardMoved += HandleCardMoved;
		_rulesEngine.OnCardDrawn += HandleCardDrawn;
	}

	public void HandleCardDrawn(Player player) {
		OnCardDrawn?.Invoke(player);
	}

	public void HandleNormalSummon(Player player, CardInstance card) {
		OnMonsterNormalSummoned?.Invoke(player, card);
	}

	public void HandleCardMoved(Player player, CardInstance card, CardZone from, CardZone to) {
		Debug.Log($"[HAND VIEW] {from} -> {to}");
		OnCardMoved?.Invoke(player, card, from, to);
		foreach (Transform child in _uiController.ActionPanel)
			UnityEngine.Object.Destroy(child.gameObject);
	}

	public void HandleFieldChanged(Player player) {
		OnFieldChanged?.Invoke(player);
	}

	public async Task HandleSpellActivated(Player player, CardInstance card) {
		if (OnSpellActivated == null) return;
		foreach (var handler in OnSpellActivated.GetInvocationList()) {
			await ((Func<Player, CardInstance, Task>)handler)(player, card);
		}
	}

	public async void HandleActionSelected(CardActionType action) {
		switch (action) {
			case CardActionType.NormalSummon:
				_rulesEngine.NormalSummon(_currentPlayer, _currentCard);
				HandleNormalSummon(_currentPlayer, _currentCard);
				break;
			case CardActionType.SetMonster:
				_rulesEngine.DoSetMonster(_currentPlayer, _currentCard);
				HandleNormalSummon(_currentPlayer, _currentCard);
				break;
			case CardActionType.SetBackrow:
				_rulesEngine.DoSetBackrow(_currentPlayer, _currentCard);
				break;
			case CardActionType.Activate:
				_rulesEngine.DoActivateBackrow(_currentPlayer, _currentCard);
				await HandleSpellActivated(_currentPlayer, _currentCard);
				OnCardChanged?.Invoke(_currentPlayer);
				OnFieldChanged?.Invoke(_currentPlayer);
				break;
		}
	}

	public int RequestSpecialSummon(Player player, CardInstance card, List<CardInstance> previousZone, BattlePosition position) {
		if (!_rulesEngine.TrySpecialSummon(player, card, previousZone)) {
			Debug.Log("Special Summon failed rule check.");
			return -1;
		}
		int index = _rulesEngine.SpecialSummon(player, card, previousZone, position);
		OnCardChanged?.Invoke(player);   // refresh hand
		OnFieldChanged?.Invoke(player);  // refresh field
		return index;
	}

	public Task<bool> RequestYesNo(Player player, string message) {
		var tcs = new TaskCompletionSource<bool>();
		_actionUIHandler.BuildYesNo(message, _uiController.ActionPanel);
		void Handler(bool result) {
			_actionUIHandler.OnYesNoSelected -= Handler;
			tcs.SetResult(result);
		}
		_actionUIHandler.OnYesNoSelected += Handler;
		return tcs.Task;
	}

	public Task<CardInstance> RequestCardSelection(Player player, string message, List<CardInstance> options) {
		Debug.Log("REQUEST CARD SELECTION CALLED");
		var tcs = new TaskCompletionSource<CardInstance>();
		_actionUIHandler.BuildCardSelection(options, _uiController.ActionPanel);
		Debug.Log("BUILDING CARD SELECTION UI");
		void Handler(CardInstance card) {
			_actionUIHandler.OnCardSelected -= Handler;
			tcs.SetResult(card);
    		}
		_actionUIHandler.OnCardSelected += Handler;
		return tcs.Task;
	}

	public Task<BattlePosition> RequestBattlePosition(Player player) {
		var tcs = new TaskCompletionSource<BattlePosition>();
		_actionUIHandler.BuildAttackDefenseChoice(_uiController.ActionPanel);
		void Handler(BattlePosition position) {
			_actionUIHandler.OnBattlePositionSelected -= Handler;
			tcs.SetResult(position);
		}
		_actionUIHandler.OnBattlePositionSelected += Handler;
		return tcs.Task;
	}

	public void RefreshAvailableActions(Player player, CardInstance card, Transform parentTransform) {
		_currentPlayer = player;
		_currentCard = card;
		Debug.Log($"[SELECT] InstanceID: {_currentCard.GetHashCode()} | Card: {_currentCard.CardData.Name}");
		List<CardActionType> legalActions = _rulesEngine.GetLegalActions(player, card);
		_actionUIHandler.BuildActionButtons(legalActions, parentTransform);
	}

	public void RequestPhaseChange(Phase targetPhase) {
		if ((int)targetPhase == (int)_turnManager.CurrentPhase + 1 )
			_turnManager.AdvancePhase();
		else if (_turnManager.CurrentPhase == Phase.MP1 && targetPhase == Phase.EP)
			_turnManager.AdvanceToEP();
	}

	public async void RequestActivateEffect(ChainLink link) {
		_chainManager.AddChain(link);
		await _chainManager.ResolveChain();
		_rulesEngine.CleanupBackrow(link.Owner);
	}

	public void RequestSearch(Player player, CardInstance card) {
		_rulesEngine.Search(player, card);
	}

	public void RequestChainAddition(Player player, CardInstance card) {
		Debug.Log("CHAIN ADD CALLED");
		var effect = card.GetComponent<CardEffectComponent>();
		if (effect == null) return;
		_rulesEngine.InitiateChainAddition(player, card, effect);
	}

}
