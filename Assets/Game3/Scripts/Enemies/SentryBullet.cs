using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class SentryBullet : MonoBehaviour
	{
		[SerializeField] private float _speed = 5f;
		[SerializeField] private float _destroyY = -4f;
		[SerializeField] private GameObject _damageAreaPrefab;

		private Vector2 _direction = Vector2.down;

		public void SetDirection(Vector2 direction)
		{
			_direction = direction.normalized;
		}

		private void Update()
		{
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
				Instantiate(_damageAreaPrefab, transform.position, Quaternion.identity);
			}
		}
	}

}
