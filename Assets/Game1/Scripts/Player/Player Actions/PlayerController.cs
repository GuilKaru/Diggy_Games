using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using System.Collections.Generic;

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
		private Vector2 _minBounds; // Minimum X and Y values
		[SerializeField]
		private Vector2 _maxBounds; // Maximum X and Y values

		[Header("Shovel Throw")]
		[SerializeField]
		private GameObject _shovelPrefab;
		[SerializeField]
		private Transform _shovelThrowTransform;
		[SerializeField]
		private Transform _shovelParent;
		[SerializeField]
		private float _automaticFireRate = 0.1f;
		[SerializeField]
		private float _shovelHitMissDistance = 25f;

		[Header("Shotgun Settings")]
		[SerializeField]
		private int _shotgunPelletCount = 3;
		[SerializeField]
		private float[] _shotgunSpreadAngles = { -30f, 0f, 30f };

		[Header("Knockback")]
		[SerializeField]
		private float knockbackDistance = 1f;
		[SerializeField]
		private float knockbackDuration = 0.2f;

		[Header("Sprite Settings")]
		[SerializeField]
		private List<Sprite> _playerSprites; // List of sprites to switch between
		[SerializeField]
		private SpriteRenderer _spriteRenderer; // Reference to the player's SpriteRenderer
		#endregion

		// Private Variables
		#region Private Variables
		private Vector2 _moveInput;
		private Rigidbody2D _rb;
		private PlayerInput _playerInput;
		private InputAction _moveAction;
		private InputAction _throwAction;
		private InputAction _switchSpriteAction;

		private bool _isStunned = false; // Tracks if the player is stunned
		private float _stunEndTime = 0f; // Time when the stun effect ends

		private int _currentSpriteIndex = 0;

		private int _currentShootMode = 1;
		private Coroutine _shootingCoroutine;
		private bool _isShooting;

		private bool _isKnockedBack = false;
		private Vector3 _knockbackTargetPosition;
		private float _knockbackStartTime;
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

			// Ensure the SpriteRenderer is assigned
			if (_spriteRenderer == null)
			{
				_spriteRenderer = GetComponent<SpriteRenderer>();
				if (_spriteRenderer == null)
				{
					Debug.LogError("SpriteRenderer component is missing!");
				}
			}

			// Setup input actions
			_moveAction = _playerInput.actions["Move"];
			_throwAction = _playerInput.actions["ThrowShovel"];
			_switchSpriteAction = _playerInput.actions["SwitchSprite"];
		}

		private void OnEnable()
		{
			_moveAction.performed += OnMoveInput;
			_moveAction.canceled += OnMoveInput;
			_throwAction.started += OnShootStart;
			_throwAction.canceled += OnShootStop;
			_switchSpriteAction.started += OnSwitchSpriteStart;
		}

		private void OnDisable()
		{
			_moveAction.performed -= OnMoveInput;
			_moveAction.canceled -= OnMoveInput;
			_switchSpriteAction.started -= OnSwitchSpriteStart;
			_throwAction.started -= OnShootStart;
			_throwAction.canceled -= OnShootStop;
		}

		#endregion

		// Movement
		#region Movement

		private void FixedUpdate()
		{
			// Handle Stun Logic
			if (_isStunned && Time.time >= _stunEndTime)
			{
				_isStunned = false; // End the stun effect
				Debug.Log("Player is no longer stunned.");
			}

			// Allow movement and teleportation only if the player is not stunned or shielding
			if (!_isStunned && !_isKnockedBack)
			{
				MovePlayer();// Allow normal movement
			}
			else if (_isKnockedBack)
			{
				ApplyKnockback(); // Apply knockback movement
			}
			else
			{
				_moveInput = Vector2.zero; // Block movement input while stunned
			}
		}

		private void OnMoveInput(InputAction.CallbackContext context)
		{
				_moveInput = context.ReadValue<Vector2>();
		}

		private void MovePlayer()
		{
			// Calculate movement
			Vector2 movement = _moveInput * _moveSpeed * Time.fixedDeltaTime;

			// New position after movement
			Vector2 newPosition = _rb.position + movement;

			// Clamp position to stay within the bounds
			newPosition.x = Mathf.Clamp(newPosition.x, _minBounds.x, _maxBounds.x);
			newPosition.y = Mathf.Clamp(newPosition.y, _minBounds.y, _maxBounds.y);

			// Move the Rigidbody to the clamped position
			_rb.MovePosition(newPosition);

		}
		#endregion

		//Throw Shovel
		#region Throw Shovel
		private void OnShootStart(InputAction.CallbackContext context)
		{

			if (_shootingCoroutine == null)
			{
				_isShooting = true;
				_shootingCoroutine = StartCoroutine(ShootingCoroutine());

			}
		}

		private void OnShootStop(InputAction.CallbackContext context)
		{
			if (_shootingCoroutine != null)
			{
				StopCoroutine(_shootingCoroutine);
				_shootingCoroutine = null;
			}
		}

		private IEnumerator ShootingCoroutine()
		{
			while (_isShooting)
			{
				while (_isShooting)
				{
					if (_currentShootMode == 1)
					{
						AutomaticShoot();
					}
					else if (_currentShootMode == 2)
					{
						ShotgunShoot();
					}

					yield return new WaitForSeconds(_automaticFireRate);
				}
			}
		}

		private void AutomaticShoot()
		{
			GameObject bullet = Instantiate(_shovelPrefab, _shovelThrowTransform.position, Quaternion.identity, _shovelParent);
			Shovel bulletController = bullet.GetComponent<Shovel>();
			if (bulletController != null)
			{
				bulletController.target = _shovelThrowTransform.position + _shovelThrowTransform.up * _shovelHitMissDistance;
			}
		}

		private void ShotgunShoot()
		{
			for (int i = 0; i < _shotgunPelletCount; i++)
			{
				float spread = _shotgunSpreadAngles[i];
				Quaternion spreadRotation = Quaternion.Euler(0, 0, spread);

				GameObject bullet = Instantiate(_shovelPrefab, _shovelThrowTransform.position, spreadRotation * Quaternion.identity, _shovelParent);
				Shovel bulletController = bullet.GetComponent<Shovel>();
				if (bulletController != null)
				{
					bulletController.target = _shovelThrowTransform.position + (spreadRotation * _shovelThrowTransform.up) * _shovelHitMissDistance;
				}
			}
		}

		private void InstantiateBullet(float angleOffset)
		{
			GameObject bullet = Instantiate(_shovelPrefab, _shovelThrowTransform.position, Quaternion.identity, _shovelParent);
			Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * _shovelThrowTransform.up;
			bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * _shovelHitMissDistance;
		}

		public void ActivateShotgunBuff(float duration)
		{
			_currentShootMode = 2;
			Invoke(nameof(DeactivateShotgunBuff), duration);
		}

		private void DeactivateShotgunBuff()
		{
			_currentShootMode = 1;
		}

		#endregion

		//Stun
		#region Stun
		public void StunPlayer(float duration)
		{
			_isStunned = true;
			_stunEndTime = Time.time + duration; // Calculate when the stun ends
			Debug.Log("Player stunned for " + duration + " seconds.");
		}
		#endregion

		//Knockback
		#region Knockback
		private void ApplyKnockback()
		{
			// Calculate the interpolation factor
			float t = (Time.time - _knockbackStartTime) / knockbackDuration; // If the knockback duration is over, stop the knockback
			if (t >= 1.0f) { _isKnockedBack = false; t = 1.0f; } 
			// Interpolate the player's position
			transform.position = Vector3.Lerp(transform.position, _knockbackTargetPosition, t);
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			// Check if the collided object is a barrel
			if (collision.gameObject.CompareTag("Barrel"))
			{
				// Calculate the knockback target position (move left on x-axis)
				_knockbackTargetPosition = new Vector3(transform.position.x - knockbackDistance, transform.position.y, transform.position.z);
				// Set knockback start time 
				_knockbackStartTime = Time.time; _isKnockedBack = true; // Destroy the barrel
				Destroy(collision.gameObject);

			}

		}
		#endregion

		//Slow Effect
		#region Slow Effect
		public void ApplySlowEffect(float slowAmount, float duration)
		{
			StartCoroutine(SlowEffectCoroutine(slowAmount, duration));
		}

		private IEnumerator SlowEffectCoroutine(float slowAmount, float duration)
		{
			float originalSpeed = _moveSpeed;
			_moveSpeed *= slowAmount; // Reduce the movement speed
			Debug.Log($"Player slowed to {_moveSpeed} for {duration} seconds.");

			yield return new WaitForSeconds(duration);

			_moveSpeed = originalSpeed; // Restore original speed
			Debug.Log("Player speed restored.");
		}
		#endregion

		//Buffs Properties
		#region Buffs Properties
		public float MoveSpeed
		{
			get => _moveSpeed;
			set => _moveSpeed = value;
		}
		#endregion

		//Sprite Switching
		#region Sprite Switching
		private void OnSwitchSpriteStart(InputAction.CallbackContext context)
		{

			// Cycle to the next sprite
			_currentSpriteIndex = (_currentSpriteIndex + 1) % _playerSprites.Count;

			// Update the sprite
			_spriteRenderer.sprite = _playerSprites[_currentSpriteIndex];
			Debug.Log($"Switched to sprite {_currentSpriteIndex}");
		}
		#endregion
	}

}
