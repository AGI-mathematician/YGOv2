using UnityEngine;
using System;
using System.Collections.Generic;

public class RulesEngine {
	
	private readonly TurnManager _turnManager;
	private readonly ChainManager _chainManager;
	public event Action<Player> OnCardDrawn;
	public event Action<Player> OnFieldChanged;
	public event Action<Player> OnCardChanged;
	public event Action<Player, CardInstance> OnMonsterNormalSummoned;
	public event Action<Player, CardInstance, CardZone, CardZone> OnCardMoved;

	public RulesEngine(TurnManager turnManager, ChainManager chainManager) {
		_turnManager = turnManager;
		_chainManager = chainManager;
	}

	public List<CardActionType> GetLegalActions(Player player, CardInstance card) {
		var LegalActions = new List<CardActionType>();
		if (TryNormalSummon(player, card)) LegalActions.Add(CardActionType.NormalSummon);
		if (TryActivateBackrow(player, card)) LegalActions.Add(CardActionType.Activate);
		if (TrySetBackrow(player, card)) LegalActions.Add(CardActionType.SetBackrow);
		if (TrySetMonster(player, card)) LegalActions.Add(CardActionType.SetMonster);
		return LegalActions;
	}

	public void DrawStartingHand(Player player) {
		for (int i = 0; i < 5; i++) {
			DrawCard(player);
		}
	}

	public void DrawCard(Player player) {
		if (player.Deck.Count == 0) {
//			AssignLoss(player);
			Debug.Log($"{player.name} has ran out of cards in their deck... Game over!!");
			return;
		}
		CardInstance topCard = player.Deck[0];
		player.Deck.RemoveAt(0);
		player.Hand.Add(topCard);
		topCard.Zone = CardZone.Hand;
		OnCardDrawn?.Invoke(player);
	}

	public bool TryNormalSummon(Player player, CardInstance card) {
		Phase phase = _turnManager.CurrentPhase;
		if (phase != Phase.MP1 && phase != Phase.MP2) return false;
		if (card.CardData is not MonsterCard monster) return false;
		if (_turnManager.HasNormalSummoned) return false;
		if (monster.Level <= 4 && Utility.MonsterCount(player) < 5) return true;
		if (monster.Level < 7 && Utility.MonsterCount(player) >= 1) return true;
		if (monster.Level >= 7 && Utility.MonsterCount(player) >= 2) return true;
		return false;
	}

	public int NormalSummon(Player player, CardInstance card) {
		player.Hand.Remove(card);
		int i;
		for (i = 0; i < player.field.MonsterZones.Length; i++) {
			if (player.field.MonsterZones[i] == null) {
				player.field.MonsterZones[i] = card;
				break;
			}
		}
		card.Zone = CardZone.Field;
		card.LastMoveReason = MoveReason.NormalSummoned;
		_turnManager.RegisterNormalSummon();
		OnCardChanged?.Invoke(player);
		OnFieldChanged?.Invoke(player);
		OnMonsterNormalSummoned?.Invoke(player, card);
		OnCardMoved?.Invoke(player, card, CardZone.Hand, CardZone.Field);
		return i;
	}

	public bool TrySpecialSummon(Player player, CardInstance card, List<CardInstance> startingZone) {
		if (Utility.MonsterCount(player) < 5) return true;
		else return false;
	}

	public int SpecialSummon(Player player, CardInstance card, List<CardInstance> previousZone, BattlePosition position) {
		CardZone fromZone = card.Zone;
		previousZone.Remove(card);
		int i;
		for (i = 0; i < player.field.MonsterZones.Length; i++) {
			if (player.field.MonsterZones[i] == null) {
				player.field.MonsterZones[i] = card;
				break;
			}
		}
		card.Zone = CardZone.Field;
		card.LastMoveReason = MoveReason.SpecialSummoned;
		MonsterFieldComponent mfc = new();
		card.AddComponent(mfc);
		mfc.Position = position;
		OnCardChanged?.Invoke(player);
		OnFieldChanged?.Invoke(player);
		OnCardMoved?.Invoke(player, card, fromZone, CardZone.Field);
		OnCardDrawn?.Invoke(player);
		return i;		
	}

	public bool TryActivateBackrow(Player player, CardInstance card) {
		Phase phase = _turnManager.CurrentPhase;
		if (card.CardData is not SpellTrapCard) return false;
		// Traps can't be activated from hand
		if (card.CardData is TrapCard && player.Hand.Contains(card))
			return false;
		// Quick-Play Spell
		if (card.CardData is SpellCard spell && spell.Category == SpellCategory.Quickplay)
			return Utility.BackrowCount(player) < 5;
		// Normal Spell (only MP1 or MP2)
		if (card.CardData is SpellCard)
			return (phase == Phase.MP1 || phase == Phase.MP2) 
				&& Utility.BackrowCount(player) < 5;
		return false;
	}

