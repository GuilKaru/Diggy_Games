using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class TransportBomb : MonoBehaviour
	{
		[Header("Barrel Settings")]
		[SerializeField]
		private float _moveSpeed = 3f;
		[SerializeField]
		private bool _moveRight = true; // True = move right, false = move left
		[SerializeField]
		private float _destroyXPositionLeft = -10f; // Position where the barrel will be destroyed if moving left
		[SerializeField]
		private float _destroyXPositionRight = 10f; // Position where the barrel will be destroyed if moving right

		[Header("Explosion Settings")]
		[SerializeField]
		private float explosionRadius = 2f; // Radius of the explosion
		[SerializeField]
		private float _stunDuration = 2f; // Duration for which the player is stunned
		[SerializeField]
		private LayerMask playerLayer; // LayerMask to detect the player

		private PlayerController _playerController;
		private bool hasExploded = false;
		private float _originalSpeed;


		private void Start()
		{
			_playerController = FindObjectOfType<PlayerController>();
			_originalSpeed = _moveSpeed;
		}

		private void Update()
		{
			MoveBarrel();
			CheckPositionAndDestroy();
		}

		private void MoveBarrel()
		{
			float direction = _moveRight ? 1f : -1f;
			transform.Translate(Vector2.right * direction * _moveSpeed * Time.deltaTime);
		}

		#region Speed Control
		public void SetSpeed(float newSpeed)
		{
			_moveSpeed = newSpeed;
		}

		// Method to restore speed to its original value
		public void RestoreSpeed()
		{
			_moveSpeed = _originalSpeed; // Restore the speed
		}
		#endregion

		private void CheckPositionAndDestroy()
		{
			// Check if the barrel reaches the destruction position on the X-axis
			if ((_moveRight && transform.position.x >= _destroyXPositionRight) ||
				(!_moveRight && transform.position.x <= _destroyXPositionLeft))
			{
				Destroy(gameObject); // Destroy the barrel if it reaches the destruction position
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				// Destroy coins carried by the player when they collide with the barrel
				PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
				if (playerController != null)
				{
					ExplodeAndStun(other.gameObject);
					DestroyCoins(playerController);
					playerController.PlayerTakeDamage();
				}
			}

			if (other.CompareTag("Rock"))
			{

				Destroy(gameObject);
			}


			if (other.CompareTag("TransportLine"))
			{
				TransportLine transportLine = other.GetComponent<TransportLine>();
				if (transportLine != null)
				{
					// Only set direction on the TransportLine this barrel touches
					transportLine.SetDriftDirection(_moveRight ? TransportLine.DriftDirection.Right : TransportLine.DriftDirection.Left);
				}
			}
		}

		private void DestroyCoins(PlayerController playerController)
		{
			// Destroy coins if the player is carrying any
			foreach (GameObject carriedObject in playerController.GetCarriedObjects()) // Use the GetCarriedObjects method
			{
				if (carriedObject.CompareTag("PickUp")) // Assuming coins have a "PickUp" tag
				{
					Destroy(carriedObject);
				}
			}
		}

		// Explosion and Stun Logic
		#region Explosion and Stun

		private void ExplodeAndStun(GameObject player)
		{
			hasExploded = true;


			// Check if the player is within the explosion radius
			Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);

			foreach (Collider2D obj in hitObjects)
			{
				if (obj.CompareTag("Player"))
				{
					PlayerController playerController = obj.GetComponent<PlayerController>();
					if (playerController != null)
					{
						playerController.StunPlayer(_stunDuration);
					}
				}
			}

			// Destroy the TNT object
			DestroyTransport();
		}

		private void OnDrawGizmosSelected()
		{
			// Visualize explosion radius in Scene view
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, explosionRadius);
		}
		#endregion


		private void DestroyTransport()
		{
			Destroy(gameObject);
		}

		

	}
}

