using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class SpawnerManager : MonoBehaviour
	{
		[Header("Spawner References")]
		[SerializeField]
		private BarrelSpawner _leftSpawner;
		[SerializeField]
		private BarrelSpawner _rightSpawner;

		[Header("Y Position Settings")]
		[SerializeField]
		private List<float> _spawnPositionsY = new List<float>(); // List of Y positions where barrels can be spawned

		private HashSet<float> _occupiedPositionsY = new HashSet<float>(); // To keep track of occupied Y positions

		private void Start()
		{
			if (_leftSpawner != null) _leftSpawner.SetSpawnerManager(this);
			if (_rightSpawner != null) _rightSpawner.SetSpawnerManager(this);
		}

		public bool CanSpawnAtPosition(float yPos)
		{
			return !_occupiedPositionsY.Contains(yPos);
		}

		public void OccupyPosition(float yPos)
		{
			_occupiedPositionsY.Add(yPos);
		}

		public void ReleasePosition(float yPos)
		{
			if (_occupiedPositionsY.Contains(yPos))
			{
				Debug.Log($"Released Y Position: {yPos}");
			}
			_occupiedPositionsY.Remove(yPos);
		}

		public float GetAvailablePosition()
		{
			// Create a shuffled list to randomize the spawn order
			List<float> shuffledPositions = new List<float>(_spawnPositionsY);
			shuffledPositions = shuffledPositions.OrderBy(x => Random.value).ToList();

			foreach (var yPos in shuffledPositions)
			{
				if (CanSpawnAtPosition(yPos))
				{
					return yPos; // Return the first available random Y position
				}
			}
			return -2; // If no available positions
		}
	}
}


