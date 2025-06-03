using System.Collections;
using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class EnemySentry : MonoBehaviour
	{
		[Header("Enemy Sentry Settings")]
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

		[SerializeField]
		private int _scoreValue = 10;

		private Transform _player;
		private float _fireTimer;
		private ScoreManager _scoreManager;
		private bool hasFired = false;

		private void Start()
		{

			_scoreManager = FindObjectOfType<ScoreManager>();
			_player = GameObject.FindGameObjectWithTag("Player").transform;
		}

		private void Update()
		{
			_fireTimer += Time.deltaTime;

			if (!hasFired && _player != null && _fireTimer >= fireCooldown && IsPlayerInCone())
			{
				FireAtPlayer();
				hasFired = true;
				
			}
		}

		private bool IsPlayerInCone()
		{
			Vector2 toPlayer = _player.position - transform.position;
			float distance = toPlayer.magnitude;

			if (distance > detectionRange)
				return false;

			float angle = Vector2.Angle(transform.right, toPlayer.normalized);
			return angle < detectionAngle;
		}

		private void FireAtPlayer()
		{
			GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
			Vector2 direction = (_player.position - firePoint.position).normalized;

			// Find bullet parent in the scene
			GameObject bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
			if (bulletParent != null)
			{
				bullet.transform.SetParent(bulletParent.transform);
			}


			SentryBullet sentryBullet = bullet.GetComponent<SentryBullet>();
			if (sentryBullet != null)
			{
				sentryBullet.SetDirection(direction);
			}
			StartCoroutine(SelfDestructAfterDelay(3f));
		}

		private void OnTriggerEnter2D(Collider2D other)
		{

			if (other.CompareTag("Shovel"))
			{
				_scoreManager.AddScore(_scoreValue);
				Destroy(gameObject);
			}
		}

		private IEnumerator SelfDestructAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			_scoreManager.AddScore(_scoreValue);
			Destroy(gameObject);
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

