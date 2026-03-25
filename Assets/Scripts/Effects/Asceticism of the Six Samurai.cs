using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AsceticismoftheSixSamurai : SpellEffectComponent, ICardEffect {
	public const string Name = "Asceticism of the Six Samurai";
	public string Message1 = "Target 1 Six Samurai on your field.";
	public string Message2 = "Special Summon 1 valid target.";
	protected CardInstance instance;

	public override void Register(DuelController duelController, CardInstance instance) {
		this.duelController = duelController;
		this.instance = instance;
		duelController.OnSpellActivated -= HandleSpellActivation;
		duelController.OnSpellActivated += HandleSpellActivation;
	}

	public async Task HandleSpellActivation(Player player, CardInstance card) {
		if (card != this.instance) return;

		// TARGET: Six Samurai on field
		List<CardInstance> legalTargets = player.field.MonsterZones
			.Where(ci =>
				ci != null &&
				ci.CardData is MonsterCard &&
				((MonsterCard)ci.CardData).Name.Contains("Six Samurai"))
			.ToList();

		if (legalTargets.Count == 0) return;

		CardInstance target = await duelController.RequestCardSelection(player, Message1, legalTargets);

		if (target == null) return;

		ChainLink link = new ChainLink(player, instance, this);
		link.Targets.Add(new Target(target));
		duelController.RequestActivateEffect(link);
	}

	public override async Task Resolve(ChainLink link) {
		Debug.Log("Resolve start. Targets: " + link.Targets.Count);
		if (link.Targets.Count > 0)
			Debug.Log("Target null? " + (link.Targets[0].Card == null));
		if (link.Targets.Count == 0 || link.Targets[0].Card == null) return;

		CardInstance target = link.Targets[0].Card;

		// CAST
		MonsterCard monster = target.CardData as MonsterCard;
		if (monster == null) return;

		int atk = monster.Attack;
		string name = monster.Name;

		// SEARCH DECK
		List<CardInstance> validTargets = link.Owner.Deck
			.Where(ci =>
				ci != null &&
				ci.CardData is MonsterCard &&
				((MonsterCard)ci.CardData).Name.Contains("Six Samurai") &&
				((MonsterCard)ci.CardData).Attack == atk &&
				((MonsterCard)ci.CardData).Name != name)
			.ToList();
		Debug.Log("Valid deck targets: " + validTargets.Count);
		if (validTargets.Count == 0) return;

		CardInstance chosen = await duelController.RequestCardSelection(
			link.Owner,
			Message2,
			validTargets
		);

		if (chosen == null) return;

		BattlePosition decision = await duelController.RequestBattlePosition(link.Owner);

		duelController.RequestSpecialSummon(
			link.Owner,
			chosen,
			link.Owner.Deck,
			decision
		);
	}
}