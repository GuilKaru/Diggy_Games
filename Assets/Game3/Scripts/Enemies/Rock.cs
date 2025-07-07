using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class Rock : MonoBehaviour
	{
		[Header("Rock Settings")]
		public float _fallSpeed = 5f;
		public float _destroyYThreshold = -10f;

		void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;

			// Move the rock downward
			transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime);

			// Destroy if below threshold
			if (transform.position.y <= _destroyYThreshold)
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


