using UnityEngine;

namespace Diggy_MiniGame_1
{
	public class EnemySpawner : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields
		[SerializeField]
		private Transform _enemyParent;

		[SerializeField]
		private GameObject[] _enemyPrefabs;

		[SerializeField]
		private float _spawnInterval = 2f; // Time between spawns

		[SerializeField]
		private Vector2 _spawnXRange = new Vector2(-5f, 5f); // Random x position range

		[SerializeField]
		private float[] _fixedYPositions = { 2f, 0f, -2f, -4f, -6f, -8f }; // Fixed Y positions

		[SerializeField]
		private float _spawnRotation = 0f; // Default spawn rotation
		#endregion

		//Private variables
		#region Private variables
		private const int MaxAttempts = 10;
		private int _unlockedEnemyIndex = 0;
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			InvokeRepeating(nameof(SpawnEnemy), 0f, _spawnInterval);
		}
		#endregion

		// Spawn Logic
		#region Spawn Logic
		private void SpawnEnemy()
		{
			if (_enemyPrefabs == null || _enemyPrefabs.Length == 0) return;

			// Ensure we only spawn unlocked enemies
			int randomEnemyIndex = Random.Range(0, _unlockedEnemyIndex + 1);
			GameObject enemyToSpawn = _enemyPrefabs[randomEnemyIndex];

			// Rest of the spawn logic remains unchanged...
			float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);
			float randomY = _fixedYPositions[Random.Range(0, _fixedYPositions.Length)];
			Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

			if (SpawnManager.TryRegisterPosition(spawnPosition))
			{
				Quaternion spawnRotation = Quaternion.Euler(0f, 0f, _spawnRotation);
				Instantiate(enemyToSpawn, new Vector3(spawnPosition.x, spawnPosition.y, 0f), spawnRotation, _enemyParent);
				return;
			}
		}
		#endregion

		//Difficulty
		#region Difficulty
		public void IncreaseDifficulty(int difficultyLevel)
		{
			// Unlock new enemy type if within bounds
			if (difficultyLevel < _enemyPrefabs.Length)
			{
				_unlockedEnemyIndex = difficultyLevel;
			}
		}
		#endregion


		//On DestroyCleanup
		#region OnDestroy Cleanup
		private void OnDestroy()
		{
			// Cleanup positions if spawner is destroyed (optional)
			SpawnManager.ClearAllPositions();
		}
		#endregion

		// Gizmos
		#region Gizmos
		private void OnDrawGizmos()
		{
			// Visualize the spawn area
			Gizmos.color = Color.green;

			// Draw a vertical line for each fixed Y position
			foreach (float y in _fixedYPositions)
			{
				Vector3 start = new Vector3(_spawnXRange.x, y, 0f);
				Vector3 end = new Vector3(_spawnXRange.y, y, 0f);
				Gizmos.DrawLine(start, end);
			}
		}
		#endregion
	}
}


