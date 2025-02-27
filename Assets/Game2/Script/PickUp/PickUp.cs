using Diggy_MiniGame_1;
using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickUp : MonoBehaviour
	{
		[Header("Pickup Settings")]
		[SerializeField] private int _scoreValue = 10;     // Score Value
		[SerializeField] private bool _isTimePickup = false; // Toggle if it's a Time Pickup
		[SerializeField] private float _bonusTime = 10f;    // Bonus Time (if it's a time pickup)

		private ScoreManager _scoreManager;
		private TimerManager _timerManager;

		private void Awake()
		{
			_scoreManager = FindObjectOfType<ScoreManager>();
			_timerManager = FindObjectOfType<TimerManager>();
		}

		public void HandleDrop()
		{
			if (_isTimePickup)
			{
				if (_timerManager != null)
				{
					_timerManager.AddTime(_bonusTime);
					Debug.Log($"Time Added: {_bonusTime} seconds!");
				}
			}
			else
			{
				if (_scoreManager != null)
				{
					_scoreManager.AddScore(_scoreValue);
					Debug.Log($"Score Added: {_scoreValue}");
				}
			}
		}

		// Helper to check if it's a Time Pickup
		public bool IsTimePickup()
		{
			return _isTimePickup;
		}

		// Helper to get Bonus Time
		public float GetBonusTime()
		{
			return _bonusTime;
		}

	}
}

