using TMPro;
using UnityEngine;

namespace Diggy_MiniGame_2
{
	public class TimerManager : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _timerText;
		[SerializeField] private float _startingTime = 180f; // 3 minutes

		private float _currentTime;
		private bool _isRunning = true;

		private void Start()
		{
			_currentTime = _startingTime;
			UpdateTimerUI();
		}

		private void Update()
		{
			if (_isRunning)
			{
				_currentTime -= Time.deltaTime;
				UpdateTimerUI();

				if (_currentTime <= 0)
				{
					_currentTime = 0;
					_isRunning = false;
					Debug.Log("Game Over!");
				}
			}
		}

		private void UpdateTimerUI()
		{
			int minutes = Mathf.FloorToInt(_currentTime / 60);
			int seconds = Mathf.FloorToInt(_currentTime % 60);
			_timerText.text = $"{minutes:00}:{seconds:00}";
		}

		public void AddTime(float bonusTime)
		{
			_currentTime += bonusTime;
			Debug.Log($"Bonus Time: {bonusTime} seconds!");

			// Optional: Cap time to the starting time
			_currentTime = Mathf.Min(_currentTime, _startingTime);
		}
	}
}

