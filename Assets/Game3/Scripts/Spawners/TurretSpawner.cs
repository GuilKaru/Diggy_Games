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

		private GameObject currentTurret;

		public void SpawnTurret()
		{
			if (currentTurret != null) return; // A turret already exists

			bool spawnLeft = Random.value > 0.5f;
			Transform spawnPoint = spawnLeft ? leftSpawnPoint : rightSpawnPoint;
			GameObject turretPrefab = spawnLeft ? turretLeftPrefab : turretRightPrefab;

			currentTurret = Instantiate(turretPrefab, spawnPoint.position, turretPrefab.transform.rotation, _parentTransform);

		}
		public bool HasActiveTurret()
		{
			return currentTurret != null;
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

