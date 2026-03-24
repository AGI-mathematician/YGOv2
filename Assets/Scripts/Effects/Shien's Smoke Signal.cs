using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ShiensSmokeSignal : SpellEffectComponent, ICardEffect {
	public const string Name = "Shiens Smoke Signal";
	public string Message = "Select lvl 3 or lower Six Samurai to add from deck.";
	protected CardInstance instance;

	public override void Register(DuelController duelController, CardInstance instance) {
		this.duelController = duelController;
		this.instance = instance;
		duelController.OnSpellActivated += HandleSpellActivation;
	}

	public void HandleSpellActivation(Player player, CardInstance card) {
		if (card != this.instance) return;
		duelController.RequestChainAddition(player, instance);
	}

	public override async Task Resolve(ChainLink link) {
		Debug.Log("RESOLVE STARTED");
		List<CardInstance> legalTargets = link.Owner.Deck
			.Where(x => x.CardData is MonsterCard m
				&& m.Name.Contains("Six Samurai")
				&& m.Level <= 3)
			.ToList();
		Debug.Log($"LEGAL TARGET COUNT: {legalTargets.Count}");
		CardInstance target = await duelController.RequestCardSelection(link.Owner, Message, legalTargets);
		Debug.Log(target == null ? "TARGET NULL" : $"TARGET: {target.CardData.Name}");
		duelController.RequestSearch(link.Owner, target);
	}
}
