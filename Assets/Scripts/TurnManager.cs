using UnityEngine;

public class TurnManager {
	public Player TurnPlayer { get; private set; }
	public Phase CurrentPhase { get; private set; }
	public bool HasNormalSummoned { get; private set; } = false;

	public void AdvancePhase() {
		switch (CurrentPhase) {
			case Phase.DP:
				CurrentPhase = Phase.SP;
				break;
			case Phase.SP:
				CurrentPhase = Phase.MP1;
				break;
			case Phase.MP1:
				CurrentPhase = Phase.BP;
				break;
			case Phase.BP:
				CurrentPhase = Phase.MP2;
				break;
			case Phase.MP2:
				CurrentPhase = Phase.EP;
				break;
			case Phase.EP:
				StartNextTurn();
				CurrentPhase = Phase.DP;
				break;
		}
	}

	public void AdvanceToEP() {
		CurrentPhase = Phase.EP;
	}

	public void RegisterNormalSummon() {
		HasNormalSummoned = true;
	}

	public void StartNextTurn() {
		TurnPlayer = TurnPlayer.opponent;
		HasNormalSummoned = false;
	}
}
