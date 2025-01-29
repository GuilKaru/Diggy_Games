using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Diggy_MiniGame_1
{
	public class PremiumBuffTooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private string tooltipText; // The text to display
		[SerializeField]
		private GameObject tooltipObject; // Tooltip UI Panel
		[SerializeField]
		private TextMeshProUGUI tooltipTextComponent; // The Text UI element (or TMP_Text for TextMeshPro)

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (tooltipObject != null && tooltipTextComponent != null)
			{
				tooltipTextComponent.text = tooltipText; // Set tooltip text
				tooltipObject.SetActive(true); // Show tooltip
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (tooltipObject != null)
			{
				tooltipObject.SetActive(false); // Hide tooltip when the mouse leaves
			}
		}

		void Update()
		{
			/*if (tooltipObject.activeSelf)
			{
				tooltipObject.transform.position = Input.mousePosition;
			}*/
		}
	}
}