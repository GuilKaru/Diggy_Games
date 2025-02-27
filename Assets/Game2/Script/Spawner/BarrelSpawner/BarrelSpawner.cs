using System.Collections;
using UnityEngine;
namespace Diggy_MiniGame_2 
{
	public class BarrelSpawner : MonoBehaviour
	{
		[Header("Spawner Settings")]
		[SerializeField]
		private GameObject[] _barrelPrefabs; // Array of barrel prefabs
		[SerializeField]
		private Transform _barrelParent; // Parent transform for spawned barrels
		[SerializeField]
		private float _initialDelay = 2f; // Initial delay before spawning starts
		[SerializeField]
		private float[] _spawnIntervals; // Array of spawn intervals

		private SpawnerManager _spawnerManager;



		private void Start()
		{
			StartCoroutine(SpawnBarrelsWithDelay());
		}

		public void SetSpawnerManager(SpawnerManager manager)
		{
			_spawnerManager = manager;
		}

		private IEnumerator SpawnBarrelsWithDelay()
		{
			yield return new WaitForSeconds(_initialDelay);

			while (true)
			{
				SpawnBarrel();

				if (_spawnIntervals.Length > 0)
				{
					float nextSpawnTime = _spawnIntervals[Random.Range(0, _spawnIntervals.Length)];
					yield return new WaitForSeconds(nextSpawnTime);
				}
				else
				{
					// Wait for a default interval (1 second or a value of your choice)
					yield return new WaitForSeconds(1f);
				}
			}
		}

		public void SpawnBarrel()
		{
			if (_spawnerManager == null || _barrelPrefabs.Length == 0)
			{
				Debug.LogWarning("SpawnerManager is null or barrelPrefabs are empty.");
				return;
			}

			float yPos = _spawnerManager.GetAvailablePosition();
			Debug.Log($"Trying to spawn barrel at Y position: {yPos}");

			if (yPos != -1)
			{
				Vector2 spawnPos = new Vector2(transform.position.x, yPos);
				GameObject randomBarrel = _barrelPrefabs[Random.Range(0, _barrelPrefabs.Length)];
				GameObject barrel = Instantiate(randomBarrel, spawnPos, Quaternion.identity, _barrelParent);

				// Set the sorting order based on the Y position
				SpriteRenderer spriteRenderer = barrel.GetComponent<SpriteRenderer>();
				if (spriteRenderer != null)
				{
					spriteRenderer.sortingOrder = Mathf.RoundToInt(yPos * -10); // Higher Y position gets a lower sorting order
				}

				_spawnerManager.OccupyPosition(yPos);

				Debug.Log($"Barrel spawned at position: {spawnPos} with sorting order: {spriteRenderer?.sortingOrder}");

				StartCoroutine(ReleasePositionAfterDelay(yPos, 5f)); // Release the position after 5 seconds
			}
			else
			{
				Debug.LogWarning("No available Y position for spawning.");
			}
		}

		private IEnumerator ReleasePositionAfterDelay(float yPos, float delay)
		{
			yield return new WaitForSeconds(delay);
			_spawnerManager.ReleasePosition(yPos);
		}

		// Draw Gizmos to visualize the spawn area
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
		}
	}
}


