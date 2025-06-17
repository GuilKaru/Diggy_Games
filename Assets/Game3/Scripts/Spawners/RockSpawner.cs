using System.Collections.Generic;
using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class RockSpawner : MonoBehaviour
	{
		[Header("Spawner Settings")]
		public List<GameObject> _rockPrefabs;     // List of rock prefabs to spawn
		public Transform _rockParent;
		public float _spawnInterval = 2f;         // Time between spawns
		public Vector2 _spawnRangeX = new Vector2(-5f, 5f); // X-axis range for spawning

		private float timer;

		void Update()
		{
			timer += Time.deltaTime;

			if (timer >= _spawnInterval)
			{
				SpawnRock();
				timer = 0f;
			}
		}

		void SpawnRock()
		{
			if (_rockPrefabs.Count == 0)
			{
				Debug.LogWarning("No rock prefabs assigned to the spawner.");
				return;
			}

			// Random position on the X-axis within the spawn range
			float spawnX = Random.Range(_spawnRangeX.x, _spawnRangeX.y);
			Vector3 spawnPosition = new Vector3(spawnX, transform.position.y, transform.position.z);

			// Random rock prefab
			GameObject selectedRock = _rockPrefabs[Random.Range(0, _rockPrefabs.Count)];

			// Instantiate the rock
			Instantiate(selectedRock, spawnPosition, Quaternion.identity, _rockParent);
		}

		void OnDrawGizmos()
		{
			// Draw a line across the spawn range
			Gizmos.color = Color.red;
			Vector3 start = new Vector3(_spawnRangeX.x, transform.position.y, transform.position.z);
			Vector3 end = new Vector3(_spawnRangeX.y, transform.position.y, transform.position.z);
			Gizmos.DrawLine(start, end);

			// Draw small spheres at the endpoints
			Gizmos.DrawSphere(start, 0.2f);
			Gizmos.DrawSphere(end, 0.2f);
		}
	}
}


