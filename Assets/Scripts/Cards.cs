// Card,
// MonsterCard : Card,
// abstract SpellTrapCard : Card,
// SpellCard : SpellTrapCard,
// TrapCard : SpellTrapCard,
// CardInstance ⊂ Card

using UnityEngine;
using System;
using System.Collections.Generic;

public class Card {
	public string Name { get; set; }
	public BaseType baseType { get; set; }
	public Sprite Artwork { get; }
	public string Text { get; }
	public int ID { get; }
	public CardZone startingZone { get; }

	public Card(string name, BaseType baseType, Sprite artwork, string text, int id, CardZone startingZone) {
		Name = name;
		this.baseType = baseType;
		Artwork = artwork;
		Text = text;
		ID = id;
		this.startingZone = startingZone;
	}
}

public class MonsterCard : Card  {
	public int Attack { get; set; }
	public int Defense { get; set; }
	public int Level { get; set; }
	public Attribute attribute { get; set; }
	public MonsterType Type { get; set; }
	public MonsterExtraType ExtraType { get; set; }
	public bool Flip { get; set; }
	public bool Gemini { get; set; }
	public bool Spirit { get; set; }
	public bool Toon { get; set; }
	public bool Tuner { get; set; }
	public bool Union { get; set; }
	public bool Ritual { get; set; }
	public bool Pendulum { get; set; }
	public MonsterEffectType EffectType { get; set; }
	public SummonRestriction Restriction { get; set; }

	public MonsterCard(string name, BaseType baseType, Sprite artwork, string text, int id, int atk, int def, int lvl, Attribute attribute, MonsterType type, MonsterExtraType extraType, bool flip, bool gemini, bool spirit, bool toon, bool tuner, bool union, MonsterEffectType effectType, bool ritual, bool pendulum, SummonRestriction restriction, CardZone startingZone)
	:base(name, baseType, artwork, text, id, startingZone) {
		Attack = atk;
		Defense = def;
		Level = lvl;
		this.attribute = attribute;
		Type = type;
		ExtraType = extraType;
		Flip = flip;
		Gemini = gemini;
		Spirit = spirit;
		Toon = toon;
		Tuner = tuner;
		Union = union;
		EffectType = effectType;
		Ritual = ritual;
		Pendulum = pendulum;
		Restriction = restriction;
	}
}

public abstract class SpellTrapCard : Card {
	protected SpellTrapCard(string name, BaseType baseType, Sprite artwork, string text, int id, CardZone startingZone)
	: base(name, baseType, artwork, text, id, startingZone) {
	}
}

public class SpellCard : SpellTrapCard {
	public SpellCategory Category { get; set; }
	public SpellCard(string name, BaseType baseType, Sprite artwork, string text, int id, SpellCategory category, CardZone startingZone)
	: base(name, baseType, artwork, text, id, startingZone) {
		Category = category;
	}
}

public class TrapCard : SpellTrapCard {
	public TrapCategory Category { get; set; }
	public TrapCard(string name, BaseType baseType, Sprite artwork, string text, int id, TrapCategory category, CardZone startingZone)
	: base(name, baseType, artwork, text, id, startingZone) {
		Category = category;
	}
}

public class CardInstance {
    public Card CardData { get; }
    public CardZone Zone { get; set; }
    public bool IsFaceUp { get; set; } = true;
    public MoveReason LastMoveReason { get; set; }

    public Dictionary<Type, CardComponent> Components = new();

    public CardInstance(Card card, CardZone startingZone) {
        CardData = card;
        Zone = startingZone;
    }

    public void AddComponent(CardComponent component) {
        component.Owner = this;
        Components[component.GetType()] = component;
    }

    public T GetComponent<T>() where T : CardComponent {
        foreach (var comp in Components.Values) {
            if (comp is T tComp)
                return tComp;
        }
        return null;
    }

    public bool HasComponent<T>() where T : CardComponent {
        foreach (var comp in Components.Values) {
            if (comp is T)
                return true;
        }
        return false;
    }

    public bool TryGetComponent<T>(out T component) where T : CardComponent {
        foreach (var comp in Components.Values) {
            if (comp is T tComp) {
                component = tComp;
                return true;
            }
        }
        component = null;
        return false;
    }

    public T GetOrAddComponent<T>() where T : CardComponent, new() {
        if (!TryGetComponent<T>(out var comp)) {
            comp = new T();
            AddComponent(comp);
        }
        return comp;
    }

    public void RemoveComponent<T>() where T : CardComponent {
        Type targetType = typeof(T);

        foreach (var key in new List<Type>(Components.Keys)) {
            if (targetType.IsAssignableFrom(key)) {
                Components.Remove(key);
            }
        }
    }
}