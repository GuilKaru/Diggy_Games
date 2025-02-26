using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Diggy_MiniGame_2
{
	public class PlayerController : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields

		[Header("Movement Settings")]
		[SerializeField]
		private float _moveSpeed = 5f;
		[SerializeField]
		private Vector2 _minBounds;
		[SerializeField]
		private Vector2 _maxBounds;

		[Header("Pickup Settings")]
		[SerializeField]
		private float _pickupRadius = 1f;
		[SerializeField]
		private int _maxCarriedObjects = 5;
		[SerializeField]
		private Vector3 _pickupOffset = new Vector3(0, 1f, 0);
		#endregion

		//Private variables
		#region Private variables
		private Vector2 _moveInput;
		private PlayerInput _playerInput;
		private InputAction _moveAction;
		private InputAction _pickUpAction;
		private Rigidbody2D _rb;
		private int _carriedObjectsCount;
		private List<GameObject> _carriedObjects = new List<GameObject>();
		#endregion

		//Initialization
		#region Initialization

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
			_playerInput = GetComponent<PlayerInput>();
			_moveAction = _playerInput.actions["Move"];
			_pickUpAction = _playerInput.actions["PickUp"];
		}

		private void OnEnable()
		{
			_moveAction.performed += OnMoveInput;
			_moveAction.canceled += OnMoveInput;
			_pickUpAction.performed += OnPickup;
		}

		private void OnDisable()
		{
			_moveAction.performed -= OnMoveInput;
			_moveAction.canceled -= OnMoveInput;
			_pickUpAction.performed -= OnPickup;
		}
		#endregion


		//Movement
		#region Movement 

		private void FixedUpdate()
		{
			MovePlayer();
			UpdateCarriedObjects();
		}

		private void MovePlayer()
		{
			float adjustedSpeed = _moveSpeed - (_carriedObjects.Count * 0.5f);
			adjustedSpeed = Mathf.Max(adjustedSpeed, 1f); // Prevent speed from going negative

			// Calculate movement
			Vector2 movement = _moveInput * adjustedSpeed * Time.fixedDeltaTime;

			// New position after movement
			Vector2 newPosition = _rb.position + movement;

			// Clamp position to stay within the bounds
			newPosition.x = Mathf.Clamp(newPosition.x, _minBounds.x, _maxBounds.x);
			newPosition.y = Mathf.Clamp(newPosition.y, _minBounds.y, _maxBounds.y);

			// Move the Rigidbody to the clamped position
			_rb.MovePosition(newPosition);
		}

		private void OnMoveInput(InputAction.CallbackContext context)
		{
			_moveInput = context.ReadValue<Vector2>();
		}

		private void UpdateCarriedObjects()
		{
			// Remove null objects from the list
			_carriedObjects.RemoveAll(item => item == null);

			for (int i = 0; i < _carriedObjects.Count; i++)
			{
				if (_carriedObjects[i] != null)
				{
					_carriedObjects[i].transform.position = transform.position + _pickupOffset + new Vector3(0, -i * 0.2f, 0);
				}
			}
		}

		#endregion

		//PickUp Objects
		#region PickUp Objects

		private void OnPickup(InputAction.CallbackContext context)
		{
			if (_carriedObjects.Count >= _maxCarriedObjects)
				return; // Prevent picking up more than max allowed

			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _pickupRadius);
			foreach (Collider2D col in colliders)
			{
				if (col.CompareTag("PickUp") && !_carriedObjects.Contains(col.gameObject))
				{
					_carriedObjects.Add(col.gameObject);
					col.gameObject.transform.SetParent(transform);

					// Set the sorting order of the sprite to create a stacked visual effect
					SpriteRenderer spriteRenderer = col.GetComponent<SpriteRenderer>();
					if (spriteRenderer != null)
					{
						spriteRenderer.sortingOrder = _carriedObjects.Count;
					}
					break;
				}
			}
		}


		#endregion


		// Gizmos
		#region Gizmos
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, _pickupRadius);
		}
		#endregion
	}

}

