using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class EnemySeeker : MonoBehaviour
	{
		[Header("Move Settings")]
		[SerializeField]
		private float _delayBeforeLaunch = 1.0f;
		[SerializeField]
		private float _moveSpeed = 4f;
		[SerializeField]
		private float _destroyY = -4f;
		[SerializeField]
		private string _playerTag = "Player";

		[Header("Score Settings")]
		[SerializeField]
		private int _scoreValue = 10;

		private Vector2 _targetPosition;
		private bool _isLaunched = false;
		private float _timer;
		private ScoreManager _scoreManager;

		private void Start()
		{
			_timer = _delayBeforeLaunch;
			_scoreManager = FindObjectOfType<ScoreManager>();
			GameObject player = GameObject.FindGameObjectWithTag(_playerTag);
			if (player != null)
			{
				_targetPosition = player.transform.position;
			}
			else
			{
				// Fallback to downward movement if no player found
				_targetPosition = transform.position + Vector3.down * 10f;
			}
		}

		private void Update()
		{
			if (!_isLaunched)
			{
				_timer -= Time.deltaTime;
				if (_timer <= 0f)
				{
					_isLaunched = true;
				}
			}
			else
			{
				MoveToTarget();
			}

			if (transform.position.y < _destroyY)
			{
				Destroy(gameObject);
			}
		}

		private void MoveToTarget()
		{
			transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

			// Optionally destroy on arrival (good for precision hits)
			if (Vector2.Distance(transform.position, _targetPosition) < 0.1f)
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				// Destroy coins carried by the player when they collide with the barrel
				PlayerHealth _playerHealth = other.gameObject.GetComponent<PlayerHealth>();
				_playerHealth.Damage(1);
			}

			if (other.CompareTag("Shovel"))
			{
				_scoreManager.AddScore(_scoreValue);
				Destroy(gameObject);
			}
		}
	}

}
