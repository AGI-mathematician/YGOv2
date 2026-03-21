using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Initializer : MonoBehaviour {

	[SerializeField] private HandView HandContainer;
	[SerializeField] private FieldView[] playerMonsterZones;
	[SerializeField] private UIController uiController;
	[SerializeField] private Button buttonPrefab;

	TurnManager turnManager;
	RulesEngine rulesEngine;
	ChainManager chainManager;
	ActionUIHandler actionUIHandler;
	DuelController duelController;

	Player Human;
	Player Ai;
	Board board;

	public void Awake() {
		chainManager = new ChainManager();
		turnManager = new TurnManager();
		rulesEngine = new RulesEngine(turnManager, chainManager);
		actionUIHandler = new ActionUIHandler(buttonPrefab);
		duelController = new DuelController(turnManager, rulesEngine, chainManager, actionUIHandler, uiController);
		LoadPlayers();
		for (int i = 0; i < playerMonsterZones.Length; i++)
			playerMonsterZones[i].Initialize(duelController, Human, i);
		HandContainer.Initialize(duelController, Human);
		uiController.Initialize(duelController);
		LoadDeck();
		Utility.Shuffle<CardInstance>(Human.Deck);
		rulesEngine.DrawStartingHand(Human);
	}

	public void LoadPlayers() {
		Field playerField = new();
		Field aiField = new();
		Human = new("Human", playerField);
		Ai = new("Ai", aiField);
		Human.opponent = Ai;
		Ai.opponent = Human;
		board = new(Human, Ai);
	}

	public void LoadDeck() {
		TextAsset jsonFile = Resources.Load<TextAsset>("sixSam");
		CardListDTO dtoContainer = JsonUtility.FromJson<CardListDTO>(jsonFile.text);
		foreach (CardDTO card in dtoContainer.cards) {
			switch (card.baseType) {
				case "Monster":
					MonsterCard monster = new(card.name, Enum.Parse<BaseType>(card.baseType), Resources.Load<Sprite>(card.artwork), card.text, card.id, card.attack, card.defense, card.level, Enum.Parse<Attribute>(card.attribute), Enum.Parse<MonsterType>(card.type), Enum.Parse<MonsterExtraType>(card.extraType), card.flip, card.gemini, card.spirit, card.toon, card.tuner, card.union, Enum.Parse<MonsterEffectType>(card.effectType), card.ritual, card.pendulum, Enum.Parse<SummonRestriction>(card.restriction), Enum.Parse<CardZone>(card.startingZone));
					CardZone startingZone;
					if (card.extraType == "None") startingZone = CardZone.Deck;
					else startingZone = CardZone.Extra;
					CardInstance instance = new(monster, startingZone);
					Utility.AttachEffects(instance, duelController);
					Human.Deck.Add(instance);
					break;
				case "Spell":
					SpellCard spell = new(card.name, Enum.Parse<BaseType>(card.baseType), Resources.Load<Sprite>(card.artwork), card.text, card.id, Enum.Parse<SpellCategory>(card.category), Enum.Parse<CardZone>(card.startingZone));
					CardInstance spellInstance = new (spell, CardZone.Deck);
					Human.Deck.Add(spellInstance);
					break;
				case "Trap":
					TrapCard trap = new(card.name, Enum.Parse<BaseType>(card.baseType), Resources.Load<Sprite>(card.artwork), card.text, card.id, Enum.Parse<TrapCategory>(card.category), Enum.Parse<CardZone>(card.startingZone));
					CardInstance trapInstance = new (trap, CardZone.Deck);
					Human.Deck.Add(trapInstance);
					break;
			}
		}
	}
}
