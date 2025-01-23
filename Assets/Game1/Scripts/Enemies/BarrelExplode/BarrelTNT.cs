using UnityEngine;
using System.Collections;
namespace Diggy_MiniGame_1
{
	public class BarrelTNT : MonoBehaviour
	{
		// Serialize Fields
		#region SerializeField
		[Header("Movement Settings")]
		[SerializeField]
		private float _speed = 2f;
		[SerializeField]
		private float _destroyXPosition = -10f;

		[Header("Score Settings")]
		[SerializeField]
		private int _scoreValue = 10;

		[Header("Explosion Settings")]
		[SerializeField]
		private float explosionRadius = 2f; // Radius of the explosion
		[SerializeField]
		private float stunDuration = 2f; // Duration for which the player is stunned
		[SerializeField]
		private LayerMask playerLayer; // LayerMask to detect the player

		[Header("Visual Effect Settings")]
		[SerializeField]
		private GameObject _explosionEffectPrefab; // Prefab for the fire/tar effect
		[SerializeField]
		private float _fireEffectDuration = 2f; // Duration before the fire effect disappears

		#endregion

		// Private Variables
		#region Private Variables
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		private Rock _rock;
		private bool hasExploded = false; // Tracks if the TNT has already exploded
		private float _originalSpeed;
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
			_originalSpeed = _speed;
			if (_scoreManager == null)
			{
				Debug.LogError("ScoreManager not found in the scene. Ensure there is a GameObject with the ScoreManager script.");
			}

			if (_playerHealth == null)
			{
				Debug.LogError("PlayerHealth not found in the scene. Ensure there is a GameObject with the PlayerHealth script.");
			}
		}

		private void Update()
		{
			MoveLeft();

			if (transform.position.x <= _destroyXPosition)
			{
				DestroyBarrel();
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

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (hasExploded) return; // Prevent multiple explosions

			if (collision.gameObject.CompareTag("Shield"))
			{
				_scoreManager.AddScore(_scoreValue);
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Boomerang"))
			{
				_scoreManager.AddScore(_scoreValue);
				SpawnExplosionEffect();
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Rock"))
			{
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Player"))
			{
				ExplodeAndStun(collision.gameObject);
				SpawnExplosionEffect();
				_playerHealth.Damage(1);
			}
		}

		#endregion

		// Explosion and Stun Logic
		#region Explosion and Stun

		private void ExplodeAndStun(GameObject player)
		{
			hasExploded = true;


			// Check if the player is within the explosion radius
			Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);

			foreach (Collider2D obj in hitObjects)
			{
				if (obj.CompareTag("Player"))
				{
					PlayerController playerController = obj.GetComponent<PlayerController>();
					if (playerController != null)
					{
						playerController.StunPlayer(stunDuration);
					}
				}
			}

			// Destroy the TNT object
			DestroyBarrel();
		}

		private void OnDrawGizmosSelected()
		{
			// Visualize explosion radius in Scene view
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, explosionRadius);
		}

		#endregion

		// Utility Methods
		#region Utility Methods

		private void SpawnExplosionEffect()
		{
			if (_explosionEffectPrefab != null)
			{
				// Instantiate the fire effect at the current position
				GameObject fireEffect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);

			}
		}


		private void DestroyBarrel()
		{
			Destroy(gameObject);
		}

		#endregion
	}
}

