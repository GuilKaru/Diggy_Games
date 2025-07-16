using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
namespace Diggy_MiniGame_3
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

		/*[Header("Mobile Joystick")]
		[SerializeField]
		private DynamicJoystick _joystick;
		[SerializeField]
		private Button _shootButton;*/

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

		[Header("Player Audio")]
		[SerializeField]
		private AudioSource _shovelAudioSource;
		[SerializeField]
		private AudioClip[] _shovelClips;
		[SerializeField]
		private AudioSource _hitAudioSource;
		[SerializeField]
		private AudioClip[] _playerHitClip;
		#endregion

		// Private Variables
		#region Private Variables
		private Vector2 _moveInput;
		private Rigidbody2D _rb;
		private PlayerInput _playerInput;
		private InputAction _moveAction;
		private InputAction _throwAction;
		private InputAction _switchSpriteAction;
		private float _originalMoveSpeed;
		private float _originalFireRate;

		private bool _isStunned = false; // Tracks if the player is stunned
		private float _stunEndTime = 0f; // Time when the stun effect ends

		private int _currentSpriteIndex = 0;

		private int _currentShootMode = 1;
		private Coroutine _shootingCoroutine;
		private bool _isShooting;

		private bool _isKnockedBack = false;
		private Vector3 _knockbackTargetPosition;
		private float _knockbackStartTime;

		private bool _isPointerOverUI = false;

		private RuntimeAnimatorController _originalAnimatorController;
		private Animator _originalAnimator;
		private Sprite _originalSprite;
		private Vector3 _originalScale;

		private bool _isUsingJoystick;
		#endregion

		//Animations
		#region Animations
		private string _currentState;
		private string _idleAnim = "Player_Idle";
		private string _walkingAnim = "Player_Walk";
		private string _shootStartAnim = "Player_Shoot_Start";
		private string _shootEndAnim = "Player_Shoot_End";
		private string _stunAnim = "Player_Stun";
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
			_throwAction = _playerInput.actions["ThrowShovel"];
			_originalMoveSpeed = _moveSpeed;
			_originalFireRate = _automaticFireRate;

			//_originalAnimator = _animator; // Store original animator
			//_originalSprite = _spriteRenderer.sprite; // Store original sprite
			//_originalAnimatorController = _animator.runtimeAnimatorController;
			_originalScale = transform.localScale;

			//_isUsingJoystick = _joystick != null && _joystick.InputVector.magnitude > 0.1f;

			//ChangeAnimationState(_idleAnim);

		}

		private void Update()
		{
			//_isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
			//_moveInput = _joystick.InputVector + _moveAction.ReadValue<Vector2>();
			_moveInput = Vector2.ClampMagnitude(_moveInput, 1f); // Normalize input

		}

		private void OnEnable()
		{
			_moveAction.performed += OnMoveInput;
			_moveAction.canceled += OnMoveInput;
			_throwAction.started += OnShootStart;
			_throwAction.canceled += OnShootStop;
		}

		private void OnDisable()
		{
			_moveAction.performed -= OnMoveInput;
			_moveAction.canceled -= OnMoveInput;	
			_throwAction.started -= OnShootStart;
			_throwAction.canceled -= OnShootStop;
		}

		#endregion

		// Movement
		#region Movement

		private void FixedUpdate()
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;

			// Handle Stun Logic
			if (_isStunned && Time.time >= _stunEndTime)
			{
				_isStunned = false; // End the stun effect
				Debug.Log("Player is no longer stunned.");
			}

			// Allow movement only if the player is not stunned knocked back
			if (!_isStunned && !_isKnockedBack)
			{
				MovePlayer();
				//HandleAnimationState();
			}
			else if (_isKnockedBack)
			{
				//ApplyKnockback(); // Apply knockback movement
			}
			else
			{
				_moveInput = Vector2.zero; // Block movement input while stunned
			}
		}

		private void OnMoveInput(InputAction.CallbackContext context)
		{
			_moveInput = context.ReadValue<Vector2>();
			//PlayAudioWalkClip(0);
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

		public void SetSpeedMultiplier(float multiplier)
		{
			_moveSpeed *= multiplier;
		}

		public void ResetSpeedMultiplier()
		{
			_moveSpeed = _originalMoveSpeed;
		}
		#endregion

		//Throw Shovel
		#region Throw Shovel
		private void OnShootStart(InputAction.CallbackContext context)
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;

			if (_isPointerOverUI) return;
			_isShooting = true;

			{
				_shootingCoroutine = StartCoroutine(ShootingCoroutine());

			}
		}

		private void OnShootStop(InputAction.CallbackContext context)
		{
			if (_shootingCoroutine != null)
			{
				StopCoroutine(_shootingCoroutine);
				_shootingCoroutine = null;
				_isShooting = false;

				// Decide next animation based on movement
				if (_moveInput != Vector2.zero)
				{
					//ChangeAnimationState(_walkingAnim);
				}
				else
				{
					//StartCoroutine(WaitForShootEndAnimation()); // Small delay for smooth transition
				}
			}
		}

		private IEnumerator ShootingCoroutine()
		{
			float initialDelay = 0; // Adjust this value based on your animation length
			yield return new WaitForSeconds(initialDelay);
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
			Quaternion _bulletRotation = Quaternion.Euler(0, 0, 0);
			GameObject bullet = Instantiate(_shovelPrefab, _shovelThrowTransform.position, _bulletRotation, _shovelParent);
			bullet.GetComponent<Collider2D>().enabled = true;
			PlayAudioShovelClip(0);
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
				Quaternion _bulletRotation = Quaternion.Euler(0, 0, 270);
				GameObject bullet = Instantiate(_shovelPrefab, _shovelThrowTransform.position, spreadRotation * _bulletRotation, _shovelParent);
				bullet.GetComponent<Collider2D>().enabled = true;
				Shovel bulletController = bullet.GetComponent<Shovel>();
				if (bulletController != null)
				{
					bulletController.target = _shovelThrowTransform.position + (spreadRotation * _shovelThrowTransform.up) * _shovelHitMissDistance;
				}
			}
		}

		private void InstantiateBullet(float angleOffset)
		{
			Quaternion _bulletRotation = Quaternion.Euler(0, 0, 270);
			GameObject bullet = Instantiate(_shovelPrefab, _shovelThrowTransform.position, _bulletRotation, _shovelParent);
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

		public void SetFireRateMultiplier(float multiplier)
		{
			_automaticFireRate *= multiplier;
		}

		public void ResetFireRateMultiplier()
		{
			_automaticFireRate = _originalFireRate;
		}

		#endregion

		//Mobile Shoot
		#region Mobile Shoot
		public void OnShootStart()
		{
			_isShooting = true;

			if (_shootingCoroutine == null)
			{
				_shootingCoroutine = StartCoroutine(ShootingCoroutine());
			}
		}

		public void OnShootStop()
		{
			_isShooting = false;

			if (_shootingCoroutine != null)
			{
				StopCoroutine(_shootingCoroutine);
				_shootingCoroutine = null;
			}
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

		//Player Animations
		#region Player Animations
		/*public void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}

		private void HandleAnimationState()
		{
			if (_isShooting)
			{
				// If the player is shooting, ensure shooting animations are playing
				if (_currentState != _shootStartAnim && _shootingCoroutine != null)
				{
					ChangeAnimationState(_shootStartAnim);
				}
			}
			else if (_moveInput != Vector2.zero)
			{
				// If the player is moving, play walking animation
				if (_currentState != _walkingAnim)
				{
					ChangeAnimationState(_walkingAnim);
				}

				if (!_walkAudioSource.isPlaying)
				{
					PlayAudioWalkClip(0);
				}
			}
			else
			{
				// If the player is idle, transition to idle animation
				if (_currentState != _idleAnim)
				{
					ChangeAnimationState(_idleAnim);
				}

				if (_walkAudioSource.isPlaying)
				{
					_walkAudioSource.Stop();
				}
			}
		}

		private void ResetToIdleAnimation()
		{
			if (!_isShooting && _moveInput == Vector2.zero)
			{
				ChangeAnimationState(_idleAnim);
			}
		}

		private IEnumerator WaitForShootEndAnimation()
		{
			// Assuming the animation length is 0.5 seconds (adjust to your actual duration)
			float shootEndDuration = 0.5f;

			yield return new WaitForSeconds(shootEndDuration);

			// Transition based on movement input
			if (_moveInput != Vector2.zero)
			{
				ChangeAnimationState(_walkingAnim);
			}
			else
			{
				ChangeAnimationState(_idleAnim);
			}
		}

		private IEnumerator WaitForStunAnimation(float duration)
		{
			yield return new WaitForSeconds(duration);

			_isStunned = false;

			// Decide next animation based on current input
			if (_moveInput != Vector2.zero)
			{
				ChangeAnimationState(_walkingAnim);
			}
			else if (_isShooting)
			{
				ChangeAnimationState(_shootStartAnim);
			}
			else
			{
				ChangeAnimationState(_idleAnim);
			}
		}


		public void SetBuffedAppearance(Sprite newSprite, RuntimeAnimatorController newAnimator)
		{
			_animator.runtimeAnimatorController = newAnimator;
			_spriteRenderer.sprite = newSprite;
			transform.localScale = _buffedScale;
		}

		public void ResetAppearance()
		{
			_isStunned = false;
			StopCoroutine(_shootingCoroutine);
			_animator.runtimeAnimatorController = _originalAnimatorController; // Reset to original animations
			_spriteRenderer.sprite = _originalSprite;
			transform.localScale = _originalScale;
		}

*/
		#endregion

		//Player Audio
		#region Player Audio

		private void PlayAudioShovelClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _shovelClips.Length)
			{
				_shovelAudioSource.clip = _shovelClips[clipIndex];
				_shovelAudioSource.Play();
			}
		}

		public void PlayAudioPlayerHitClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _playerHitClip.Length)
			{
				_hitAudioSource.clip = _playerHitClip[clipIndex];
				_hitAudioSource.Play();
			}
		}

		#endregion

	}
}

