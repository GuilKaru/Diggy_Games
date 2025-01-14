using UnityEngine;

namespace Diggy_MiniGame_1
{
	public class DifficultyManager : MonoBehaviour
	{
		// References
		#region References
		[SerializeField]
		private ScoreManager _scoreManager; // Reference to ScoreManager

		[SerializeField]
		private EnemySpawner _enemySpawner; // Reference to EnemySpawner
		#endregion

		// Configuration
		#region Configuration
		[SerializeField]
		private int _pointsPerLevel = 50; // Points required to level up difficulty
		#endregion

		// Variables
		#region Variables
		private int _currentDifficultyLevel = 0; // Current difficulty level
		#endregion

		// Monitor the score and adjust difficulty
		#region Difficulty Adjustment
		private void Update()
		{
			// Calculate the current difficulty level based on the score
			int calculatedDifficultyLevel = _scoreManager.GetScore() / _pointsPerLevel;

			// If the difficulty has increased, notify the EnemySpawner
			if (calculatedDifficultyLevel > _currentDifficultyLevel)
			{
				_currentDifficultyLevel = calculatedDifficultyLevel;

				Debug.Log("Difficulty increased to: " + _currentDifficultyLevel);

				// Notify the EnemySpawner to unlock a new enemy
				_enemySpawner.IncreaseDifficulty(_currentDifficultyLevel);
			}
		}
		#endregion

	}
}

