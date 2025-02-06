using UnityEngine;
using System.Collections;
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

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _currentState;
		private string _destroyLavaDropAnim = "LavaDrop_Destroy";

		#endregion

		// Cached Components
		#region Cached Components
		private Rigidbody2D _rigidbody;
		private Collider2D _collider;
		private PlayerHealth _playerHealth;
		private bool _isDestroying = false;
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Cache Rigidbody2D (optional, in case physics is needed)
			_rigidbody = GetComponent<Rigidbody2D>();
			_collider = GetComponent<Collider2D>(); 
			_playerHealth = FindObjectOfType<PlayerHealth>();
		}
		#endregion

		// Movement
		#region Movement
		private void Update()
		{

			if (!_isDestroying)
			{
				transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime, Space.World);
			}

			// Check if the lava drop reaches the destroy position
			if (transform.position.y <= _destroyYPosition && !_isDestroying)
			{
				StartDestroySequence();
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



		// Destruction Sequence
		#region Destruction Sequence
		private void StartDestroySequence()
		{
			_isDestroying = true;

			_collider.enabled = false; // Disable collider to prevent further interactions
			ChangeAnimationState(_destroyLavaDropAnim); // Trigger destruction animation

			// Wait for the animation to finish before destroying
			StartCoroutine(DestroyAfterAnimation());
		}

		private IEnumerator DestroyAfterAnimation()
		{
			// Get the animation length
			float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(animationLength);

			Destroy(gameObject);
		}
		#endregion

		//Animation
		#region Animation
		public void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}
		#endregion
	}
}
