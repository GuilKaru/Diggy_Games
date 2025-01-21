using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class LavaDrop : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields
		[Header("Movement Settings")]
		[SerializeField]
		private float _fallSpeed = 5f; // Speed at which the lava drop falls

		[SerializeField]
		private float _destroyYPosition = -5f; // Position at which the lava drop is destroyed

		#endregion

		// Cached Components
		#region Cached Components
		private Rigidbody2D _rigidbody;
		private PlayerHealth _playerHealth;
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Cache Rigidbody2D (optional, in case physics is needed)
			_rigidbody = GetComponent<Rigidbody2D>();
			_playerHealth = FindObjectOfType<PlayerHealth>();
		}
		#endregion

		// Update Loop
		#region Update Loop
		private void Update()
		{
			// Move the lava drop downward
			transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime, Space.World);

			// Destroy the lava drop if it goes below the destroy position
			if (transform.position.y <= _destroyYPosition)
			{
				Destroy(gameObject);
			}
		}
		#endregion

		// Collision Logic
		#region Collision Logic
		private void OnTriggerEnter2D(Collider2D other)
		{
			// Check if the lava drop collided with the player
			if (other.CompareTag("Player"))
			{
				_playerHealth.Damage(1);
			
				// Destroy the lava drop
				Destroy(gameObject);
			}
		}
		#endregion
	}
}
