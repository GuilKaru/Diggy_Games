using UnityEngine;
namespace Diggy_MiniGame_1
{



	public class Boiler : MonoBehaviour
	{
		#region Private Variables
		[SerializeField] 
		private SpriteRenderer[] _healthSprites; // Array of sprites representing health
		[SerializeField] private Collider2D _triggerZone;        // Trigger zone to detect enemies
		#endregion

		#region Unity Methods
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Barrel")) // Assuming enemies are tagged as "Enemy"
			{
				DestroyNextHealthSprite();
			}
		}
		#endregion

		#region Private Methods
		private void DestroyNextHealthSprite()
		{
			foreach (var sprite in _healthSprites)
			{
				if (sprite != null) // Find the first active sprite
				{
					Destroy(sprite.gameObject); // Destroy the sprite GameObject
					break; // Exit after destroying one sprite
				}
			}

			CheckForCompleteDestruction();
		}

		private void CheckForCompleteDestruction()
		{
			// If no sprites remain, take additional action
			bool allDestroyed = true;
			foreach (var sprite in _healthSprites)
			{
				if (sprite != null) // If any sprite is still active
				{
					allDestroyed = false;
					break;
				}
			}

			if (allDestroyed)
			{
				Debug.Log("All health sprites are destroyed!");
				// Add logic for complete destruction (e.g., disable this GameObject)
			}
		}
		#endregion
	}
}

