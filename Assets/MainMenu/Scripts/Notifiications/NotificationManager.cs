using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NotificationManager : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private GameObject _notificationPanel;
	[SerializeField] private Transform _contentParent; // ScrollView content
	[SerializeField] private Button _toggleButton;

	[Header("Prefabs")]
	[SerializeField] private GameObject _normalToastPrefab;
	[SerializeField] private GameObject _newsToastPrefab;

	private bool _isPanelVisible = true;

	private void Start()
	{
		_toggleButton.onClick.AddListener(TogglePanel);
	}

	public void TogglePanel()
	{
		_isPanelVisible = !_isPanelVisible;
		_notificationPanel.SetActive(_isPanelVisible);
	}

	public void ShowNotification(string message, bool isWarning = false)
	{
		GameObject toastPrefab = isWarning ? _newsToastPrefab : _normalToastPrefab;
		GameObject toastInstance = Instantiate(toastPrefab, _contentParent);

		TMP_Text textComponent = toastInstance.GetComponentInChildren<TMP_Text>();
		if (textComponent != null)
		{
			textComponent.text = message;
		}

		// Optionally, scroll to bottom if needed:
		Canvas.ForceUpdateCanvases();
		ScrollRect scrollRect = _notificationPanel.GetComponentInChildren<ScrollRect>();
		if (scrollRect != null)
		{
			scrollRect.verticalNormalizedPosition = 0f;
		}
	}
}
