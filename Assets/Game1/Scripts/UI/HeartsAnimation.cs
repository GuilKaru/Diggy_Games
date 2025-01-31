using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class HeartsAnimation : MonoBehaviour
	{
		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		[Header("Movement Settings")]
		[SerializeField]
		private float _amplitude = 0.1f; // Height of the oscillation
		[SerializeField]
		private float _frequency = 1f; // Speed of the oscillation

		private string _currentState;
		private string _idleAnim = "Hearts_Idle";
		private Vector3 _initialPosition;



		public void Awake()
		{
			_initialPosition = transform.localPosition;
			ChangeAnimationState(_idleAnim);
		}

		private void Update()
		{
			// Add subtle movement to the hearts on the Y axis
			float newY = _initialPosition.y + Mathf.Sin(Time.time * _frequency) * _amplitude;
			transform.localPosition = new Vector3(_initialPosition.x, newY, _initialPosition.z);
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

