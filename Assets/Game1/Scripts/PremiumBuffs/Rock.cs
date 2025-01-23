using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class Rock : MonoBehaviour
	{
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
			Debug.Log($"Rock took damage. Remaining health: {_health}");

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

		#endregion
	}
}

