using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Diggy_MiniGame_1
{
	public class PlayerController : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields

		[Header("Player Movement")]
		[SerializeField]
		private float _moveSpeed = 5f;
		[SerializeField]
		private float _leftLimit = -8f; // Left boundary for X position
		[SerializeField]
		private float _rightLimit = 8f; // Right boundary for X position

		[Header("Teleport Settings")]
		[SerializeField]
		private float _teleportDistance = 5f; // Distance to teleport up or down
		[SerializeField]
		private float _upperLimit = 3f; // Upper boundary for Y position
		[SerializeField]
		private float _lowerLimit = -5f; // Lower boundary for Y position
		[SerializeField]
		private float _teleportCooldown = 0.5f; // Cooldown time between teleports

		[Header("Attack Settings")]
		[SerializeField] 
		private GameObject attackPrefab; // Red circle prefab for attack
		[SerializeField] 
		private float attackDistance = 1.5f; // Distance in front of player where attack will appear
		[SerializeField]
		private float attackDuration = 1f;

		[Header("Shield Settings")]
		[SerializeField]
		private GameObject shieldObject; 
		[SerializeField]
		private Collider2D playerCollider;
		#endregion

		// Private Variables
		#region Private Variables
		private Vector2 _moveInput;
		private Rigidbody2D _rb;
		private PlayerInput _playerInput;
		private InputAction _moveAction;
		private InputAction _attackAction;
		private InputAction shieldAction;
		private bool isShieldActive = false;
		private bool _canTeleport = true; // Tracks if teleportation is allowed
		private bool isStunned = false; // Tracks if the player is stunned
		private float stunEndTime = 0f; // Time when the stun effect ends
		#endregion

		// Initialization
		#region Initialization

		private void Awake()
		{
			// Initialize components
			_rb = GetComponent<Rigidbody2D>();
			if (_rb == null)
			{
				Debug.LogError("Rigidbody2D component is missing!");
			}

			_playerInput = GetComponent<PlayerInput>();
			if (_playerInput == null)
			{
				Debug.LogError("PlayerInput component is missing!");
			}


			// Setup input actions
			_moveAction = _playerInput.actions["Move"];
			_attackAction = _playerInput.actions["Attack"];
			shieldAction = _playerInput.actions["Shield"];
		}

		private void OnEnable()
		{
			_moveAction.performed += OnMoveInput;
			_moveAction.canceled += OnMoveInput;

			_attackAction.started += OnAttackStart;
			shieldAction.performed += OnShieldHold;
			shieldAction.canceled += OnShieldRelease;

		}

		private void OnDisable()
		{
			_moveAction.performed -= OnMoveInput;
			_moveAction.canceled -= OnMoveInput;
			
			_attackAction.started -= OnAttackStart;
			shieldAction.performed -= OnShieldHold;
			shieldAction.canceled -= OnShieldRelease;
		}

		#endregion

		// Movement
		#region Movement

		private void FixedUpdate()
		{
			// Handle Stun Logic
			if (isStunned && Time.time >= stunEndTime)
			{
				isStunned = false; // End the stun effect
				Debug.Log("Player is no longer stunned.");
			}

			// Allow movement and teleportation only if the player is not stunned or shielding
			if (!isStunned)
			{
				if (!isShieldActive)
				{
					HandleHorizontalMovement(); // Allow normal movement
				}
			}
			else
			{
				_moveInput = Vector2.zero; // Block movement input while stunned
			}
		}

		private void OnMoveInput(InputAction.CallbackContext context)
		{
			if (!isShieldActive) // Only allow movement/teleport when the shield is not active
			{
				_moveInput = context.ReadValue<Vector2>();

				// Check for teleportation input (W or S keys)
				if (_moveInput.y > 0) // W key pressed
				{
					TryTeleport(Vector3.up);
				}
				else if (_moveInput.y < 0) // S key pressed
				{
					TryTeleport(Vector3.down);
				}
			}
			else
			{
				_moveInput = Vector2.zero; // Block movement input while shield is active
			}
		}

		private void HandleHorizontalMovement()
		{
			if (_moveInput.x != 0) // Horizontal movement only
			{
				Vector2 newPosition = _rb.position + new Vector2(_moveInput.x * _moveSpeed * Time.fixedDeltaTime, 0);

				// Clamp the new position to the horizontal limits
				newPosition.x = Mathf.Clamp(newPosition.x, _leftLimit, _rightLimit);

				// Apply the movement
				_rb.MovePosition(newPosition);
			}
		}

		#endregion

		// Teleportation
		#region Teleportation

		private void TryTeleport(Vector3 direction)
		{
			// Check if teleportation is allowed
			if (!_canTeleport) return;

			// Calculate the new position
			Vector3 newPosition = transform.position + direction * _teleportDistance;

			// Check boundaries
			if (direction == Vector3.up && newPosition.y > _upperLimit)
			{
				Debug.Log("Cannot teleport up, reached the upper limit!");
				return;
			}
			else if (direction == Vector3.down && newPosition.y < _lowerLimit)
			{
				Debug.Log("Cannot teleport down, reached the lower limit!");
				return;
			}

			// Start teleportation with horizontal limits applied
			newPosition.x = Mathf.Clamp(newPosition.x, _leftLimit, _rightLimit);
			StartCoroutine(TeleportCooldown(newPosition));
		}

		private IEnumerator TeleportCooldown(Vector3 targetPosition)
		{
			_canTeleport = false; // Prevent further teleportation during cooldown
			transform.position = targetPosition; // Teleport the player
			yield return new WaitForSeconds(_teleportCooldown); // Wait for the cooldown
			_canTeleport = true; // Allow teleportation again
		}
		#endregion

		//Attack
		#region Attack
		private void OnAttackStart(InputAction.CallbackContext context)
		{
			if (isShieldActive)
			{
				Debug.Log("Attack blocked because the shield is active.");
				return; // Prevent attack if shield is active
			}

			// Instantiate the red circle in front of the player
			Vector3 spawnPosition = transform.position + Vector3.right * attackDistance;
			GameObject attackObject = Instantiate(attackPrefab, spawnPosition, Quaternion.identity);
			Destroy(attackObject, attackDuration);
		}
		#endregion

		//Shield
		#region Shield
		private void OnShieldHold(InputAction.CallbackContext context)
		{
			ActivateShield(); // Called when the shield button is held
		}

		private void OnShieldRelease(InputAction.CallbackContext context)
		{
			DeactivateShield(); // Called when the shield button is released
		}

		private void ActivateShield()
		{
			isShieldActive = true;
			playerCollider.enabled = false; // Disable player collider
			shieldObject.SetActive(true); // Activate shield visual/effect
			Debug.Log("Shield Activated: Player movement and teleportation disabled.");
		}

		private void DeactivateShield()
		{
			isShieldActive = false;
			playerCollider.enabled = true; // Enable player collider
			shieldObject.SetActive(false); // Deactivate shield visual/effect
			Debug.Log("Shield Deactivated: Player movement and teleportation enabled.");
		}

		#endregion

		//Stun
		#region Stun
		public void StunPlayer(float duration)
		{
			isStunned = true;
			stunEndTime = Time.time + duration; // Calculate when the stun ends
			Debug.Log("Player stunned for " + duration + " seconds.");
		}
		#endregion

	}

}
