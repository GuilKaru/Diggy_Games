using UnityEngine;
using System.Collections;
namespace Diggy_MiniGame_1
{
	public class TarBarrel : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields
		[Header("Movement Settings")]
		[SerializeField]
		private float _speed = 2f;
		[SerializeField]
		private float _destroyXPosition = -10f;

		[Header("Score Settings")]
		[SerializeField]
		private int _scoreValue = 10;

		[Header("Slow Effect Settings")]
		[SerializeField]
		private float _slowDuration = 3f; // Duration of the slow effect
		[SerializeField]
		private float _slowAmount = 0.5f; // Percentage of speed reduction

		[Header("Visual Effect Settings")]
		[SerializeField]
		private GameObject _fireEffectPrefab; // Prefab for the fire/tar effect
		[SerializeField]
		private float _fireEffectDuration = 2f; // Duration before the fire effect disappears

		[Header("Barrel Components Settings")]
		[SerializeField]
		SpriteRenderer _spriteRenderer;
		[SerializeField]
		Collider2D _collider;

		[Header("Barrel Audio")]
		[SerializeField]
		private AudioSource _barrelAudioSource;
		[SerializeField]
		private AudioClip[] _barrelClips;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		#endregion

		// Private Variables
		#region Private Variables
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		private Rock _rock;
		private float _originalSpeed;
		private Transform _fireParent;
		private string _currentState;
		private string _idleAnim = "TarBarrel_Idle";
		#endregion

		// Initialization
		#region Initialization

		private void Start()
		{
			// Find necessary components in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();
			_playerHealth = FindObjectOfType<PlayerHealth>();
			_playerController = FindObjectOfType<PlayerController>();
			_rock = FindObjectOfType<Rock>();
			_barrelAudioSource = GetComponent<AudioSource>();
			_collider = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_originalSpeed = _speed;

			ChangeAnimationState(_idleAnim);
			if (_scoreManager == null)
			{
				Debug.LogError("ScoreManager not found in the scene. Ensure there is a GameObject with the ScoreManager script.");
			}


			if (_playerHealth == null)
			{
				Debug.LogError("PlayerHealth not found in the scene. Ensure there is a GameObject with the PlayerHealth script.");
			}
		}
		#endregion

		// Movement
		#region Movement

		private void Update()
		{
			MoveLeft();

			if (transform.position.x <= _destroyXPosition)
			{
				Destroy(gameObject);
			}
		}

		private void MoveLeft()
		{
			transform.Translate(Vector3.left * _speed * Time.deltaTime);
		}
		#endregion

		// Speed Modification Method (for external control)
		#region Speed Control
		public void SetSpeed(float newSpeed)
		{
			_speed = newSpeed;
		}

		// Method to restore speed to its original value
		public void RestoreSpeed()
		{
			_speed = _originalSpeed; // Restore the speed
		}
		#endregion

		// Collision
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				PlayerController player = collision.gameObject.GetComponent<PlayerController>();
				if (player != null)
				{
					player.ApplySlowEffect(_slowAmount, _slowDuration);
				}
				SpawnFireEffect();
				_playerController.PlayAudioPlayerHitLavaClip(0);
				_playerHealth.Damage(1);
				Destroy(gameObject); // Destroy the TarBarrel after applying the effect
			}

			if (collision.gameObject.CompareTag("AttackPoint"))
			{
				_scoreManager.AddScore(_scoreValue);
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Shield"))
			{
				PlayerController player = collision.gameObject.GetComponent<PlayerController>();
				if (player != null)
				{
					player.ApplySlowEffect(_slowAmount, _slowDuration);
				}
				_playerController.StunPlayer(3f);
				SpawnFireEffect();
				DestroyBarrel();

			}

			if (collision.gameObject.CompareTag("Rock"))
			{
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Boomerang"))
			{
				_scoreManager.AddScore(_scoreValue);
				SpawnFireEffect();
				PlayAudioBarrelClip(0);
				StartCoroutine(DestroyBarrelWithShovel());
			}

		}
		#endregion

		// Utility Methods
		#region Utility Methods

		private void SpawnFireEffect()
		{
			if (_fireEffectPrefab != null)
			{
				// Instantiate the fire effect at the current position
				GameObject fireEffect = Instantiate(_fireEffectPrefab, transform.position, Quaternion.identity);

			}
		}

		private IEnumerator DestroyBarrelWithShovel()
		{
			_spriteRenderer.enabled = false;
			_collider.enabled = false;

			yield return new WaitForSeconds(1);
			Destroy(gameObject);
		}


		private void PlayAudioBarrelClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _barrelClips.Length)
			{
				_barrelAudioSource.clip = _barrelClips[clipIndex];
				_barrelAudioSource.Play();
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

		private void DestroyBarrel()
		{
			Destroy(gameObject);
		}

		#endregion
	}
}

