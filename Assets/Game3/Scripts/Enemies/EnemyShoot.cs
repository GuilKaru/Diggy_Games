using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class EnemyShoot : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Movement Settings")]
		[SerializeField]
		private float _moveSpeed = 2f;
		[SerializeField]
		private float _changeDirectionInterval = 2f;
		[SerializeField]
		private float _leftBoundary = -8f;
		[SerializeField]
		private float _rightBoundary = 8f;

		[Header("Shooting Settings")]
		[SerializeField]
		private GameObject _bulletPrefab;
		[SerializeField]
		private Transform _firePoint;
		[SerializeField]
		private float _shootInterval = 3f;

		[Header("Score Settings")]
		[SerializeField]
		private int _scoreValue = 10;
		#endregion

		//Private Variables
		#region Private Variables

		private ScoreManager _scoreManager;
		private float _moveDirection = 1f;
		private float _moveTimer;
		private float _shootTimer;
		#endregion

		//Initialization
		#region Initialization

		private void Start()
		{
			_scoreManager = FindObjectOfType<ScoreManager>();
			_moveTimer = _changeDirectionInterval;
			_shootTimer = _shootInterval;
			_moveDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
		}

		private void Update()
		{
			HandleMovement();
			HandleShooting();
		}

		#endregion

		//Movement
		#region Movement

		private void HandleMovement()
		{
			// Move
			transform.Translate(Vector2.right * _moveDirection * _moveSpeed * Time.deltaTime);

			// Change direction at interval
			_moveTimer -= Time.deltaTime;
			if (_moveTimer <= 0f)
			{
				_moveDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
				_moveTimer = _changeDirectionInterval;
			}

			// Clamp within boundaries
			if (transform.position.x < _leftBoundary)
			{
				transform.position = new Vector2(_leftBoundary, transform.position.y);
				_moveDirection = 1f;
			}
			else if (transform.position.x > _rightBoundary)
			{
				transform.position = new Vector2(_rightBoundary, transform.position.y);
				_moveDirection = -1f;
			}
		}

		#endregion

		//Shooting
		#region Shooting

		private void HandleShooting()
		{
			_shootTimer -= Time.deltaTime;
			if (_shootTimer <= 0f)
			{
				Shoot();
				_shootTimer = _shootInterval;
			}
		}

		private void Shoot()
		{
			if (_bulletPrefab != null && _firePoint != null)
			{
				GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);

				// Find bullet parent in the scene
				GameObject bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
				if (bulletParent != null)
				{
					bullet.transform.SetParent(bulletParent.transform);
				}

			}
		}

		#endregion

		//Trigger 
		#region Trigger

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Shovel"))
			{
				_scoreManager.AddScore(_scoreValue);
				Destroy(gameObject);
			}
		}
		#endregion
	}

}

