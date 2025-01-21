using UnityEngine;
using System.Collections;
namespace Diggy_MiniGame_1
{
	public class Explosion : MonoBehaviour
	{
		[Header("Explosion Settings")]
		[SerializeField]
		private float explosionRadius = 2f; // Radius of the explosion
		[SerializeField]
		private float stunDuration = 2f; // Duration for which the player is stunned
		[SerializeField]
		private LayerMask playerLayer; // LayerMask to detect the player

		[Header("Stun Visual Effect Duration")]
		[SerializeField]
		private float _stunVisualEffectDuration = 2f;

		// Private Variables
		#region Private Variables
		private Rigidbody2D _rigidbody;
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Cache Rigidbody2D (optional, in case physics is needed)
			_rigidbody = GetComponent<Rigidbody2D>();
			_playerHealth = FindObjectOfType<PlayerHealth>();
			_playerController = FindObjectOfType<PlayerController>();

			StartCoroutine(DestroyAfterDuration(_stunVisualEffectDuration));
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
				_playerController.StunPlayer(2f);
			}
		}
		#endregion

		

		private IEnumerator DestroyAfterDuration(float duration)
		{
			yield return new WaitForSeconds(duration);

			Destroy(gameObject);
		}
	}
}


