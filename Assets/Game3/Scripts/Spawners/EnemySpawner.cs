using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class EnemySpawner : MonoBehaviour
	{
		public EnemyType[] spawnableEnemies;
		public GameObject[] enemyPrefabs;

		public Transform _parentTransform;

		public Vector2 spawnAreaSize = new Vector2(5, 5);


		public GameObject SpawnRandomWithReturn()
		{

			if (spawnableEnemies.Length == 0 || enemyPrefabs.Length == 0)
				return null;

			int index = Random.Range(0, spawnableEnemies.Length);
			Vector2 spawnPos = GetRandomPosition();
			return Instantiate(enemyPrefabs[index], spawnPos, Quaternion.identity, _parentTransform);
		}

		private Vector2 GetRandomPosition()
		{
			Vector2 center = transform.position;
			float x = Random.Range(center.x - spawnAreaSize.x / 2f, center.x + spawnAreaSize.x / 2f);
			float y = Random.Range(center.y - spawnAreaSize.y / 2f, center.y + spawnAreaSize.y / 2f);
			return new Vector2(x, y);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Vector3 center = transform.position;
			Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f);
			Gizmos.DrawWireCube(center, size);
		}
	}

}
