using UnityEngine;

public class FieldView : MonoBehaviour {
	[SerializeField] private GameObject cardPrefab;

	private DuelController duelController;
	private CardView currentCard;
	private Player player;
	private int zoneIndex;

	public void Initialize(DuelController controller, Player player, int zoneIndex) {
		this.duelController = controller;
		this.player = player;
		this.zoneIndex = zoneIndex;

		duelController.OnCardMoved += HandleCardMoved;
	}

	private void HandleCardMoved(Player changedPlayer, CardInstance card, CardZone from, CardZone to) {
		if (changedPlayer != player) return;
		if (from == CardZone.Field || to == CardZone.Field) RefreshField(player);
	}

	private void RefreshField(Player changedPlayer) {
		if (changedPlayer != player) return;
		CardInstance instance = player.field.MonsterZones[zoneIndex];
		if (instance == null) {
			if (currentCard != null) {
				Destroy(currentCard.gameObject);
				currentCard = null;
			}
			return;
		}
		if (currentCard == null) {
			GameObject obj = Instantiate(cardPrefab, transform, false);
			obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			currentCard = obj.GetComponent<CardView>();
		}
		currentCard.Bind(instance);
		currentCard.BattlePositionRefresh();
	}
}
