using System.Collections.Generic;
using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class TransportZigzag : MonoBehaviour
	{
		[Header("Barrel Settings")]
		[SerializeField] private float _moveSpeed = 3f;
		[SerializeField] private bool _moveRight = true; // True = move right, false = move left
		[SerializeField] private float _destroyXPositionLeft = -10f;
		[SerializeField] private float _destroyXPositionRight = 10f;

		[Header("Lane Switching Settings")]
		[SerializeField] private float[] _laneYPositions; // Possible Y positions for transport lanes
		[SerializeField] private float[] _switchXPositions; // X positions where it can switch lanes
		[SerializeField] private float _laneSwitchSpeed = 3f;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;
		//[SerializeField]
		//private SpriteRenderer _spriteRenderer;

		private string _idleAnim = "TransportZigZag_Idle";

		private string _currentState;
		private float _originalSpeed;
		private bool _isSwitchingLane = false;
		private float _targetY; // Y position to smoothly move to

		private void Start()
		{
			ChangeAnimationState(_idleAnim);
			_originalSpeed = _moveSpeed;
			_targetY = transform.position.y; // Start at the current Y position
		}

		private void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
			//_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		}

		private void Update()
		{
			MoveBarrel();
			SmoothMoveToLane();
			CheckPositionAndDestroy();
		}

		private void MoveBarrel()
		{
			float direction = _moveRight ? 1f : -1f;
			transform.Translate(Vector2.right * direction * _moveSpeed * Time.deltaTime);

			// Check if it's time to switch lanes
			if (!_isSwitchingLane && IsNearSwitchXPosition(transform.position.x))
			{
				TrySwitchLane();
			}
		}

		private bool IsNearSwitchXPosition(float currentX)
		{
			foreach (float switchX in _switchXPositions)
			{
				if (Mathf.Abs(currentX - switchX) < 0.2f) // Small tolerance for accuracy
				{
					return true;
				}
			}
			return false;
		}

		private void TrySwitchLane()
		{
			float currentY = transform.position.y;
			int currentLaneIndex = GetCurrentLaneIndex(currentY);

			if (currentLaneIndex != -1)
			{
				int newLaneIndex;

				if (currentLaneIndex == 0)
				{
					newLaneIndex = 1; // If in top lane, move down
				}
				else if (currentLaneIndex == _laneYPositions.Length - 1)
				{
					newLaneIndex = currentLaneIndex - 1; // If in bottom lane, move up
				}
				else
				{
					// Randomly move up or down
					newLaneIndex = Random.value > 0.5f ? currentLaneIndex + 1 : currentLaneIndex - 1;
				}

				// Set the target Y position for smooth movement
				_targetY = _laneYPositions[newLaneIndex];
				_isSwitchingLane = true; // Start the transition
			}
		}

		private void SmoothMoveToLane()
		{
			if (_isSwitchingLane)
			{
				// Smoothly move toward the target Y position
				float newY = Mathf.Lerp(transform.position.y, _targetY, _laneSwitchSpeed * Time.deltaTime);
				transform.position = new Vector2(transform.position.x, newY);

				// Stop switching when close enough
				if (Mathf.Abs(transform.position.y - _targetY) < 0.05f)
				{
					transform.position = new Vector2(transform.position.x, _targetY); // Snap to final Y
					_isSwitchingLane = false;
				}
			}
		}

		private int GetCurrentLaneIndex(float currentY)
		{
			for (int i = 0; i < _laneYPositions.Length; i++)
			{
				if (Mathf.Abs(currentY - _laneYPositions[i]) < 0.1f)
				{
					return i;
				}
			}
			return -1; // Not in any defined lane
		}

		#region Speed Control
		public void SetSpeed(float newSpeed)
		{
			_moveSpeed = newSpeed;
		}

		public void RestoreSpeed()
		{
			_moveSpeed = _originalSpeed;
		}
		#endregion

		private void CheckPositionAndDestroy()
		{
			if ((_moveRight && transform.position.x >= _destroyXPositionRight) ||
				(!_moveRight && transform.position.x <= _destroyXPositionLeft))
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
				if (playerController != null)
				{
					DestroyCoins(playerController);
					playerController.PlayerTakeDamage();
					playerController.DropAllPickups();
				}
			}

			if (other.CompareTag("Rock"))
			{
				Destroy(gameObject);
			}

			if (other.CompareTag("TransportLine"))
			{
				TransportLine transportLine = other.GetComponent<TransportLine>();
				if (transportLine != null)
				{
					transportLine.SetDriftDirection(_moveRight ? TransportLine.DriftDirection.Right : TransportLine.DriftDirection.Left);
				}
			}
		}

		private void DestroyCoins(PlayerController playerController)
		{
			foreach (GameObject carriedObject in playerController.GetCarriedObjects())
			{
				if (carriedObject.CompareTag("PickUp"))
				{
					Destroy(carriedObject);
				}
			}
		}


		public void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}
	}
}


