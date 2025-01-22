using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class Shield : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField]
		private int _maxHits = 3; // Maximum hits the shield can absorb
		[SerializeField]
		private Transform _player; // Reference to the player object

		#endregion

		#region Private Variables

		private int _currentHits;

		#endregion

		#region Unity Methods

		private void Start()
		{
			// Initialize shield hits
			_currentHits = _maxHits;

			if (_player == null)
			{
				Debug.LogError("Shield: Player reference is missing! Assign it in the Inspector.");
			}
		}

		private void Update()
		{
			// Follow the player's position
			if (_player != null)
			{
				transform.position = _player.position;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			// Check if the shield was hit by a projectile
			if (collision.CompareTag("Barrel"))
			{
				_currentHits--;

				Debug.Log($"Shield absorbed a hit! Remaining hits: {_currentHits}");

				// Destroy the projectile
				Destroy(collision.gameObject);

				// Deactivate the shield if no hits remain
				if (_currentHits <= 0)
				{
					DeactivateShield();
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Activates the shield and resets the hit counter.
		/// </summary>
		/// <param name="maxHits">The maximum number of hits the shield can absorb.</param>
		public void ActivateShield(int maxHits)
		{
			_maxHits = maxHits;
			_currentHits = maxHits;

			gameObject.SetActive(true);
			Debug.Log($"Shield activated with {maxHits} hits.");
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Deactivates the shield when all hits are used up.
		/// </summary>
		private void DeactivateShield()
		{
			gameObject.SetActive(false);
			Debug.Log("Shield deactivated.");
		}

		#endregion
	}
}

