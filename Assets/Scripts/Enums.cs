using UnityEngine;

public enum Phase { DP, SP, MP1, BP, MP2, EP }  // DO NOT CHANGE ORDER

public enum CardZone { Field, Hand, GY, Deck, Banished, Extra, MonsterZone, SpellTrapZone, Side }

public enum BaseType { Monster, Spell, Trap }

public enum Attribute { EARTH, WATER, FIRE, WIND, DARK, LIGHT, DIVINE }

public enum MonsterType { 
	Aqua, Beast, BeastWarrior, Cyberse, Dinosaur, Divine, Dragon, Fairy, Fiend, Fish, Illusion, Insect, Machine, Plant, Psychic, Pyro, Reptile, Rock, SeaSerpent, Spellcaster, Thunder, Warrior, WingedBeast, Wyrm, Zombie }

public enum MonsterEffectType { Vanilla, Effect }
public enum BattlePosition { Attack, Defense }
public enum SummonRestriction { Summonable, Seminomi, Nomi }
public enum MonsterExtraType { Fusion, Synchro, Xyz, Link, None }

public enum SpellCategory { Normal, Equip, Quickplay, Continuous, Ritual, Field }
public enum TrapCategory { Normal, Continuous, Counter }

public enum MoveReason {
	Activated,
	NormalSummoned,
	SetMonster,
	SetBackrow,
	SpecialSummoned,
	None,
	Destroyed,
	SentByEffect,
	SentByCost,
	Tributed,
	UsedAsMaterial,
	Discarded,
	Returned,
	Resolved,
	Banished
}

public enum CardActionType {
	Use,
	NormalSummon,
	Attack,
	ChangePosition,
	Activate,
	SetMonster,
	SetBackrow,
	Flip,
	SpecialSummon
}
