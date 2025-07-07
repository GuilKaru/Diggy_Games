using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class EnemyBullet : MonoBehaviour
	{
		[SerializeField] private float _speed = 5f;
		[SerializeField] private float _destroyY = -4f;

		private void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;

			MoveDown();
			CheckOutOfBounds();
		}

		private void MoveDown()
		{
			transform.Translate(Vector2.down * _speed * Time.deltaTime);
		}

		private void CheckOutOfBounds()
		{
			if (transform.position.y < _destroyY)
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
				if (playerHealth != null)
				{
					playerHealth.Damage(1);
				}
				Destroy(gameObject);
			}
		}
	}
}


