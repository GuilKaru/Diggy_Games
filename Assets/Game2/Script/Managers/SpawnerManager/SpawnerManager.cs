using System.Collections.Generic;
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
			_occupiedPositionsY.Remove(yPos);
		}

		public float GetAvailablePosition()
		{
			foreach (var yPos in _spawnPositionsY)
			{
				if (CanSpawnAtPosition(yPos))
				{
					return yPos;
				}
			}
			return -1;
		}
	}
}


