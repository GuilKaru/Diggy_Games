using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diggy_MiniGame_1
{
	public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[Header("Default Button")]
		[SerializeField]
		private Sprite _defaultSprite; // Default image
		[SerializeField]
		private Vector2 _defaultSize = new Vector2(160, 60); // Default size
		[Header("Highlight Button")]
		[SerializeField]
		private Sprite _highlightedSprite; // Highlighted image
		[SerializeField]
		private Vector2 _highlightedSize = new Vector2(180, 80); // Highlighted size

		private Image buttonImage;
		private RectTransform rectTransform;

		private void Awake()
		{
			buttonImage = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();

			// Set the initial size to default size
			rectTransform.sizeDelta = _defaultSize;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			// Change to highlighted image and size
			buttonImage.sprite = _highlightedSprite;
			rectTransform.sizeDelta = _highlightedSize;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			// Change back to default image and size
			buttonImage.sprite = _defaultSprite;
			rectTransform.sizeDelta = _defaultSize;
		}
	}

}
