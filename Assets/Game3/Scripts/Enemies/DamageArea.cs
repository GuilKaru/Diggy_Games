using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class DamageArea : MonoBehaviour
	{
		[SerializeField] private float _duration = 5f;
		private float _timer;

		private void Start()
		{
			_timer = _duration;
		}

		private void Update()
		{
			_timer -= Time.deltaTime;
			if (_timer <= 0f)
			{
				Destroy(gameObject);
			}
		}

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

