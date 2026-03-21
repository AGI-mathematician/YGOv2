using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	[SerializeField] private Button DP;
	[SerializeField] private Button SP;
	[SerializeField] private Button MP1;
	[SerializeField] private Button BP;
	[SerializeField] private Button MP2;
	[SerializeField] private Button EP;
	[SerializeField] private Transform actionPanel;

	public Transform ActionPanel => actionPanel;

	private DuelController _duelController;

	public void Initialize(DuelController duelController) {
		_duelController = duelController;
		HighlightCurrentPhase();					//Remove and fire upon DC changing phases
	}

	public void OnSPClick() {
		_duelController.RequestPhaseChange(Phase.SP);
		HighlightCurrentPhase();					//Remove and fire upon DC changing phases
	}

	public void OnMP1Click() {
		_duelController.RequestPhaseChange(Phase.MP1);
		HighlightCurrentPhase();					//Remove and fire upon DC changing phases
	}

	public void OnBPClick() {
		_duelController.RequestPhaseChange(Phase.BP);
		HighlightCurrentPhase();					//Remove and fire upon DC changing phases
	}

	public void OnMP2Click() {
		_duelController.RequestPhaseChange(Phase.MP2);
		HighlightCurrentPhase();					//Remove and fire upon DC changing phases
	}

	public void OnEPClick() {
		_duelController.RequestPhaseChange(Phase.EP);
		HighlightCurrentPhase();					//Remove and fire upon DC changing phases
	}

	public void HighlightCurrentPhase() {
		switch(_duelController.CurrentPhase) {
			case Phase.DP:
				ResetButtons();
				DP.GetComponent<Image>().color = Color.yellow;
				break;
			case Phase.SP:
				ResetButtons();
				SP.GetComponent<Image>().color = Color.yellow;
				break;
			case Phase.MP1:
				ResetButtons();
				MP1.GetComponent<Image>().color = Color.yellow;
				break;
			case Phase.BP:
				ResetButtons();
				BP.GetComponent<Image>().color = Color.yellow;
				break;
			case Phase.MP2:
				ResetButtons();
				MP2.GetComponent<Image>().color = Color.yellow;
				break;
			case Phase.EP:
				ResetButtons();
				EP.GetComponent<Image>().color = Color.yellow;
				break;
											//use ResetButton() on phase change event from DueLController instead to reduce repetitive code
		}
	}

	private void ResetButtons() {
		DP.GetComponent<Image>().color = Color.white;
		SP.GetComponent<Image>().color = Color.white;
		MP1.GetComponent<Image>().color = Color.white;
		BP.GetComponent<Image>().color = Color.white;
		MP2.GetComponent<Image>().color = Color.white;
		EP.GetComponent<Image>().color = Color.white;
	}
	
}
