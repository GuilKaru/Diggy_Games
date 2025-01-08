using UnityEngine;
using System.Collections;
namespace Diggy_MiniGame_1
{
	public class BuffManager : MonoBehaviour
	{
		// Buff Settings
		#region Buff Settings
		[Header("Speed Buff")]
		[SerializeField]
		private float _speedBuffDuration = 5f;
		[SerializeField]
		private float _speedMultiplier = 2f;

		// Buff State
		private bool _isSpeedBuffActive = false;

		private Coroutine _speedBuffCoroutine;
		#endregion

		// Player Reference
		#region Player Reference
		[Header("Player Reference")]
		[SerializeField]
		private PlayerController _playerController; // Reference to the PlayerController
		#endregion

		// Public Methods
		#region Public Methods
		public void ActivateSpeedBuff()
		{
			if (_isSpeedBuffActive) return; // Prevent multiple activations

			_isSpeedBuffActive = true;
			_playerController.MoveSpeed *= _speedMultiplier; // Increase the player's speed

			if (_speedBuffCoroutine != null) StopCoroutine(_speedBuffCoroutine);
			_speedBuffCoroutine = StartCoroutine(SpeedBuffTimer());
		}
		#endregion

		// Private Methods
		#region Private Methods
		private IEnumerator SpeedBuffTimer()
		{
			yield return new WaitForSeconds(_speedBuffDuration);

			_isSpeedBuffActive = false;
			_playerController.MoveSpeed /= _speedMultiplier; // Reset the player's speed
			Debug.Log("Speed Buff Deactivated!");
		}
		#endregion
	}
}

