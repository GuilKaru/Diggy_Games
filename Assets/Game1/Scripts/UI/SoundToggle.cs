using UnityEngine;
using UnityEngine.UI;
namespace Diggy_MiniGame_1
{
	public class SoundToggle : MonoBehaviour
	{
		[SerializeField]
		private Sprite soundOnSprite; // Image for sound on
		[SerializeField]
		private Sprite soundOffSprite; // Image for sound off
		[SerializeField]
		private AudioSource backgroundMusic; // Reference to the AudioSource for background music

		private Image buttonImage;
		private bool isSoundOn = true;

		private void Awake()
		{
			buttonImage = GetComponent<Image>();
			buttonImage.sprite = soundOnSprite; // Set the initial image to sound on
		}

		public void ToggleSound()
		{
			isSoundOn = !isSoundOn;

			if (isSoundOn)
			{
				buttonImage.sprite = soundOnSprite;
				backgroundMusic.Play();
			}
			else
			{
				buttonImage.sprite = soundOffSprite;
				backgroundMusic.Pause();
			}
		}
	}
}

