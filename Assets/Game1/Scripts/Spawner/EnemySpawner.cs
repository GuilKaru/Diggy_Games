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

			// Pick a random enemy prefab
			int randomEnemyIndex = Random.Range(0, _enemyPrefabs.Length);
			GameObject enemyToSpawn = _enemyPrefabs[randomEnemyIndex];

			// Randomize the spawn position
			float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y); // Random X position
			float randomY = _fixedYPositions[Random.Range(0, _fixedYPositions.Length)]; // Random fixed Y position
			Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

			// Spawn the enemy
			Quaternion spawnRotation = Quaternion.Euler(0f, 0f, _spawnRotation);
			Instantiate(enemyToSpawn, spawnPosition, spawnRotation, _enemyParent);
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


