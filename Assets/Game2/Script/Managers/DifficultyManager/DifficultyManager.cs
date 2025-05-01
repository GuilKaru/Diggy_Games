using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class DifficultyManager : MonoBehaviour
	{
		[SerializeField]
		private TimerManager _timerManager;
		[SerializeField]
		private PlayerController _playerController;
		[SerializeField]
		private float difficultyIncreaseTime = 60f; // 1 minute mark
		[SerializeField]
		private float driftIncrement = 0.05f; // How much to increase each time

		private bool difficultyIncreased = false;

		void Update()
		{
			if (!difficultyIncreased && _timerManager.GetCurrentTime() <= (_timerManager.GetStartingTime() - difficultyIncreaseTime))
			{
				_playerController.IncreaseDriftDifficulty(driftIncrement);
				difficultyIncreased = true;
			}
		}
	}

}
