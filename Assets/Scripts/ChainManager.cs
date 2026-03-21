using UnityEngine;
using System.Collections.Generic;

public class ChainManager {
	private Stack<ChainLink> _chain = new();

	public bool IsChainActive() {
		return _chain.Count > 0;
	}
	public void AddChain(ChainLink link) {
		_chain.Push(link);
	}
	public void ResolveChain() {
		while (_chain.Count > 0) {
			ChainLink link = _chain.Pop();
			if (link.Effect != null)
				link.Effect.Resolve(link);
		}
	}
	
}


public class ChainLink {

	public Player Owner { get; set; }
	public CardInstance Card { get; set; }
	public ICardEffect Effect { get; set; }

	public List<Target> Targets { get; set; } = new();

	public ChainLink(Player owner, CardInstance card, ICardEffect effect) {
		Owner = owner;
		Card = card;
		Effect = effect;
	}
}