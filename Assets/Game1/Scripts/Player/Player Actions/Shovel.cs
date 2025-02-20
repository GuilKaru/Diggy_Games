using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class Shovel : MonoBehaviour
	{
		public Vector3 target; // Target position for the shovel
		private Collider2D _shovelCollider;
		public float disableXPosition = 5f; // X position where collider is disabled

		private void Start()
		{
			_shovelCollider = GetComponent<Collider2D>();
			_shovelCollider.enabled = true; // Collider is enabled at spawn
		}

		private void Update()
		{
			// Move towards the target
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 10f);

			// Disable collider when reaching a certain X position
			if (transform.position.x >= disableXPosition)
			{
				_shovelCollider.enabled = false;
			}

			// Destroy the shovel if it reaches the target
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

