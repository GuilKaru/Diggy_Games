using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Diggy_MiniGame_2
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
			if (_spawnPositions == null || _spawnPositions.Count == 0)
			{
				Debug.LogWarning("SpawnRock: No spawn positions assigned!");
				return;
			}

			List<int> availablePositions = GetAvailablePositions();

			if (availablePositions.Count == 0)
			{
				Debug.LogWarning("SpawnRock: No available spawn positions!");
				return;
			}

			if (availablePositions.Count < numberOfRocks)
			{
				Debug.LogWarning($"Not enough available positions to spawn {numberOfRocks} rocks.");
				return;
			}

			System.Random random = new System.Random();
			availablePositions = availablePositions.OrderBy(x => random.Next()).ToList();

			for (int i = 0; i < numberOfRocks; i++)
			{
				int positionIndex = availablePositions[i];
				Transform spawnPosition = _spawnPositions[positionIndex];

				if (_rockPrefab == null)
				{
					Debug.LogWarning("SpawnRock: Rock prefab is missing!");
					return;
				}

				GameObject rock = Instantiate(_rockPrefab, spawnPosition.position, Quaternion.identity, _rockParent);
				Debug.Log($"SpawnRock: Rock spawned at position {positionIndex} ({spawnPosition.position}).");

				_usedPositions.Add(positionIndex);

				SpriteRenderer sr = rock.GetComponent<SpriteRenderer>();
				if (sr != null)
				{
					sr.sortingOrder = 100 - (int)(spawnPosition.position.y * 10);
				}

				Rock rockScript = rock.GetComponent<Rock>();
				if (rockScript != null)
				{
					rockScript.Initialize(_rockHealth, _rockLifetime, () =>
					{
						_usedPositions.Remove(positionIndex);
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

