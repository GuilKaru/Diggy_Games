using System.Collections.Generic;
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
		private GameObject[] _specialEnemyPrefabs;

		[SerializeField]
		private int _enemiesPerInterval = 1;

		public float _spawnInterval = 2f; // Time between spawns

		[SerializeField]
		private Vector2 _spawnXRange = new Vector2(-5f, 5f); // Random x position range

		[SerializeField]
		private float[] _fixedYPositions = { 2f, 0f, -2f, -4f, -6f, -8f }; // Fixed Y positions

		[SerializeField]
		private float _spawnRotation = 0f; // Default spawn rotation

		[SerializeField, Range(0f, 100f)]
		private float _specialEnemyChance = 30f;
		#endregion

		//Private variables
		#region Private variables
		private const int MaxAttempts = 10;
		private int _unlockedEnemyIndex = 0;
		private bool canSpawn = true;
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
		public void SpawnEnemy()
		{
			if (!canSpawn || (_enemyPrefabs == null && _specialEnemyPrefabs == null)) return;

			// Shuffle the available Y positions to ensure randomness
			List<float> availablePositions = new List<float>(_fixedYPositions);
			availablePositions = ShuffleList(availablePositions);

			// Determine how many enemies to spawn (capped by available positions)
			int enemiesToSpawn = Mathf.Min(_enemiesPerInterval, availablePositions.Count);

			for (int i = 0; i < enemiesToSpawn; i++)
			{
				GameObject enemyToSpawn;

				// 30% chance to spawn a special enemy
				if (_specialEnemyPrefabs != null && _specialEnemyPrefabs.Length > 0 && Random.value * 100f <= _specialEnemyChance)
				{
					int randomSpecialIndex = Random.Range(0, _specialEnemyPrefabs.Length);
					enemyToSpawn = _specialEnemyPrefabs[randomSpecialIndex];
				}
				else
				{
					// Spawn a regular enemy
					int randomEnemyIndex = Random.Range(0, _unlockedEnemyIndex + 1);
					enemyToSpawn = _enemyPrefabs[randomEnemyIndex];
				}

				// Get a unique Y position
				float randomY = availablePositions[i];

				// Randomize spawn X position
				float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);

				Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

				// Check and spawn
				if (SpawnManager.TryRegisterPosition(spawnPosition))
				{
					Quaternion spawnRotation = Quaternion.Euler(0f, 0f, _spawnRotation);
					Instantiate(enemyToSpawn, spawnPosition, spawnRotation, _enemyParent);
				}
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

		public void IncreaseEnemiesPerInterval()
		{
			if (_enemiesPerInterval < 4)
			{
				_enemiesPerInterval++;
				Debug.Log($"Enemies per interval increased to: {_enemiesPerInterval}");
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

		#region Control Spawning
		public void SetSpawning(bool value)
		{
			canSpawn = value; // Enable or disable spawning
		}

		// Utility method to shuffle a list
		private List<T> ShuffleList<T>(List<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int randomIndex = Random.Range(i, list.Count);
				T temp = list[i];
				list[i] = list[randomIndex];
				list[randomIndex] = temp;
			}
			return list;
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


