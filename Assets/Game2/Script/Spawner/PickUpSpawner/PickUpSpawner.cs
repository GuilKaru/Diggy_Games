using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickupSpawner : MonoBehaviour
	{
		[Header("Spawner Settings")]
		[SerializeField]
		private GameObject[] _pickupPrefabs;
		[SerializeField]
		private Transform _pickupParent;
		[SerializeField]
		private Transform[] _spawnPoints;
		[SerializeField]
		private int _maxPickups = 10;
		[SerializeField]
		private float _spawnInterval = 3f;

		[Header("Special Pickup Settings")]
		[SerializeField] private GameObject _specialPickupPrefab; // The new pickup type
		[SerializeField][Range(0f, 100f)] private float _specialPickupChance = 5f; // Adjustable probability in %

		private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>(); // Keep track of occupied positions

		private void Start()
		{
			InvokeRepeating(nameof(SpawnPickup), 0f, _spawnInterval);
		}

		private void SpawnPickup()
		{
			int currentPickups = _pickupParent.childCount;
			if (currentPickups >= _maxPickups || _spawnPoints.Length == 0) return;

			// Find available spawn points
			List<Transform> availableSpawnPoints = _spawnPoints.Where(spawn => !occupiedSpawnPoints.Contains(spawn)).ToList();
			if (availableSpawnPoints.Count == 0) return;

			// Pick a random available spawn point
			Transform spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];

			GameObject pickupToSpawn;
			if (_specialPickupPrefab != null && Random.Range(0f, 100f) <= _specialPickupChance)
			{
				// Spawn special pickup
				pickupToSpawn = _specialPickupPrefab;
			}
			else
			{
				// Spawn normal pickup
				if (_pickupPrefabs.Length == 0) return;
				pickupToSpawn = _pickupPrefabs[Random.Range(0, _pickupPrefabs.Length)];
			}

			// Instantiate and mark the spawn point as occupied
			GameObject spawnedPickup = Instantiate(pickupToSpawn, spawnPoint.position, Quaternion.identity, _pickupParent);
			occupiedSpawnPoints.Add(spawnPoint);

			// Attach script to track when it's destroyed
			spawnedPickup.AddComponent<PickupTracker>().Initialize(spawnPoint, this);

		}

		public void FreeSpawnPoint(Transform spawnPoint)
		{
			occupiedSpawnPoints.Remove(spawnPoint);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			foreach (Transform point in _spawnPoints)
			{
				if (point != null)
				{
					Gizmos.DrawWireSphere(point.position, 0.2f); // Draw spheres at each spawn point
				}
			}
		}


	}

	public class PickupTracker : MonoBehaviour
	{
		private Transform spawnPoint;
		private PickupSpawner spawner;

		public void Initialize(Transform spawnPoint, PickupSpawner spawner)
		{
			this.spawnPoint = spawnPoint;
			this.spawner = spawner;
		}

		public void NotifyPickup()
		{
			if (spawner != null)
			{
				spawner.FreeSpawnPoint(spawnPoint);
			}

		}
	}

}

