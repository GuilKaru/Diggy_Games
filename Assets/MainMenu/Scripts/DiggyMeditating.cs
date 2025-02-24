using UnityEngine;
namespace Diggy_MiniGame_1
{
	public class DiggyMeditating : MonoBehaviour
	{
		[Header("Movement Settings")]
		[SerializeField]
		private float _amplitude = 0.1f; // Height of the oscillation
		[SerializeField]
		private float _frequency = 1f; // Speed of the oscillation

		private Vector3 _initialPosition;


		private void Update()
		{
			// Add subtle movement to the hearts on the Y axis
			float newY = _initialPosition.y + Mathf.Sin(Time.time * _frequency) * _amplitude;
			transform.localPosition = new Vector3(_initialPosition.x, newY, _initialPosition.z);
		}

	}
}

