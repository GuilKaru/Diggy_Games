using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickUpScore : MonoBehaviour
	{
		[SerializeField]
		private int _scoreValue = 10;

		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _idleAnim = "PickUpScore_Idle";

		private string _currentState;
		private ScoreManager _scoreManager;
		private Vector3 _initialPosition;
		private bool _isPickedUp = false;

		private void Awake()
		{
			_initialPosition = transform.localPosition;
			ChangeAnimationState(_idleAnim);
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_scoreManager = FindObjectOfType<ScoreManager>();
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
			if (_scoreManager != null)
			{
				_scoreManager.AddScore(_scoreValue);
				Debug.Log($"Score Added: {_scoreValue}");
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