	public bool TrySetBackrow(Player player, CardInstance card) {
		Phase phase = _turnManager.CurrentPhase;
		if (phase != Phase.MP1 && phase != Phase.MP2) return false;
		if (card.CardData is SpellTrapCard && player.Hand.Contains(card))
			return true;
		return false;
	}

	public bool TrySetMonster(Player player, CardInstance card) {
		Phase phase = _turnManager.CurrentPhase;
		if (phase != Phase.MP1 && phase != Phase.MP2) return false;
		if (card.CardData is not MonsterCard monster) return false;
		if (_turnManager.HasNormalSummoned) return false;
		if (monster.Level <= 4 && Utility.MonsterCount(player) < 5) return true;
		if (monster.Level < 7 && Utility.MonsterCount(player) >= 1) return true;
		if (monster.Level >= 7 && Utility.MonsterCount(player) >= 2) return true;
		return false;
	}

	public void DoSetBackrow(Player player, CardInstance card) {
		player.Hand.Remove(card);
		int i;
		for (i = 0; i < player.field.SpellTrapZones.Length; i++) {
			if (player.field.SpellTrapZones[i] == null) {
				player.field.SpellTrapZones[i] = card;
				break;
			}
		}
		card.IsFaceUp = false;
		card.Zone = CardZone.Field;
		card.LastMoveReason = MoveReason.SetBackrow;
		OnCardChanged?.Invoke(player);
		OnFieldChanged?.Invoke(player);
		OnCardMoved?.Invoke(player, card, CardZone.Hand, CardZone.Field);			
	}

	public void DoSetMonster(Player player, CardInstance card) {
		Debug.Log($"[SET] InstanceID: {card.GetHashCode()} | InHandBeforeRemove: {player.Hand.Contains(card)}");
		bool removed = player.Hand.Remove(card);
		Debug.Log($"[SET] RemovedFromHand: {removed}");
		int i;
		for (i = 0; i < player.field.MonsterZones.Length; i++) {
			if (player.field.MonsterZones[i] == null) {
				player.field.MonsterZones[i] = card;
				break;
			}
		}
		card.GetOrAddComponent<MonsterFieldComponent>().Position = BattlePosition.Defense;
		card.IsFaceUp = false;
		card.Zone = CardZone.Field;
		card.LastMoveReason = MoveReason.SetMonster;
		_turnManager.RegisterNormalSummon();
		OnCardChanged?.Invoke(player);
		OnFieldChanged?.Invoke(player);
		OnMonsterNormalSummoned?.Invoke(player, card);
		OnCardMoved?.Invoke(player, card, CardZone.Hand, CardZone.Field);
	}
	
	public void Search(Player player, CardInstance card) {
		player.Deck.Remove(card);
		player.Hand.Add(card);
		OnCardDrawn?.Invoke(player);
	}

	public void DoActivateBackrow(Player player, CardInstance card) {
		Debug.Log($"[ACTIVATE] ComponentCount: {card.Components.Count}");
		Debug.Log($"[ACTIVATE] InstanceID: {card.GetHashCode()} | InHandBeforeRemove: {player.Hand.Contains(card)}");
		bool removed = player.Hand.Remove(card);
		Debug.Log($"[ACTIVATE] RemovedFromHand: {removed}");
		Debug.Log($"[ACTIVATE] HasEffect: {card.HasComponent<CardEffectComponent>()}");
		int i;
		for (i = 0; i < player.field.SpellTrapZones.Length; i++) {
			if (player.field.SpellTrapZones[i] == null) {
				player.field.SpellTrapZones[i] = card;
				break;
			}
		}
		CardZone fromZone = card.Zone;
		card.Zone = CardZone.Field;
		card.LastMoveReason = MoveReason.Activated;
		OnCardChanged?.Invoke(player);
		OnFieldChanged?.Invoke(player);
		OnCardMoved?.Invoke(player, card, fromZone, CardZone.Field);
		
		if (!card.TryGetComponent<CardEffectComponent>(out var effect)) {
			Debug.Log("NO EFFECT COMPONENT FOUND");
			return;
		}
		InitiateChainAddition(player, card, effect);
	}

	public void InitiateChainAddition(Player player, CardInstance card, ICardEffect effect) {
		ChainLink link = new ChainLink(player, card, effect);
		_chainManager.AddChain(link);
		ResolveChain(_chainManager, player);
	}

	public async void ResolveChain(ChainManager manager, Player player) {
		await manager.ResolveChain();
		CleanupBackrow(player);
	}

	public void CleanupBackrow(Player player) {
		for (int i = 0; i < player.field.SpellTrapZones.Length; i++) {
			var card = player.field.SpellTrapZones[i];
			if (card == null) continue;
			if (!card.IsFaceUp) continue;
			if (card.CardData is not SpellCard) continue;
			player.field.SpellTrapZones[i] = null;
			player.GY.Add(card);
			card.Zone = CardZone.GY;
			card.LastMoveReason = MoveReason.Resolved;
			OnCardMoved?.Invoke(player, card, CardZone.Field, CardZone.GY);
		}
		OnFieldChanged?.Invoke(player);
	}
}
