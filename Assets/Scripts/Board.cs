using UnityEngine;
using System.Collections.Generic;

public class Board {
	public Player Player1 { get; set; }
	public Player Player2 { get; set; }

	public Board(Player p1, Player p2) {
		Player1 = p1;
		Player2 = p2;
	}
}

public class Player {
	public string name { get; }
	public Player opponent { get; set; }
	public int LP { get; set; } = 8000;
	public Field field { get; } = new Field();
	public List<CardInstance> Hand = new List<CardInstance>();
	public List<CardInstance> GY = new List<CardInstance>();
	public List<CardInstance> Banished = new List<CardInstance>();
	public List<CardInstance> Deck = new List<CardInstance>();
	public List<CardInstance> Extra = new List<CardInstance>();
	public List<CardInstance> Side = new List<CardInstance>();

	public Player(string name, Field field) {
		this.name = name;
		this.field = field;
	}
}

public class Field {
	public CardInstance[] MonsterZones = new CardInstance[5];
	public CardInstance[] SpellTrapZones = new CardInstance[5];
	public CardInstance FieldSpell { get; set; }
}




	
