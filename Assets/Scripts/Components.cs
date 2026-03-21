using UnityEngine;

public abstract class CardComponent {
	public CardInstance Owner { get; internal set; }
	public virtual void Register(DuelController duel) { }  
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

public class CardEffectComponent : CardComponent {
	protected DuelController duelController;
}

public abstract class MonsterEffectComponent : CardEffectComponent {
	public abstract void Register(DuelController duelController, CardInstance instance);
}

public abstract class SpellEffectComponent : CardEffectComponent {
	public abstract void Register(DuelController duelController, CardInstance instance);
}

public class TrapEffectComponent : CardEffectComponent {
}

