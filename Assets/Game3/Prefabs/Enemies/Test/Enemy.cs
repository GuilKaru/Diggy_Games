using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class Enemy : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				// Destroy coins carried by the player when they collide with the barrel
				PlayerHealth _playerHealth = other.gameObject.GetComponent<PlayerHealth>();
				_playerHealth.Damage(1);
			}
		}
	}
}

