using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class LavaDropSpawner : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields
		[Header("Lava Drop Configuration")]
		[SerializeField]
		private GameObject[] _lavaDropPrefabs; // Array of prefabs to spawn

		[SerializeField]
		private Transform _lavaParent; // Parent object for spawned drops

		[SerializeField]
		private Vector2 _spawnXRange = new Vector2(-7f, 7f); // X range for spawning

		[SerializeField]
		private float _spawnYPosition = 5f; // Y position for spawning (top of screen)

		[SerializeField]
		private float _spawnInterval = 1f; // Time between spawns

		[SerializeField]
		private float _defaultZRotation = 180f;
		#endregion

		// Private Variables
		#region Private Variables
		private bool _isSpawning = false; // Control whether spawning is active
		private int _unlockedLavaIndex = 0; // Tracks the unlocked prefabs
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Spawning is disabled by default; controlled externally
			_isSpawning = false;
		}
		#endregion

		// Public Methods
		#region Public Methods
		public void StartSpawning()
		{
			if (!_isSpawning)
			{
				_isSpawning = true;
				InvokeRepeating(nameof(SpawnLavaDrop), 0f, _spawnInterval);
			}
		}

		public void StopSpawning()
		{
			_isSpawning = false;
			CancelInvoke(nameof(SpawnLavaDrop));
		}

		public void UnlockNewLavaDrop(int index)
		{
			// Ensure the index is within bounds of the array
			if (index < _lavaDropPrefabs.Length)
			{
				_unlockedLavaIndex = index;
			}
		}
		#endregion

		// Spawning Logic
		#region Spawning Logic
		private void SpawnLavaDrop()
		{
			if (_lavaDropPrefabs == null || _lavaDropPrefabs.Length == 0) return;

			// Pick a random prefab from the unlocked pool
			int randomIndex = Random.Range(0, _unlockedLavaIndex + 1);
			GameObject lavaDrop = _lavaDropPrefabs[randomIndex];

			// Generate a random position within the X range
			float randomX = Random.Range(_spawnXRange.x, _spawnXRange.y);
			Vector3 spawnPosition = new Vector3(randomX, _spawnYPosition, 0f);
			// Set the rotation for the prefab
			Quaternion spawnRotation = Quaternion.Euler(0f, 0f, _defaultZRotation);
			// Spawn the lava drop
			Instantiate(lavaDrop, spawnPosition, spawnRotation, _lavaParent);
		}
		#endregion

		// Gizmos for Debugging
		#region Gizmos
		private void OnDrawGizmos()
		{
			// Visualize the spawn area
			Gizmos.color = Color.red;

			// Draw the spawn range at the fixed Y position
			Vector3 start = new Vector3(_spawnXRange.x, _spawnYPosition, 0f);
			Vector3 end = new Vector3(_spawnXRange.y, _spawnYPosition, 0f);
			Gizmos.DrawLine(start, end);
		}
		#endregion
	}
}

