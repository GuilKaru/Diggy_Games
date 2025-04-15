using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class WarningSpawner : MonoBehaviour
	{
		[SerializeField]
		private Transform _spawnPosition;  // The position to spawn the warning sign (set in Inspector)
		[SerializeField]
		private GameObject _warningPrefab;  // The prefab to spawn (set in Inspector)
		[SerializeField]
		private float _warningDuration = 5f;  // Duration for which the warning sign appears
		[SerializeField]
		private Transform _warningParent;

		private GameObject _currentWarning;  // Reference to the currently spawned warning sign


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
			if (_warningPrefab != null && _spawnPosition != null)
			{
				// Instantiate the warning sign at the specified position
				_currentWarning = Instantiate(_warningPrefab, _spawnPosition.position, Quaternion.identity, _warningParent);
				isWarningActive = true;

				// Start the duration timer to destroy the warning sign after the set time
				Invoke("DestroyWarningSign", _warningDuration);
			}
		}

		private void DestroyWarningSign()
		{
			if (_currentWarning != null)
			{
				Destroy(_currentWarning);
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


