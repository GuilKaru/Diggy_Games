using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Diggy_MiniGame_2
{
	public class TransportLine : MonoBehaviour
	{
		public enum DriftDirection { None, Left, Right }

		[Header("Transport Line Settings")]
		[SerializeField]
		private DriftDirection _driftDirection; // Toggle between Left, Right, or None

		[SerializeField]
		private float _driftSpeed = 0.1f; // Optional custom drift speed

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _idleAnim = "TransportLine_Idle";
		private string _leftAnim = "TransportLine_Left";
		private string _rightAnim = "TransportLine_Right";
		private string _currentState;

		private Dictionary<PlayerController, int> _playerTriggers = new Dictionary<PlayerController, int>();

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			
		}

		private void Update()
		{
			UpdateAnimationBasedOnDirection();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				PlayerController playerController = other.GetComponent<PlayerController>();
				if (playerController != null)
				{
					if (_driftDirection == DriftDirection.Left)
					{
						playerController.StartLeftDrift();
					}
					else if (_driftDirection == DriftDirection.Right)
					{
						playerController.StartRightDrift();

					}
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				PlayerController playerController = other.GetComponent<PlayerController>();
				if (playerController != null)
				{
					playerController.StopDrift(); // Always stop drifting immediately
				}
			}
		}

		public void SetDriftDirection(DriftDirection direction)
		{
			_driftDirection = direction;
			UpdateAnimationBasedOnDirection();
		}

		private void UpdateAnimationBasedOnDirection()
		{
			switch (_driftDirection)
			{
				case DriftDirection.Left:
					ChangeAnimationState(_leftAnim);
					break;

				case DriftDirection.Right:
					ChangeAnimationState(_rightAnim);
					break;

				case DriftDirection.None:
					ChangeAnimationState(_idleAnim);
					break;
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
