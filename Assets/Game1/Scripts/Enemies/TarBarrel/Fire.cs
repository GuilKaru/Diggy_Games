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

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private GameObject _fireParent;

		// Private Variables
		#region Private Variables
		private Rigidbody2D _rigidbody;
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		private string _currentState;
		private string _idleAnim = "Fire_Idle";

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
			ChangeAnimationState(_idleAnim);
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
				_playerController.PlayAudioPlayerHitLavaClip(0);
			}
		}
		#endregion

		private IEnumerator DestroyAfterDuration(float duration)
		{
			yield return new WaitForSeconds(duration);

			Destroy(_fireParent);
			Destroy(gameObject);
		}

		public void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}
	}
}


