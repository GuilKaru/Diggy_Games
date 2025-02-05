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

		[SerializeField]
		private GameObject _explosionParent;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _currentState;
		private string _idleAnim = "Explosion_Idle";

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
			_animator= GetComponent<Animator>();
			ChangeAnimationState(_idleAnim);
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


		public void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}

		private IEnumerator DestroyAfterDuration(float duration)
		{
			yield return new WaitForSeconds(duration);
			Destroy(_explosionParent);
			Destroy(gameObject);
		}
	}
}


