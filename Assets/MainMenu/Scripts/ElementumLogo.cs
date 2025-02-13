using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class ElementumLogo : MonoBehaviour
	{
		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _currentState;
		private string _startAnim = "Elementum_Idle";

		private void Awake()
		{
			_animator = GetComponent<Animator>();

			ChangeAnimationState("Elementum_Idle");
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
