using UnityEngine;

namespace Diggy_MiniGame_1
{
	public class BuffSpawner : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields
		[SerializeField]
		private Transform _buffParent;

		[SerializeField]
		private GameObject[] _buffPrefabs;

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
			InvokeRepeating(nameof(SpawnBuff), 0f, _spawnInterval);
		}
		#endregion

		// Spawn Logic
		#region Spawn Logic
		private void SpawnBuff()
		{
			if (_buffPrefabs == null || _buffPrefabs.Length == 0) return;

			// Pick a random enemy prefab
			int randomBuffIndex = Random.Range(0, _buffPrefabs.Length);
			GameObject buffToSpawn = _buffPrefabs[randomBuffIndex];

			// Randomize the spawn position
			float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y); // Random X position
			float randomY = _fixedYPositions[Random.Range(0, _fixedYPositions.Length)]; // Random fixed Y position
			Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

			if (SpawnManager.TryRegisterPosition(spawnPosition))
			{
				Quaternion spawnRotation = Quaternion.Euler(0f, 0f, _spawnRotation);
				Instantiate(buffToSpawn, new Vector3(spawnPosition.x, spawnPosition.y, 0f), spawnRotation, _buffParent);
				return; // Exit after successful spawn
			}
		}
		#endregion

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
			Gizmos.color = Color.red;

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
