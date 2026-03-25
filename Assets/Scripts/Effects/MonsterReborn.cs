using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MonsterReborn : SpellEffectComponent, ICardEffect {
	public const string Name = "Monster Reborn";
	public string Message1 = "Target Monster to Special Summon.";
	public string Message2 = "Choose the battle position.";
	protected CardInstance instance;
	private Player owner;

	public override void Register(DuelController duelController, CardInstance instance) {
		this.duelController = duelController;
		this.instance = instance;
		duelController.OnSpellActivated -= HandleSpellActivation;
		duelController.OnSpellActivated += HandleSpellActivation;
	}

	public async Task HandleSpellActivation(Player player, CardInstance card) {
		if (card != this.instance) return;
		List<CardInstance> legalTargets = player.GY
			.Where(ci => ci.CardData is MonsterCard)
			.ToList();
		CardInstance target = await duelController.RequestCardSelection(player, Message1, legalTargets);
		ChainLink link = new ChainLink(player, instance, this);
		link.Targets.Add(new Target(target));
		duelController.RequestActivateEffect(link);		
	}

	public override async Task Resolve(ChainLink link) {
		BattlePosition decision = await duelController.RequestBattlePosition(link.Owner);
		duelController.RequestSpecialSummon(link.Owner, link.Targets[0].Card, owner.GY, decision); 
	}
}
