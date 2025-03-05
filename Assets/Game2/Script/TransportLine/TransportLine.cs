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

		private Dictionary<PlayerController, int> _playerTriggers = new Dictionary<PlayerController, int>();

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
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = _driftDirection == DriftDirection.Left ? Color.red :
			   _driftDirection == DriftDirection.Right ? Color.blue :
			   Color.yellow; // Different colors for each drift type

			BoxCollider2D box = GetComponent<BoxCollider2D>();
			if (box != null)
			{
				Vector2 size = box.size;
				Vector3 center = transform.position + (Vector3)box.offset;

				// Draw wire cube showing the transport area
				Gizmos.DrawWireCube(center, size);
			}

			// Draw arrow direction
			if (_driftDirection != DriftDirection.None)
			{
				Vector3 arrowDirection = _driftDirection == DriftDirection.Left ? Vector3.left : Vector3.right;
				Gizmos.color = Color.white;
				Gizmos.DrawLine(transform.position, transform.position + arrowDirection * 1f);
				Gizmos.DrawSphere(transform.position + arrowDirection * 1f, 0.1f);
			}
		}
	}

}
