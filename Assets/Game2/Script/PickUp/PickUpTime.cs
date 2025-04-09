using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickUpTime : MonoBehaviour
	{
		[SerializeField] private float _bonusTime = 10f;
		private TimerManager _timerManager;

		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _idleAnim = "PickUpTime_Idle";

		private string _currentState;

		private Vector3 _initialPosition;
		private bool _isPickedUp = false;

		private void Awake()
		{
			_initialPosition = transform.localPosition;
			ChangeAnimationState(_idleAnim);
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_timerManager = FindObjectOfType<TimerManager>();
		}

		private void Update()
		{
			if (_isPickedUp) return;
			
		}

		public void OnPickedUp()
		{
			_isPickedUp = true;
			_spriteRenderer.enabled = false;
		}

		public void HandleDrop()
		{
			if (_timerManager != null)
			{
				_timerManager.AddTime(_bonusTime);
				Debug.Log($"Time Added: {_bonusTime} seconds!");
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

