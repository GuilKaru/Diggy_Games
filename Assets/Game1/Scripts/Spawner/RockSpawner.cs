using UnityEngine;
using System.Collections.Generic;
using Moq;
using System.Linq;

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

		// Spawns a rock at a random available position.
		
		public void SpawnRock(int numberOfRocks)
		{
			// Get a list of available positions
			List<int> availablePositions = GetAvailablePositions();

			if (availablePositions.Count < numberOfRocks)
			{
				Debug.LogWarning($"Not enough available positions to spawn {numberOfRocks} rocks.");
				return;
			}

			// Shuffle available positions to pick randomly
			System.Random random = new System.Random();
			availablePositions = availablePositions.OrderBy(x => random.Next()).ToList();

			for (int i = 0; i < numberOfRocks; i++)
			{
				int positionIndex = availablePositions[i];
				Transform spawnPosition = _spawnPositions[positionIndex];

				// Spawn the rock
				GameObject rock = Instantiate(_rockPrefab, spawnPosition.position, Quaternion.identity, _rockParent);
				Debug.Log($"Rock spawned at position {positionIndex}.");

				// Mark the position as used
				_usedPositions.Add(positionIndex);

				// Initialize the rock with health and lifetime
				Rock rockScript = rock.GetComponent<Rock>();
				if (rockScript != null)
				{
					rockScript.Initialize(_rockHealth, _rockLifetime, () =>
					{
						// Callback when the rock is destroyed
						_usedPositions.Remove(positionIndex); // Free up the position
						Debug.Log($"Rock at position {positionIndex} destroyed.");
					});
				}
			}
		}

		#endregion

		#region Private Methods
		// Returns a list of indices of available spawn positions
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

