using UnityEngine;
using System.Collections;
namespace Diggy_MiniGame_1
{
	public class Fire : MonoBehaviour
	{
		[Header("Slow Effect Settings")]
		[SerializeField]
		private float _slowDuration = 3f; // Duration of the slow effect
		[SerializeField]
		private float _slowAmount = 0.5f; // Percentage of speed reduction

		[Header("Fire Duration")]
		[SerializeField]
		private float _fireEffectDuration = 2f;

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

			StartCoroutine(DestroyAfterDuration(_fireEffectDuration));
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
				_playerController.ApplySlowEffect(_slowAmount, _slowDuration);
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


