using Diggy_MiniGame_1;
using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickUp : MonoBehaviour
	{
		[Header("Pickup Settings")]
		[SerializeField] private int _scoreValue = 10;

		private ScoreManager _scoreManager;

		private void Awake()
		{
			_scoreManager = FindObjectOfType<ScoreManager>(); // Ensure ScoreManager is in the scene
		}

		public void HandleDrop()
		{
			if (_scoreManager != null)
			{
				Debug.Log($"Adding Score: {_scoreValue}");
				_scoreManager.AddScore(_scoreValue);
			}
			else
			{
				Debug.LogWarning("ScoreManager not found!");
			}
		}
	}
}

