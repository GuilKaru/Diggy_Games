using Diggy_MiniGame_1;
using System.Collections.Generic;
using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class SpawnManager : MonoBehaviour
	{
		public List<EnemySpawner> areaSpawners;
		public TurretSpawner turretSpawner;

		[Header("Spawn Timers")]
		public float spawnInterval = 5f;
		private float timer;

		void Update()
		{
			timer += Time.deltaTime;
			if (timer >= spawnInterval)
			{
				timer = 0f;
				//SpawnEnemies();
			}
		}

		void SpawnEnemies()
		{
			// Spawn one random enemy from area spawners
			foreach (var spawner in areaSpawners)
			{
				spawner.SpawnRandomWithReturn();
			}

			// Spawn one turret (left or right, randomly)
			turretSpawner.SpawnTurret();
		}

	}
}

