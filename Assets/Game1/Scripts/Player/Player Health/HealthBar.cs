using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Diggy_MiniGame_1
{
	public class HealthBar : MonoBehaviour
	{
		#region Serialize Fields
		[Header("Health Bar Images")]
		[SerializeField]
		private List<GameObject> _healthSegments; // List of health segment images
		#endregion

		// Methods
		#region Methods
		public void SetMaxHealth(int maxHealth)
		{
			if (_healthSegments == null || _healthSegments.Count == 0)
			{
				Debug.LogError("Health segments list is not assigned or empty.");
				return;
			}

			// Enable only the necessary number of health segments
			for (int i = 0; i < _healthSegments.Count; i++)
			{
				_healthSegments[i].SetActive(i < maxHealth);
			}
		}

	
		public void UpdateHealth(int currentHealth)
		{
			for (int i = 0; i < _healthSegments.Count; i++)
			{
				// Activate segments up to the current health, deactivate the rest
				_healthSegments[i].SetActive(i < currentHealth);
			}
		}

		#endregion
	}
}

