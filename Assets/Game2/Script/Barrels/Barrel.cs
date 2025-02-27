using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class Barrel : MonoBehaviour
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
					DestroyCoins(playerController);
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
	}
}
