using UnityEngine;
using System.Collections.Generic;
using Moq;

namespace Diggy_MiniGame_1
{
	public class RockSpawner : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField]
		private GameObject _rockPrefab; // The rock prefab to spawn
		[SerializeField]
		private Transform _rockParent;
		[SerializeField]
		private List<Transform> _spawnPositions = new List<Transform>(); // List of spawn positions
		[SerializeField]
		private float _rockLifetime = 10f; // Time before the rock is destroyed (if not destroyed by hits)
		[SerializeField]
		private int _rockHealth = 3; // Health of the rock (number of hits to destroy)

		#endregion

		#region Private Variables

		private HashSet<int> _usedPositions = new HashSet<int>(); // Tracks which spawn positions have been used

		#endregion

		#region Public Methods

		/// <summary>
		/// Spawns a rock at a random available position.
		/// </summary>
		public void SpawnRock()
		{
			// Get a list of available positions
			List<int> availablePositions = GetAvailablePositions();
			if (availablePositions.Count == 0)
			{
				Debug.Log("No available positions to spawn a rock.");
				return;
			}

			// Choose a random position from the available list
			int randomIndex = availablePositions[Random.Range(0, availablePositions.Count)];
			Transform spawnPosition = _spawnPositions[randomIndex];

			// Spawn the rock
			GameObject rock = Instantiate(_rockPrefab, spawnPosition.position, Quaternion.identity, _rockParent);
			Debug.Log($"Rock spawned at position {randomIndex}.");

			// Mark the position as used
			_usedPositions.Add(randomIndex);

			// Initialize the rock with health and lifetime
			Rock rockScript = rock.GetComponent<Rock>();
			if (rockScript != null)
			{
				rockScript.Initialize(_rockHealth, _rockLifetime, () =>
				{
					// Callback when the rock is destroyed
					_usedPositions.Remove(randomIndex); // Free up the position
					Debug.Log($"Rock at position {randomIndex} destroyed.");
				});
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns a list of indices of available spawn positions.
		/// </summary>
		private List<int> GetAvailablePositions()
		{
			List<int> availablePositions = new List<int>();
			for (int i = 0; i < _spawnPositions.Count; i++)
			{
				if (!_usedPositions.Contains(i))
				{
					availablePositions.Add(i);
				}
			}
			return availablePositions;
		}

		#endregion
	}
}

