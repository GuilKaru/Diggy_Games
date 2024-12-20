using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class Barrel : MonoBehaviour
	{
		// Serialize Fields
		#region SerializeField
		[SerializeField]
		private float _speed = 2f;

		[SerializeField]
		private float _destroyXPosition = -10f;

		[SerializeField]
		private int _scoreValue = 10;
		#endregion

		//Private Variables
		#region Private Variables
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		#endregion

		// Initialization
		#region Initialization

		private void Start()
		{
			// Find the ScoreManager script in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();
			_playerHealth = FindObjectOfType<PlayerHealth>();
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

		//Collision
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("AttackPoint"))
			{
				_scoreManager.AddScore(_scoreValue); 
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("House"))
			{
				_playerHealth.Damage(1);
				DestroyBarrel();
			}
		}
		#endregion

		// Utility Methods
		#region Utility Methods
		private void DestroyBarrel()
		{
			Destroy(gameObject);
		}
		#endregion
	}
}

