using System.Collections.Generic;
using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class DifficultyManager : MonoBehaviour
	{
		[SerializeField]
		private ScoreManager _scoreManager;
		[SerializeField]
		private EnemySpawner _enemySpawner;
		[SerializeField]
		private TurretSpawner _turretSpawner;

		[SerializeField]
		private float _spawnInterval = 2f;
		[SerializeField]
		private int _maxEnemiesOnScreen = 1;

		private float _spawnTimer = 0f;
		private List<GameObject> _activeEnemies = new List<GameObject>();

		private void Update()
		{
			UpdateDifficulty();

			_spawnTimer += Time.deltaTime;
			if (_spawnTimer >= _spawnInterval && _activeEnemies.Count < _maxEnemiesOnScreen)
			{
				TrySpawn();
				_spawnTimer = 0f;
			}
			if (_turretSpawner.gameObject.activeSelf && !_turretSpawner.HasActiveTurret())
			{
				_turretSpawner.SpawnTurret();
			}
			CleanupDestroyedEnemies();
		}

		private void UpdateDifficulty()
		{
			int score = _scoreManager.GetScore();

			if (score < 20)
			{
				SetSpawnerConfig(new[] { EnemyType.EnemyA }, 1, turretActive: false);
			}
			else if (score < 50)
			{
				SetSpawnerConfig(new[] { EnemyType.EnemyB }, 1, turretActive: false);
			}
			else if (score < 70)
			{
				SetSpawnerConfig(new[] { EnemyType.EnemyA, EnemyType.EnemyB }, 2, turretActive: false);
			}
			else if (score < 100)
			{
				SetSpawnerConfig(new[] { EnemyType.EnemyA, EnemyType.EnemyB }, 2, turretActive: true);
			}
			else
			{
				SetSpawnerConfig(new[] { EnemyType.EnemyA, EnemyType.EnemyB }, 3, turretActive: true);
			}
		}

		private void SetSpawnerConfig(EnemyType[] allowedEnemies, int maxEnemies, bool turretActive)
		{
			_enemySpawner.spawnableEnemies = allowedEnemies;
			_maxEnemiesOnScreen = maxEnemies;

			_turretSpawner.gameObject.SetActive(turretActive);
		}

		private void TrySpawn()
		{
			GameObject spawned = _enemySpawner.SpawnRandomWithReturn();
			if (spawned != null)
			_activeEnemies.Add(spawned);
		}

		private void CleanupDestroyedEnemies()
		{
			_activeEnemies.RemoveAll(e => e == null);
		}
	}
}

