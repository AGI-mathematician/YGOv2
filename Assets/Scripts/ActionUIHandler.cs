using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class ActionUIHandler {

	private Button buttonPrefab;
	public event Action<CardActionType> OnActionSelected;
	public event Action<BattlePosition> OnBattlePositionSelected;
	public event Action<bool> OnYesNoSelected;
	public event Action<CardInstance> OnCardSelected;

	public ActionUIHandler(Button buttonPrefab) {
		this.buttonPrefab = buttonPrefab;
	}

	public void BuildActionButtons(List<CardActionType> legalActions, Transform parent) {
		foreach (Transform child in parent)
			UnityEngine.Object.Destroy(child.gameObject);

		foreach (CardActionType action in legalActions) {
			Button button = UnityEngine.Object.Instantiate(buttonPrefab, parent.transform, false);
			TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
			if (tmp != null) tmp.text = action.ToString();
			CardActionType captured = action;
			button.onClick.AddListener(() => OnActionSelected?.Invoke(captured));
		}
	}

	public void BuildYesNo(string message, Transform parent) {
		foreach (Transform child in parent)
			UnityEngine.Object.Destroy(child.gameObject);

		Button yes = UnityEngine.Object.Instantiate(buttonPrefab, parent, false);
		Button no = UnityEngine.Object.Instantiate(buttonPrefab, parent, false);

		yes.GetComponentInChildren<TextMeshProUGUI>().text = "Yes";
		no.GetComponentInChildren<TextMeshProUGUI>().text = "No";

		yes.onClick.AddListener(() => OnYesNoSelected?.Invoke(true));
		no.onClick.AddListener(() => OnYesNoSelected?.Invoke(false));
	}

	public void BuildCardSelection(List<CardInstance> options, Transform parent) {
		foreach (Transform child in parent)
			UnityEngine.Object.Destroy(child.gameObject);

		foreach (var card in options) {
			Button button = UnityEngine.Object.Instantiate(buttonPrefab, parent, false);

			TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
			if (tmp != null) {
    				tmp.text = card.CardData.Name;
    				tmp.enableAutoSizing = true;
    				tmp.fontSizeMin = 10;
    				tmp.fontSizeMax = 24;
			}

			CardInstance captured = card;
			button.onClick.AddListener(() => OnCardSelected?.Invoke(captured));
		}
	}

	public void BuildAttackDefenseChoice(Transform parent) {
		foreach (Transform child in parent)
			UnityEngine.Object.Destroy(child.gameObject);

		Button attack = UnityEngine.Object.Instantiate(buttonPrefab, parent, false);
		Button defense = UnityEngine.Object.Instantiate(buttonPrefab, parent, false);

		attack.GetComponentInChildren<TextMeshProUGUI>().text = "Attack";
		defense.GetComponentInChildren<TextMeshProUGUI>().text = "Defense";

		attack.onClick.AddListener(() => OnBattlePositionSelected?.Invoke(BattlePosition.Attack));

		defense.onClick.AddListener(() => OnBattlePositionSelected?.Invoke(BattlePosition.Defense));
	}
}