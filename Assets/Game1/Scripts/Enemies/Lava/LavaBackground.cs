using Org.BouncyCastle.Asn1.Mozilla;
using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class LavaBackground : MonoBehaviour
	{

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		private string _currentState;
		private string _idleAnim = "Lava_Idle";

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
