using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class DangerZone : MonoBehaviour
	{

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				// Destroy coins carried by the player when they collide with the barrel
				PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
				if (playerController != null)
				{
					DestroyCoins(playerController);
					playerController.PlayerTakeDamage();
				}
			}

		}


		private void DestroyCoins(PlayerController playerController)
		{
			// Destroy coins if the player is carrying any
			foreach (GameObject carriedObject in playerController.GetCarriedObjects()) // Use the GetCarriedObjects method
			{
				if (carriedObject.CompareTag("PickUp")) // Assuming coins have a "PickUp" tag
				{
					Destroy(carriedObject);
				}
			}
		}
	}

}
