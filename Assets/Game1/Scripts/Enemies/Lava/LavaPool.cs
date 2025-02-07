using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class LavaPool : MonoBehaviour
	{
		//Private Variables
		#region Private Variables
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		#endregion

		//Initialization
		#region Initialization
		private void Awake()
		{
			_playerHealth = FindObjectOfType<PlayerHealth>();
			_playerController = FindObjectOfType<PlayerController>();
		}
		#endregion

		//Collision
		#region Collision

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Player"))
			{
				_playerController.PlayAudioPlayerHitLavaClip(0);
				_playerHealth.Damage(2);
			}

			if (collision.CompareTag("PushBarrel"))
			{
				Destroy(collision.gameObject);
			}

			if (collision.CompareTag("Barrel"))
			{
				Destroy(collision.gameObject);
			}
		}
		#endregion

	}
}

