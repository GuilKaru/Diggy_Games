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
		[SerializeField]
		private SpawnerSide _side;

		public enum SpawnerSide { Left, Right }

		private SpawnerManager _spawnerManager;
		private bool canSpawn = true;




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
				if (_spawnIntervals.Length > 0 && canSpawn)
				{
					float nextSpawnTime = _spawnIntervals[Random.Range(0, _spawnIntervals.Length)];

					// Just spawn the barrel directly (no warning)
					float yPos = _spawnerManager.GetAvailablePosition();

					if (yPos != -2)
					{
						_spawnerManager.OccupyPosition(yPos);
						SpawnBarrelAtPosition(yPos);
					}

					yield return new WaitForSeconds(nextSpawnTime);
				}
				else
				{
					yield return new WaitForSeconds(1f);
				}
			}
		}

		public void SpawnBarrelAtPosition(float yPos)
		{
			if (_barrelPrefabs.Length == 0)
			{
				Debug.LogWarning("BarrelPrefabs are empty.");
				return;
			}

			Vector2 spawnPos = new Vector2(transform.position.x, yPos);
			GameObject randomBarrel = _barrelPrefabs[Random.Range(0, _barrelPrefabs.Length)];
			GameObject barrel = Instantiate(randomBarrel, spawnPos, Quaternion.identity, _barrelParent);

			// Set sorting order
			SpriteRenderer spriteRenderer = barrel.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				spriteRenderer.sortingOrder = Mathf.RoundToInt(yPos * -10);
			}

			Debug.Log($"Barrel spawned at position: {spawnPos} with sorting order: {spriteRenderer?.sortingOrder}");

			StartCoroutine(ReleasePositionAfterDelay(yPos, 8.5f));
		}

		public void SetSpawning(bool value)
		{
			canSpawn = value; // Enable or disable spawning
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

		public void SetSide(SpawnerSide side)
		{
			_side = side;
		}

	}
}


