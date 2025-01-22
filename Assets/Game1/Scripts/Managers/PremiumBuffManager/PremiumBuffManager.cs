using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Diggy_MiniGame_1
{
	public class PremiumBuffManager : MonoBehaviour
	{
		//Serialize Fields
		#region Serialized Fields
		[SerializeField] private List<BuffData> _buffs = new List<BuffData>(); // List of all buffs
		#endregion

		//Private Variables
		#region Private Variables
		private Dictionary<string, bool> _buffCooldownStates = new Dictionary<string, bool>(); // Tracks cooldown states for each buff
		private bool _isAnyBuffActive = false;
		#endregion

		//Initialization
		#region Initialization

		private void Start()
		{
			// Initialize cooldown states for all buffs
			foreach (var buff in _buffs)
			{
				_buffCooldownStates[buff.buffName] = false; // No buff is on cooldown initially
			}
		}

		#endregion

		//Buff Active
		#region Buff Active
		// Activates a specific buff by name
		public void ActivateBuff(string buffName)
		{
			if (_isAnyBuffActive)
			{
				Debug.Log("Another buff is currently active. Please wait for it to finish.");
				return;
			}

			// Find the buff by name
			BuffData buff = _buffs.Find(b => b.buffName == buffName);

			if (buff == null)
			{
				Debug.LogWarning($"Buff '{buffName}' not found!");
				return;
			}

			// Check if the buff is on cooldown
			if (_buffCooldownStates[buffName])
			{
				Debug.Log($"Buff '{buffName}' is on cooldown. Please wait.");
				return;
			}

			// Apply the buff logic
			Debug.Log($"Activating Buff: {buff.buffName}");
			ApplyBuffEffect(buff);

			// Start cooldown
			StartCoroutine(StartBuffCooldown(buff));
		}

		#endregion

		//Buff Logic
		#region Buff Logic

		//Applying the buff
		private void ApplyBuffEffect(BuffData buff)
		{
			_isAnyBuffActive = true;

			switch (buff.buffName)
			{
				case "StopEnemiesBuff":
					StopEnemies(buff);
					break;

				case "DestroyChildrenBuff":
					DestroyAllChildren(buff.targetGameObject);
					break;

				case "SpawnRockBuff":
					SpawnRock(buff);
					break;

				case "ShotgunBuff":
					ActivateShotgun(buff.targetGameObject);
					break;

				case "ShieldBuff":
					ActivateShield(buff.targetGameObject);
					break;

				default:
					Debug.LogWarning($"No effect implemented for buff '{buff.buffName}'.");
					break;
			}
		}

		//Stop Enemies Buff
		#region Stop Enemies Buff

		private void StopEnemies(BuffData buff)
		{
			// Find all enemies with relevant scripts
			var enemies = FindObjectsOfType<MonoBehaviour>().Where(obj => obj is Barrel || obj is TarBarrel || obj is BarrelTNT || obj is BarrelPush).ToList();

			// Set the speed of each enemy to 0
			foreach (var enemy in enemies)
			{
				if (enemy is Barrel barrel)
				{
					barrel.SetSpeed(0); // Set speed to 0 for Barrel type
				}
				else if (enemy is TarBarrel tarBarrel)
				{
					tarBarrel.SetSpeed(0); // Set speed to 0 for TarBarrel type
				}
				else if (enemy is BarrelTNT barrelTnt)
				{
					barrelTnt.SetSpeed(0); // Set speed to 0 for BarrelTnt type
				}
				else if (enemy is BarrelPush barrelPush)
				{
					barrelPush.SetSpeed(0); // Set speed to 0 for BarrelPush type
				}
			}

			// Stop spawning
			var enemySpawner = FindObjectOfType<EnemySpawner>();
			if (enemySpawner != null)
			{
				enemySpawner.SetSpawning(false);
			}

			// Wait for the duration of the buff before restoring the speed and resuming spawning
			StartCoroutine(RestoreEnemiesAfterDelay(enemies, enemySpawner, buff.cooldownTime));
		}

		//Restore Enemies
		private IEnumerator RestoreEnemiesAfterDelay(List<MonoBehaviour> enemies, EnemySpawner enemySpawner, float delay)
		{
			yield return new WaitForSeconds(delay);

			// Restore speed of each enemy to its original speed
			foreach (var enemy in enemies)
			{
				if (enemy is Barrel barrel)
				{
					barrel.RestoreSpeed(); // Assuming OriginalSpeed is the default speed
				}
				else if (enemy is TarBarrel tarBarrel)
				{
					tarBarrel.RestoreSpeed();
				}
				else if (enemy is BarrelTNT barrelTnt)
				{
					barrelTnt.RestoreSpeed();
				}
				else if (enemy is BarrelPush barrelPush)
				{
					barrelPush.RestoreSpeed();
				}
			}

			// Resume spawning
			if (enemySpawner != null)
			{
				enemySpawner.SetSpawning(true);
			}
		}
		#endregion

		//Blast Buff
		#region Blast Buff

		private void DestroyAllChildren(GameObject target)
		{
			if (target == null)
			{
				Debug.LogWarning("DestroyAllChildren: Target GameObject is null! Cannot destroy children.");
				return;
			}

			if (target.transform.childCount == 0)
			{
				Debug.LogWarning($"DestroyAllChildren: Target GameObject '{target.name}' has no children to destroy.");
				return;
			}

			Debug.Log($"DestroyAllChildren: Destroying all children of '{target.name}'.");

			for (int i = target.transform.childCount - 1; i >= 0; i--) // Reverse loop
			{
				Transform child = target.transform.GetChild(i);
				Destroy(child.gameObject);
			}
		}
		#endregion

		//Rock Buff
		#region Rock Buff

		private void SpawnRock(BuffData buff)
		{
			RockSpawner spawner = buff.targetGameObject.GetComponent<RockSpawner>();
			if (spawner == null)
			{
				Debug.LogWarning("No RockSpawner component found on the target GameObject!");
				return;
			}

			spawner.SpawnRock();
		}
		#endregion

		//Shotgun Buff
		#region Shotgun Buff

		private void ActivateShotgun(GameObject player)
		{
			if (player == null)
			{
				Debug.LogWarning("ActivateShotgun: Target player is null!");
				return;
			}

			PlayerController playerController = player.GetComponent<PlayerController>();
			if (playerController == null)
			{
				Debug.LogWarning("ActivateShotgun: Player does not have a PlayerController component!");
				return;
			}

			playerController.ActivateShotgunBuff(10f); // Enable shotgun for 10 seconds
			Debug.Log("Shotgun Buff activated: Player will shoot 3 bullets for 10 seconds.");
		}
		#endregion

		//Shield Buff
		#region Shield Buff

		private void ActivateShield(GameObject player)
		{
			if (player == null)
			{
				Debug.LogWarning("ActivateShield: Target player is null!");
				return;
			}

			Shield shield = player.GetComponentInChildren<Shield>();
			if (shield == null)
			{
				Debug.LogWarning("ActivateShield: No Shield component found on the player!");
				return;
			}

			shield.ActivateShield(3); // Shield can take 3 hits
			Debug.Log("Shield Buff activated: Shield absorbs 3 hits.");
		}
		#endregion

		// Starts the cooldown for a specific buff.
		private IEnumerator StartBuffCooldown(BuffData buff)
		{
			_buffCooldownStates[buff.buffName] = true; // Set buff on cooldown
			Debug.Log($"Buff '{buff.buffName}' is on cooldown for {buff.cooldownTime} seconds.");

			yield return new WaitForSeconds(buff.cooldownTime);

			_buffCooldownStates[buff.buffName] = false; // Reset cooldown
			Debug.Log($"Buff '{buff.buffName}' is ready to use again.");
			_isAnyBuffActive = false;
		}

		#endregion
	}

	[System.Serializable]
	public class BuffData
	{
		public string buffName; // Name of the buff
		public GameObject targetGameObject; // The GameObject affected by the buff
		public float cooldownTime; // Cooldown time for the buff
	}
}



