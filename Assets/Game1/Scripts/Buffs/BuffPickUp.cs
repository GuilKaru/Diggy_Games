using System.Collections;
using UnityEngine;

namespace Diggy_MiniGame_1
{
	public class BuffPickUp : MonoBehaviour
	{
		[SerializeField]
		private float _moveSpeed = 2f; // Speed at which the pickup moves
		[SerializeField]
		private float _destroyPositionX = -10f; // Position to destroy the pickup
		[SerializeField]
		private int _scoreValue = 10;

		[Header("Barrel Components Settings")]
		[SerializeField]
		SpriteRenderer _spriteRenderer;
		[SerializeField]
		Collider2D _collider;

		[Header("Barrel Audio")]
		[SerializeField]
		private AudioSource _buffAudioSource;
		[SerializeField]
		private AudioClip[] _buffClips;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _currentState;
		private string _idleAnim = "BuffPickUp_Idle";
		private ScoreManager _scoreManager;

		private void Start()
		{
			// Find the BuffManager in the scene dynamically
			_scoreManager = FindObjectOfType<ScoreManager>();
			_buffAudioSource = GetComponent<AudioSource>();
			_collider = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
			ChangeAnimationState(_idleAnim);
		}

		private void Update()
		{
			// Move the pickup to the left
			transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);

			// Destroy the pickup if it goes beyond the boundary
			if (transform.position.x <= _destroyPositionX)
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			// Check if the player collects the pickup
			if (other.CompareTag("Player"))
			{
				StartCoroutine(DestroyBuff());
				PlayAudioBarrelBuffClip(0);
				_scoreManager.AddScore(_scoreValue);
			}
		}

		private IEnumerator DestroyBuff()
		{
			
			_spriteRenderer.enabled = false;
			_collider.enabled = false;

			yield return new WaitForSeconds(1);
			Destroy(gameObject);
		}


		private void PlayAudioBarrelBuffClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _buffClips.Length)
			{
				_buffAudioSource.clip = _buffClips[clipIndex];
				_buffAudioSource.Play();
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
	}
}

