using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickUpTime : MonoBehaviour
	{
		[SerializeField] private float _bonusTime = 10f;
		private TimerManager _timerManager;

		private void Awake()
		{
			_timerManager = FindObjectOfType<TimerManager>();
		}

		public void HandleDrop()
		{
			if (_timerManager != null)
			{
				_timerManager.AddTime(_bonusTime);
				Debug.Log($"Time Added: {_bonusTime} seconds!");
			}
		}

		
	}
}

