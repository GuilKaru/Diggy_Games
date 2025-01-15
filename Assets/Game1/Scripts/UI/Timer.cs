using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Diggy_MiniGame_1
{
	public class Timer : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[SerializeField]
		private TextMeshProUGUI _timerText;
		#endregion

		//Private Variables
		#region Private Variables
		private float _elapsedTime; 
		private bool _isRunning;
		#endregion


		void Start()
		{
			_elapsedTime = 0f;
			_isRunning = true; // Start counting immediately
		}

		void Update()
		{
			if (_isRunning)
			{
				_elapsedTime += Time.deltaTime; // Add the time since the last frame
				UpdateTimerUI();
			}
		}

		void UpdateTimerUI()
		{
			// Format the elapsed time to display minutes and seconds
			int minutes = Mathf.FloorToInt(_elapsedTime / 60F);
			int seconds = Mathf.FloorToInt(_elapsedTime % 60F);
			_timerText.text = $"{minutes:00}:{seconds:00}";
		}

		// Optional: Stop the timer
		public void StopTimer()
		{
			_isRunning = false;
		}

		// Optional: Reset the timer
		public void ResetTimer()
		{
			_elapsedTime = 0f;
			UpdateTimerUI();
			_isRunning = true;
		}
	}

}
