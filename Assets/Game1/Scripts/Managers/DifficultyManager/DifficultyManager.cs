using UnityEngine;

namespace Diggy_MiniGame_1
{
	public class DifficultyManager : MonoBehaviour
	{
		// References
		#region References
		[Header("Spawners")]
		[SerializeField]
		private EnemySpawner _sideSpawner; // Spawner for right-side enemies

		[SerializeField]
		private LavaDropSpawner _lavaDropSpawner; // Spawner for falling lava drops

		[Header("Score Management")]
		[SerializeField]
		private ScoreManager _scoreManager; // Reference to ScoreManager

		[SerializeField]
		private int _pointsPerLevel = 50;
		#endregion

		// Configuration
		#region Configuration
		[SerializeField]
		private int _lavaSpawnerUnlockScore = 150; // Score to unlock the lava spawner
		#endregion

		// Variables
		#region Variables
		private int _currentDifficultyLevel = 0; // Current difficulty level
		private bool _isLavaSpawnerUnlocked = false; // Track if the lava spawner is unlocked
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Ensure the lava spawner is disabled at the start
			if (_lavaDropSpawner != null)
			{
				_lavaDropSpawner.StopSpawning();
			}
		}
		#endregion

		// Update Loop
		#region Update Loop
		private void Update()
		{

			// Check the score and unlock the lava spawner when the threshold is reached
			int currentScore = _scoreManager.GetScore();

			if (!_isLavaSpawnerUnlocked && currentScore >= _lavaSpawnerUnlockScore)
			{
				UnlockLavaSpawner();
			}

			// Calculate the current difficulty level based on the score
			int calculatedDifficultyLevel = _scoreManager.GetScore() / _pointsPerLevel;

			// If the difficulty has increased, notify the EnemySpawner
			if (calculatedDifficultyLevel > _currentDifficultyLevel)
			{
				_currentDifficultyLevel = calculatedDifficultyLevel;

				Debug.Log("Difficulty increased to: " + _currentDifficultyLevel);

				// Notify the EnemySpawner to unlock a new enemy
				_sideSpawner.IncreaseDifficulty(_currentDifficultyLevel);

				// Increase the number of enemies per interval
				_sideSpawner.IncreaseEnemiesPerInterval();
			}

		}
		#endregion

		// Unlock Logic
		#region Unlock Logic
		private void UnlockLavaSpawner()
		{
			_isLavaSpawnerUnlocked = true;

			// Enable the lava spawner to start spawning drops
			if (_lavaDropSpawner != null)
			{
				_lavaDropSpawner.StartSpawning();
				Debug.Log("Lava spawner unlocked!");
			}
		}
		#endregion

	}
}

