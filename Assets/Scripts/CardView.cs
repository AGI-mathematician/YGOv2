using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CardView : MonoBehaviour, IPointerClickHandler {

	[SerializeField] private Transform visual;
	[SerializeField] private Sprite faceDownSprite;
	public event Action<CardView> OnCardClicked;
	public CardInstance BoundInstance { get; private set; }
	private Image image;
	[SerializeField] private Transform uiRoot;
	public Transform UIRoot => uiRoot;

	public void Awake() {
		image = visual.GetComponent<Image>();
	}

	public void Bind(CardInstance instance) {
		BoundInstance = instance;
		image.sprite = BoundInstance.IsFaceUp
			? BoundInstance.CardData.Artwork
			: faceDownSprite;
		BattlePositionRefresh();
	}

	public void OnPointerClick(PointerEventData eventData) {
		OnCardClicked?.Invoke(this);
	}

	public void BattlePositionRefresh() {
		if (BoundInstance.CardData is MonsterCard) {
			var monster = BoundInstance.GetComponent<MonsterFieldComponent>();
			bool isDefense = (monster != null && monster.Position == BattlePosition.Defense);
			visual.localRotation = isDefense
				? Quaternion.Euler(0, 0, 90)
				: Quaternion.identity;
		}
	}
}