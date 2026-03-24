using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Kageki : MonsterEffectComponent, ICardEffect {

	public const string Name =  "Legendary Six Samurai - Kageki";
	public string Message1 = "Activate Kageki's effect to Special Summon?";
	public string Message2 = "Choose which Six Samurai to Special Summon.";
	protected CardInstance instance;
	private Player owner;

	public override void Register(DuelController duelController, CardInstance instance) {
		this.duelController = duelController;
		this.instance = instance;
		duelController.OnMonsterNormalSummoned += HandleNormalSummoned;
	}

	public async void HandleNormalSummoned(Player player, CardInstance card) {
		if (card != this.instance) return;
		bool activate = await duelController.RequestYesNo(player, Message1);
		if (!activate) return;
		owner = player;
		duelController.RequestActivateEffect(new ChainLink(player, instance, this));	 
	}

	public override async Task Resolve(ChainLink link) {
		List<CardInstance> options =
			owner.Hand
			.Where(ci => ci.CardData is MonsterCard m &&
				m.Level <= 4 &&
				ci.CardData.Name.Contains("Six Samurai"))
			.ToList();
		CardInstance selection = await duelController.RequestCardSelection(owner, Message2, options);
		BattlePosition battleChoice = await duelController.RequestBattlePosition(owner);
		duelController.RequestSpecialSummon(owner, selection, owner.Hand, battleChoice); 
	}
}
