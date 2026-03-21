using UnityEngine;

public class HandView : MonoBehaviour {
	[SerializeField] private Transform actionPanel;
	[SerializeField] private GameObject card;
	private CardView currentlySelected;
	private DuelController duelController;
	private Player player;

	public void RefreshHandUI(Player player) {
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}
		foreach (CardInstance instance in player.Hand) {
			GameObject spawned = Instantiate(card, transform, false);
			CardView view = spawned.GetComponent<CardView>();
			view.Bind(instance);
			view.OnCardClicked += HandleCardClicked;
		}
	}

	private void HandleCardClicked(CardView view) {
		if (currentlySelected == view) {
			ClearButtons(view.UIRoot);
			currentlySelected = null;
			return;
		}
		if (currentlySelected != null)
			ClearButtons(currentlySelected.UIRoot);
		currentlySelected = view;
		duelController.RefreshAvailableActions(player, view.BoundInstance, view.UIRoot);
	}

	private void ClearButtons(Transform parent) {
		foreach (Transform child in parent) {
			if (child.GetComponent<UnityEngine.UI.Button>() != null)
				Destroy(child.gameObject);
		}
	}

private void HandleCardMoved(Player changedPlayer, CardInstance card, CardZone from, CardZone to) {
		if (changedPlayer != player) return;
		if (from == CardZone.Hand || to == CardZone.Hand)
		RefreshHandUI(player);
	}

	public void Initialize(DuelController controller, Player player) {
		duelController = controller;
		this.player = player;
		duelController.OnCardDrawn += RefreshHandUI;
		duelController.OnCardMoved += HandleCardMoved;
	}
}
