using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReinforcementoftheArmy : SpellEffectComponent, ICardEffect {
	public const string Name = "Reinforcement of the Army";
	public string Message = "Select lvl 4 or lower Warrior to add from deck.";
	protected CardInstance instance;

	public override void Register(DuelController duelController, CardInstance instance) {
		this.duelController = duelController;
		this.instance = instance;
		duelController.OnSpellActivated -= HandleSpellActivation;
		duelController.OnSpellActivated += HandleSpellActivation;
	}

	public Task HandleSpellActivation(Player player, CardInstance card) {
		if (card != this.instance) return Task.CompletedTask;
		duelController.RequestChainAddition(player, instance);
		return Task.CompletedTask;
	}

	public override async Task Resolve(ChainLink link) {
		Debug.Log("RESOLVE STARTED");
		List<CardInstance> legalTargets = link.Owner.Deck
			.Where(x => x.CardData is MonsterCard m
				&& m.Type == MonsterType.Warrior
				&& m.Level <= 4)
			.ToList();
		Debug.Log($"LEGAL TARGET COUNT: {legalTargets.Count}");
		CardInstance target = await duelController.RequestCardSelection(link.Owner, Message, legalTargets);
		Debug.Log(target == null ? "TARGET NULL" : $"TARGET: {target.CardData.Name}");
		duelController.RequestSearch(link.Owner, target);
	}
}
