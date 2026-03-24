using UnityEngine;
using System.Threading.Tasks;

public abstract class CardComponent {
	public CardInstance Owner { get; internal set; }
	public virtual void Register(DuelController duel, CardInstance instance) {
		Owner = instance;
	}  
}

public class MonsterFieldComponent : CardComponent {
	public bool WasSummonedThisTurn { get; set; } = true;
	public BattlePosition Position { get; set; }
	public int AttacksThisTurn { get; set; }
	public int BattlePositionChangesThisTurn { get; set; }
}

public class BackrowFieldComponent : CardComponent {
	public bool WasSetThisTurn { get; set; }
}

public abstract class CardEffectComponent : CardComponent, ICardEffect {
	protected DuelController duelController;
	public abstract Task Resolve(ChainLink link);
}

public abstract class MonsterEffectComponent : CardEffectComponent {
	public abstract override void Register(DuelController duelController, CardInstance instance);
}

public abstract class SpellEffectComponent : CardEffectComponent {
	public abstract override void Register(DuelController duelController, CardInstance instance);
}

public abstract class TrapEffectComponent : CardEffectComponent {
	public abstract override void Register(DuelController duelController, CardInstance instance);
}

