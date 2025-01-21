using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class Shovel : MonoBehaviour
	{
		public Vector3 target; // Target position for the bullet

		private void Update()
		{
			// Move towards the target
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 10f);

			// Destroy the bullet if it reaches the target
			if (Vector3.Distance(transform.position, target) < 0.1f)
			{
				Destroy(gameObject);
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("PushBarrel"))
			{
				Destroy(gameObject);
			}

			if (collision.gameObject.CompareTag("Barrel"))
			{
				Destroy(gameObject);
			}
		}
	}
}

