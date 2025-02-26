using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class DropZone : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("PickUp"))
			{
				Debug.Log("Si toco la moneda");
				PickUp pickup = other.GetComponent<PickUp>();
				if (pickup != null)
				{
					pickup.HandleDrop();
					Destroy(other.gameObject);
				}
			}
		}
	}

}
