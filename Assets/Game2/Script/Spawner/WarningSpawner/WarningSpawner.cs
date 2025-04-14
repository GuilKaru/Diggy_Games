using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class WarningSpawner : MonoBehaviour
	{
		public Transform spawnPosition;  // The position to spawn the warning sign (set in Inspector)
		public GameObject warningPrefab;  // The prefab to spawn (set in Inspector)
		public float warningDuration = 5f;  // Duration for which the warning sign appears
		public Transform warningParent;
		private GameObject currentWarning;  // Reference to the currently spawned warning sign


		private bool isWarningActive = false;  // To toggle the warning on/off

		void OnTriggerEnter2D(Collider2D other)
		{
			// Check if the detected object is a barrel
			if (other.CompareTag("Barrel") && !isWarningActive)
			{
				SpawnWarningSign();
			}
		}

		private void SpawnWarningSign()
		{
			if (warningPrefab != null && spawnPosition != null)
			{
				// Instantiate the warning sign at the specified position
				currentWarning = Instantiate(warningPrefab, spawnPosition.position, Quaternion.identity, warningParent);
				isWarningActive = true;

				// Start the duration timer to destroy the warning sign after the set time
				Invoke("DestroyWarningSign", warningDuration);
			}
		}

		private void DestroyWarningSign()
		{
			if (currentWarning != null)
			{
				Destroy(currentWarning);
				isWarningActive = false;
			}
		}

		// Optionally toggle the warning on/off manually if needed
		public void ToggleWarning()
		{
			if (isWarningActive)
			{
				DestroyWarningSign();
			}
			else
			{
				SpawnWarningSign();
			}
		}
	}
}


