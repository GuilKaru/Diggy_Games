using UnityEngine;
using System;

namespace Diggy_MiniGame_1
{
	public class Boomerang : MonoBehaviour
	{
		public event Action OnBoomerangCaught;  // Triggered when the player catches the boomerang
		public event Action OnBoomerangMissed; // Triggered when the boomerang returns but isn't caught

		[SerializeField]
		private float _speed = 10f;
		[SerializeField]
		private float _maxDistance = 5f;
		[SerializeField]
		private float _returnThresholdX = -8f;

		public bool _isReturning = false;

		private Transform _player;
		private Vector3 _startPosition;


		private void Start()
		{
			_player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
			_startPosition = transform.position;
		}

		private void Update()
		{
			if (!_isReturning)
			{
				// Move the boomerang to the right
				transform.position += Vector3.right * _speed * Time.deltaTime;

				// Check if max distance is reached
				if (Vector3.Distance(_startPosition, transform.position) >= _maxDistance)
				{
					_isReturning = true;
				}
			}
			else
			{
				// Return to the left (fixed threshold or player position)
				Vector3 targetPosition = new Vector3(_returnThresholdX, transform.position.y, transform.position.z);
				transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

				// Check if the player catches the boomerang
				if (Vector3.Distance(transform.position, _player.position) < 0.5f)
				{
					Debug.Log("Boomerang caught by player!");
					OnBoomerangCaught?.Invoke(); // Notify that the boomerang was caught
					Destroy(gameObject);
				}

				// Check if the boomerang missed the player and reached the return threshold
				if (transform.position.x <= _returnThresholdX)
				{
					Debug.Log("Boomerang missed. Returning to starting position.");
					OnBoomerangMissed?.Invoke(); // Notify that the boomerang missed
					Destroy(gameObject);
				}
			}


		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("Barrel"))
			{
				_isReturning = true;
			}
		}
	}

}