using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class Rock : MonoBehaviour
	{

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private AudioSource _rockAudioSource;
		[SerializeField]
		private AudioClip[] _rockBuffClips;

		private string _currentState;
		private string _rockHitAnim = "Rock_Hit";

		#region Private Variables
		private int _health;
		private float _lifetime;
		private System.Action _onDestroyedCallback;
		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the rock with health and lifetime.
		/// </summary>
		public void Initialize(int health, float lifetime, System.Action onDestroyedCallback)
		{
			_health = health;
			_lifetime = lifetime;
			_onDestroyedCallback = onDestroyedCallback;

			// Start the lifetime countdown
			Invoke(nameof(DestroyRock), _lifetime);
		}

		/// <summary>
		/// Called when the rock is hit.
		/// </summary>
		public void RockTakeDamage()
		{
			_health--;
			PlayAudioRockBuffClip(0);
			Debug.Log($"Rock took damage. Remaining health: {_health}");
			ChangeAnimationState(_rockHitAnim);
			if (_health <= 0)
			{
				DestroyRock();
			}
		}

		#endregion

		#region Private Methods

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("Barrel"))
			{
				RockTakeDamage();
			}
		}

		/// <summary>
		/// Destroys the rock and triggers the callback.
		/// </summary>
		private void DestroyRock()
		{
			Destroy(gameObject);
		}

		private void PlayAudioRockBuffClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _rockBuffClips.Length)
			{
				_rockAudioSource.clip = _rockBuffClips[clipIndex];
				_rockAudioSource.Play();
			}
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
		#endregion
	}
}

