using UnityEngine;
namespace Diggy_MiniGame_1
{
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
		#endregion

		// Private Variables
		#region Private Variables
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		private Rock _rock;
		private float _originalSpeed;
		private int _currentHits = 0; // Current hits taken
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Find the ScoreManager script in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();
			_playerHealth = FindObjectOfType<PlayerHealth>();
			_playerController = FindObjectOfType<PlayerController>();
			_rock = FindObjectOfType<Rock>();
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
					DestroyBarrel();
				}
				Destroy(collision.gameObject);
			}

			if (collision.gameObject.CompareTag("Rock"))
			{
				_rock.RockTakeDamage();
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Player"))
			{
				PushPlayerBackwards();
			}
		}
		#endregion

		// Utility Methods
		#region Utility Methods
		private void DestroyBarrel()
		{
			Destroy(gameObject);
		}

		private void PushPlayerBackwards()
		{
			// Implement logic to push the player backwards
			// For example:
			_playerController.transform.Translate(Vector3.left * _speed * Time.deltaTime);
			// Adjust the speed and direction as needed
		}
		#endregion
	}


}
