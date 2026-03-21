using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardListDTO {
	public List<CardDTO> cards;		// Must match the JSON file
}


[System.Serializable]
public class CardDTO {
	public int id;
	public string name;
	public string artwork;
	public string baseType;			// Monster / Spell / Trap
	public string text;

						// Monster-only
	public int attack;
	public int defense;
	public int level;
	public string attribute;
	public string type;
	public bool tuner;
	public string extraType;

						// Spell/Trap-only
	public string category;

						// shared flags
	public string startingZone;
	public bool flip;
	public bool gemini;
	public bool spirit;
	public bool toon;
	public bool union;
	public string effectType;
	public bool ritual;
	public bool pendulum;
	public string restriction;
}