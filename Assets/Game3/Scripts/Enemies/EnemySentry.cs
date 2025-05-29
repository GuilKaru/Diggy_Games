using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class EnemySentry : MonoBehaviour
	{
		[SerializeField]
		private float detectionAngle = 45f; // Cone half-angle
		[SerializeField]
		private float detectionRange = 10f;
		[SerializeField]
		private float fireCooldown = 2f;
		[SerializeField]
		private GameObject bulletPrefab;
		[SerializeField]
		private Transform firePoint;

		private Transform player;
		private float fireTimer;

		private void Start()
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}

		private void Update()
		{
			fireTimer += Time.deltaTime;

			if (player != null && fireTimer >= fireCooldown && IsPlayerInCone())
			{
				FireAtPlayer();
				fireTimer = 0f;
			}
		}

		private bool IsPlayerInCone()
		{
			Vector2 toPlayer = player.position - transform.position;
			float distance = toPlayer.magnitude;

			if (distance > detectionRange)
				return false;

			float angle = Vector2.Angle(transform.right, toPlayer.normalized);
			return angle < detectionAngle;
		}

		private void FireAtPlayer()
		{
			GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
			Vector2 direction = (player.position - firePoint.position).normalized;

			SentryBullet sentryBullet = bullet.GetComponent<SentryBullet>();
			if (sentryBullet != null)
			{
				sentryBullet.SetDirection(direction);
			}
		}

		private void OnDrawGizmos()
		{
			// Draw detection range
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, detectionRange);

			// Draw detection cone
			Vector2 direction1 = Quaternion.Euler(0, 0, detectionAngle) * transform.right;
			Vector2 direction2 = Quaternion.Euler(0, 0, -detectionAngle) * transform.right;

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, direction1 * detectionRange);
			Gizmos.DrawRay(transform.position, direction2 * detectionRange);
		}
	}
}

