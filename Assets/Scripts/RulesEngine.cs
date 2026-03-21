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
		if (TrySetBackrow(player, card)) LegalActions.Add(CardActionType.Set);
		if (TrySetMonster(player, card)) LegalActions.Add(CardActionType.Set);
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
		OnFieldChanged?.Invoke(player);
		OnCardMoved?.Invoke(player, card, fromZone, CardZone.Field);
		return i;		
	}

	public bool TryActivateBackrow(Player player, CardInstance card) {
		if (card.CardData is not SpellTrapCard) return false;
		if (card.CardData is SpellCard)
			if (Utility.BackrowCount(player) < 5) return true;
		if (card.CardData is TrapCard && player.Hand.Contains(card))
			return false;
		return false;
	}

	public bool TrySetBackrow(Player player, CardInstance card) {
		if (card.CardData is SpellTrapCard && player.Hand.Contains(card))
			return true;
		return false;
	}

	public bool TrySetMonster(Player player, CardInstance card) {
		if (card.CardData is MonsterCard && player.Hand.Contains(card))
			return true;
		return false;
	}
}
