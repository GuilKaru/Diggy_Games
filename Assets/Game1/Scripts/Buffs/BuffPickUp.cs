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

		private ScoreManager _scoreManager;

		private void Start()
		{
			// Find the BuffManager in the scene dynamically
			_scoreManager = FindObjectOfType<ScoreManager>();
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
			if (other.TryGetComponent<PlayerController>(out PlayerController player))
			{
				_scoreManager.AddScore(_scoreValue);
				Destroy(gameObject); // Destroy the pickup after collection
			}
		}
	}
}

