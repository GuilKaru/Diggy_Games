using UnityEngine;
namespace Diggy_MiniGame_1
{
	using System.Collections;
	using UnityEngine;

	public class BarrelPush : MonoBehaviour
	{
		// Serialize Fields
		#region SerializeField
		[SerializeField]
		private float _speed = 2f;

		[SerializeField]
		private float _destroyXPosition = -10f;

		[SerializeField]
		private int _scoreValue = 10;

		[SerializeField]
		private int _maxHits = 5; // Maximum hits to destroy the barrel

		[SerializeField]
		private GameObject _pushBarrelParent;

		[SerializeField]
		private float _pushForce = 500f;

		[Header("Barrel Components Settings")]
		[SerializeField]
		SpriteRenderer _spriteRenderer;
		[SerializeField]
		Collider2D _collider;
		[SerializeField]
		Collider2D _trigger;

		[Header("Barrel Audio")]
		[SerializeField]
		private AudioSource _barrelAudioSource;
		[SerializeField]
		private AudioClip[] _barrelClips;
		#endregion

		// Private Variables
		#region Private Variables
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		private Rock _rock;
		private float _originalSpeed;
		private int _currentHits = 0; // Current hits taken
		private bool _isPushingPlayer = false; // Tracks if the player is being pushed
		private Rigidbody2D _playerRb;
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Find the ScoreManager script in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();
			_playerHealth = FindObjectOfType<PlayerHealth>();
			_playerController = FindObjectOfType<PlayerController>();
			_rock= FindObjectOfType<Rock>();
			_barrelAudioSource = GetComponent<AudioSource>();
			_collider = GetComponent<Collider2D>();
			_trigger = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_originalSpeed = _speed;
			if (_scoreManager == null)
			{
				Debug.LogError("ScoreManager not found in the scene. Ensure there is a GameObject with the ScoreManager script.");
			}
		}

		private void Update()
		{
			MoveLeft();

			if (transform.position.x <= _destroyXPosition)
			{
				DestroyBarrel();
			}

			if (_isPushingPlayer && _playerRb != null)
			{
				Vector2 pushDirection = new Vector2(-1, 0); // Left direction
				_playerRb.AddForce(pushDirection * _pushForce * Time.deltaTime);
			}
		}
		#endregion

		// Movement Methods
		#region Movement Methods
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
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Boomerang"))
			{
				_currentHits++;
				Debug.Log("Barrel hit by Shovel. Current hits: " + _currentHits);

				if (_currentHits >= _maxHits)
				{
					_scoreManager.AddScore(_scoreValue);
					StartCoroutine(DestroyBarrelWithShovel());
				}
				Destroy(collision.gameObject);
			}

			if (collision.gameObject.CompareTag("Rock"))
			{
				DestroyBarrel();
			}

			_playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
			if (_playerRb != null)
			{
				_isPushingPlayer = true; // Start pushing the player
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				_isPushingPlayer = false; // Stop pushing the player
				_playerRb = null; // Clear the reference to the player's Rigidbody2D
			}
		}
		#endregion

		// Utility Methods
		#region Utility Methods
		private void DestroyBarrel()
		{
			Destroy(gameObject);
			Destroy(_pushBarrelParent);
		}

		private IEnumerator DestroyBarrelWithShovel()
		{
			PlayAudioBarrelClip(0);
			_spriteRenderer.enabled = false;
			_collider.enabled = false;

			yield return new WaitForSeconds(1);
			Destroy(gameObject);
			Destroy(_pushBarrelParent);
		}


		private void PlayAudioBarrelClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _barrelClips.Length)
			{
				_barrelAudioSource.clip = _barrelClips[clipIndex];
				_barrelAudioSource.Play();
			}
		}

		#endregion
	}


}
