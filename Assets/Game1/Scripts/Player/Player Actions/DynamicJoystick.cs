using UnityEngine;
using UnityEngine.EventSystems;
namespace Diggy_MiniGame_1
{
	public class DynamicJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
	{
		public RectTransform joystickBackground;
		public RectTransform joystickKnob;
		public float maxJoystickDistance = 100f;

		private Vector2 _inputVector = Vector2.zero;

		public Vector2 InputVector => _inputVector;

		public void OnPointerDown(PointerEventData eventData)
		{
			joystickBackground.position = eventData.position; // Move joystick to touch position
			joystickBackground.gameObject.SetActive(true);
			joystickKnob.position = eventData.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			Vector2 direction = eventData.position - (Vector2)joystickBackground.position;
			float distance = Mathf.Clamp(direction.magnitude, 0, maxJoystickDistance);
			_inputVector = direction.normalized;

			joystickKnob.position = (Vector2)joystickBackground.position + (_inputVector * distance);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_inputVector = Vector2.zero;
			joystickKnob.localPosition = Vector2.zero; // Hide joystick when touch is lifted
		}
	}
}

