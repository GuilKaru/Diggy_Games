using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

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

		[Header("Mobile Joystick")]
		[SerializeField]
		private JoyStick _joystick;
		[SerializeField]
		private Button _shootButton;

		[Header("Pickup Settings")]
		[SerializeField]
		private float _pickupRadius = 1f;
		[SerializeField]
		private int _maxCarriedObjects = 5;
		[SerializeField]
		private Vector3 _pickupOffset = new Vector3(0, 1f, 0);

		[Header("Coin Visuals")]
		[SerializeField]
		private List<GameObject> _coinVisuals;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		[Header("Player Audio")]
		[SerializeField]
		private AudioSource _coinAudioSource;
		[SerializeField]
		private AudioClip[] _coinClips;
		[SerializeField]
		private AudioSource _hitAudioSource;
		[SerializeField]
		private AudioClip[] _playerHitClip;

		[Header("Player Hit")]
		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[Header("Player Collider")]
		[SerializeField]
		private Collider2D _collider2D;

		[Header("Difficulty Drift Settings")]
		[SerializeField]
		private float _leftDriftSpeed = 0.1f;
		[SerializeField]
		private float _rightDriftSpeed = 0.1f;

		
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
		private bool _isDriftingLeft = false;
		private bool _isDriftingRight = false;
		private bool _isStunned = false; // Tracks if the player is stunned
		private float _stunEndTime = 0f; // Time when the stun effect ends

		private string _currentState;
		private string _idleAnim = "Player_Idle";
		private string _hitAnim = "Player_Hit";

		private bool _isUsingJoystick;
		#endregion

		//Initialization
		#region Initialization

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
			_playerInput = GetComponent<PlayerInput>();
			ChangeAnimationState(_idleAnim);
			_moveAction = _playerInput.actions["Move"];
			_pickUpAction = _playerInput.actions["PickUp"];

			_isUsingJoystick = _joystick != null && _joystick.InputVector.magnitude > 0.1f;
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
			_moveInput = _joystick.InputVector + _moveAction.ReadValue<Vector2>();
			_moveInput = Vector2.ClampMagnitude(_moveInput, 1f); // Normalize input

			if (_isStunned && Time.time >= _stunEndTime)
			{
				_isStunned = false; // End the stun effect
				Debug.Log("Player is no longer stunned.");
			}

			// Allow movement only if the player is not stunned knocked back
			if (!_isStunned)
			{
				MovePlayer();
			}
			UpdateCarriedObjects();
		}

		private void MovePlayer()
		{
			float adjustedSpeed = _moveSpeed - (_carriedObjects.Count * 0.5f);
			adjustedSpeed = Mathf.Max(adjustedSpeed, 1f); // Prevent speed from going below 1

			// Calculate player input movement
			Vector2 movement = _moveInput * adjustedSpeed * Time.fixedDeltaTime;

			// Automatic Drift
			if (_isDriftingLeft)
			{
				movement.x -= _leftDriftSpeed * Time.fixedDeltaTime;
			}
			else if (_isDriftingRight)
			{
				movement.x += _rightDriftSpeed * Time.fixedDeltaTime;
			}

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

		public void UpdateCarriedObjects()
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

		public void StartLeftDrift()
		{
			_isDriftingLeft = true;
			_isDriftingRight = false; // Stops right drift if active
		}

		public void StartRightDrift()
		{
			_isDriftingRight = true;
			_isDriftingLeft = false; // Stops left drift if active
		}

		public void StopDrift()
		{
			_isDriftingLeft = false;
			_isDriftingRight = false;
		}

		public void IncreaseDriftDifficulty(float driftIncrement)
		{
			_leftDriftSpeed += driftIncrement;
			_rightDriftSpeed += driftIncrement;
			Debug.Log($"Drift speeds increased! Left: {_leftDriftSpeed}, Right: {_rightDriftSpeed}");
		}

		#endregion

		//PickUp Objects
		#region PickUp Objects

		public void OnPickup(InputAction.CallbackContext context)
		{
			if (_carriedObjects.Count >= _maxCarriedObjects)
				return; // Prevent picking up more than max allowed

				PlayAudioCoinClip(0);

			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _pickupRadius);
			foreach (Collider2D col in colliders)
			{
				PickUpTime timePickup = col.GetComponent<PickUpTime>();
				if (timePickup != null)
				{
					timePickup.OnPickedUp();
				}

				PickUpScore scorePickup = col.GetComponent<PickUpScore>();
				if (scorePickup != null)
				{
					scorePickup.OnPickedUp();
				}

				if (col.CompareTag("PickUp") && !_carriedObjects.Contains(col.gameObject))
				{
					_carriedObjects.Add(col.gameObject);
					col.gameObject.transform.SetParent(transform);

					// Notify the spawner to free up the spawn point
					PickupTracker tracker = col.GetComponent<PickupTracker>();
					if (tracker != null)
					{
						tracker.NotifyPickup();
					}

					// Set the sorting order of the sprite to create a stacked visual effect
					SpriteRenderer spriteRenderer = col.GetComponent<SpriteRenderer>();
					if (spriteRenderer != null)
					{
						spriteRenderer.sortingOrder = _carriedObjects.Count;
					}
					UpdateCoinVisuals();

					break;
				}
			}
		}

		public List<GameObject> GetCarriedObjects()
		{
			return _carriedObjects; // Returns the list of carried objects
		}

		public void DropAllPickups()
		{
			foreach (GameObject obj in _carriedObjects)
			{
				if (obj != null)
				{
					obj.transform.SetParent(null);
					obj.SetActive(false); // Or just drop it in place if you want to reuse them
				}
			}
			_carriedObjects.Clear();
			UpdateCoinVisuals();
		}

		private void UpdateCoinVisuals()
		{
			for (int i = 0; i < _coinVisuals.Count; i++)
			{
				_coinVisuals[i].SetActive(i == _carriedObjects.Count - 1);
			}
		}

		public void TriggerPickup()
		{
			// Simulate a pickup without using the Input System context
			if (_carriedObjects.Count >= _maxCarriedObjects)
				return;

			PlayAudioCoinClip(0);

			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _pickupRadius);
			foreach (Collider2D col in colliders)
			{
				PickUpTime timePickup = col.GetComponent<PickUpTime>();
				if (timePickup != null)
				{
					timePickup.OnPickedUp();
				}

				PickUpScore scorePickup = col.GetComponent<PickUpScore>();
				if (scorePickup != null)
				{
					scorePickup.OnPickedUp();
				}

				if (col.CompareTag("PickUp") && !_carriedObjects.Contains(col.gameObject))
				{
					_carriedObjects.Add(col.gameObject);
					col.gameObject.transform.SetParent(transform);

					PickupTracker tracker = col.GetComponent<PickupTracker>();
					if (tracker != null)
					{
						tracker.NotifyPickup();
					}

					SpriteRenderer spriteRenderer = col.GetComponent<SpriteRenderer>();
					if (spriteRenderer != null)
					{
						spriteRenderer.sortingOrder = _carriedObjects.Count;
					}

					UpdateCoinVisuals();
					break;
				}
			}
		}

		#endregion

		//Stun Player
		#region Stun
		public void StunPlayer(float duration)
		{
			_isStunned = true;
			_stunEndTime = Time.time + duration; // Calculate when the stun ends
			Debug.Log("Player stunned for " + duration + " seconds.");
		}
		#endregion

		//Take Damage
		#region Take Damage

		public void PlayerTakeDamage()
		{
			StartCoroutine(ToggleSpriteAndCollider(0.2f));
			ChangeAnimationState(_hitAnim);
			PlayAudioPlayerHitClip(0);
		}

		private IEnumerator ToggleSpriteAndCollider(float delay)
		{
			_collider2D.enabled = !_collider2D.enabled;
			yield return new WaitForSeconds(delay);

			float _toggleDuration = 2.0f;
			float _toggleInterval = 0.2f;

			float endTime = Time.time + _toggleDuration;

			while (Time.time < endTime)
			{
				_spriteRenderer.enabled = !_spriteRenderer.enabled;
				yield return new WaitForSeconds(_toggleInterval);
			}
			_spriteRenderer.enabled = true;
			_collider2D.enabled = true;
			ChangeAnimationState(_idleAnim);
		}
		#endregion

		//Animation
		#region Animation
		public void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}
		#endregion

		//Audio
		#region Audio

		public void PlayAudioPlayerHitClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _playerHitClip.Length)
			{
				_hitAudioSource.clip = _playerHitClip[clipIndex];
				_hitAudioSource.Play();
			}
		}

		public void PlayAudioCoinClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _coinClips.Length)
			{
				_coinAudioSource.clip = _coinClips[clipIndex];
				_coinAudioSource.Play();
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

