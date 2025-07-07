using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class SentryBullet : MonoBehaviour
	{
		[SerializeField] private float _speed = 5f;
		[SerializeField] private float _destroyY = -4f;
		[SerializeField] private GameObject _damageAreaPrefab;

		private Vector2 _direction = Vector2.down;
		private Transform _player;

		public void SetDirection(Vector2 direction)
		{
			_direction = direction.normalized;
		}

		private void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;

			transform.Translate(_direction * _speed * Time.deltaTime);

			CheckOutOfBounds();
		}

		private void CheckOutOfBounds()
		{
			if (transform.position.y < _destroyY)
			{
				SpawnDamageArea();
				Destroy(gameObject);
			}
		}

		private void SpawnDamageArea()
		{
			if (_damageAreaPrefab != null)
			{
				GameObject bullet = Instantiate(_damageAreaPrefab, transform.position, Quaternion.identity);
				GameObject bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
				if (bulletParent != null)
				{
					bullet.transform.SetParent(bulletParent.transform);
				}
			}
		}
	}

}
