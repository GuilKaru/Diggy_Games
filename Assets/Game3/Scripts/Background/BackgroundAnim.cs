using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class BackgroundAnim : MonoBehaviour
	{
		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _currentState;
		private string _idleAnim = "BackLava_Idle";

		public void Awake()
		{
			ChangeAnimationState(_idleAnim);
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

