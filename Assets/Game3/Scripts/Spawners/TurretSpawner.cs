using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class TurretSpawner : MonoBehaviour
	{
		public Transform leftSpawnPoint;
		public Transform rightSpawnPoint;

		public GameObject turretLeftPrefab;
		public GameObject turretRightPrefab;

		public Transform _parentTransform;

		public void SpawnTurret()
		{
			// Randomly choose left or right (never both)
			bool spawnLeft = Random.value > 0.5f;

			if (spawnLeft)
			{
				Instantiate(turretLeftPrefab, leftSpawnPoint.position, turretLeftPrefab.transform.rotation, _parentTransform);
			}
			else
			{
				Instantiate(turretRightPrefab, rightSpawnPoint.position, turretRightPrefab.transform.rotation, _parentTransform);
			}
		}

		private void OnDrawGizmos()
		{
		if (leftSpawnPoint != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(leftSpawnPoint.position, 0.2f);
		}

		if (rightSpawnPoint != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(rightSpawnPoint.position, 0.2f);
			}
		}
	}
}

