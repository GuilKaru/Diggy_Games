using UnityEngine;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Diggy_MiniGame_1;
namespace Diggy_MiniGame_2
{
	public class PremiumBuffManager : MonoBehaviour
	{
		//Serialize Fields
		#region Serialized Fields
		[SerializeField] private List<BuffData> _buffs = new List<BuffData>(); // List of all buffs

		[Header("Premium Buff Audio")]
		[SerializeField]
		private AudioSource _gameManagerAudioSource;
		[SerializeField]
		private AudioClip[] _gameManagerClips;

		[SerializeField]
		private AudioSource _shieldAudioSource;
		[SerializeField]
		private AudioClip[] _shieldBuffClips;

		[SerializeField]
		private AudioSource _timeStopAudioSource;
		[SerializeField]
		private AudioClip[] _timeStopBuffClips;

		[SerializeField]
		private AudioSource _blastBuffAudioSource;
		[SerializeField]
		private AudioClip[] _blastBuffClips;

		#endregion


		//Private Variables
		#region Private Variables
		private Dictionary<string, bool> _buffCooldownStates = new Dictionary<string, bool>(); // Tracks cooldown states for each buff
		private Dictionary<string, bool> _buffUnlockedStates = new Dictionary<string, bool>();
		private bool _isAnyBuffActive = false;
		#endregion

		//Initialization
		#region Initialization

		private void Start()
		{
			foreach (var buff in _buffs)
			{
				if (!_buffUnlockedStates.ContainsKey(buff.buffName))
				{
					//_buffUnlockedStates[buff.buffName] = false; // All buffs are initially locked
					if (buff.buffName == "DestroyChildrenBuff")
					{
						{
							_buffUnlockedStates[buff.buffName] = true;
						}
					}
					else if (buff.buffName == "StopEnemiesBuff")
					{
						
						{
							_buffUnlockedStates[buff.buffName] = true;
						}
						
					}
					else if (buff.buffName == "SpawnRockBuff")
					{
						
						{
							_buffUnlockedStates[buff.buffName] = true;
						}
					
					}
					else if (buff.buffName == "ShieldBuff")
					{
						
						{
							_buffUnlockedStates[buff.buffName] = true;
						}
						
					}
					else if (buff.buffName == "ShotgunBuff")
					{
						
						{
							_buffUnlockedStates[buff.buffName] = true;
						}
					
					}

				}

				if (!_buffCooldownStates.ContainsKey(buff.buffName))
				{
					_buffCooldownStates[buff.buffName] = false; // No buff is on cooldown initially
				}

				if (buff.buffButton != null)
				{
					buff.buffButton.SetActive(false); // Hide all buff buttons initially
				}
			}

			UpdateBuffButtonStates();
		}

		private void UpdateBuffButtonStates()
		{
			foreach (var buff in _buffs)
			{
				if (_buffUnlockedStates[buff.buffName])
				{
					buff.buffButton.SetActive(true); // Show button if buff is unlocked
				}
				else
				{
					buff.buffButton.SetActive(false); // Hide button if buff is not unlocked
				}
			}
		}

		public void UnlockBuff(string buffName)
		{
			if (_buffUnlockedStates.ContainsKey(buffName))
			{
				_buffUnlockedStates[buffName] = !_buffUnlockedStates[buffName]; // Unlock or lock the specific buff
				Debug.Log($"Buff '{buffName}' is now unlocked!");
				UpdateBuffButtonStates(); // Refresh button states
			}
			else
			{
				Debug.LogWarning($"Buff '{buffName}' not found!");
			}
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Y))
			{
				UnlockBuff("DestroyChildrenBuff"); // Unlock DestroyChildrenBuff
			}
			else if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				ActivateBuff("DestroyChildrenBuff");
			}

			if (Input.GetKeyDown(KeyCode.U))
			{
				UnlockBuff("SpawnRockBuff"); // Unlock SpawnRockBuff
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				ActivateBuff("SpawnRockBuff");
			}

			if (Input.GetKeyDown(KeyCode.I))
			{
				UnlockBuff("ShotgunBuff"); // Unlock ShotgunBuff
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				ActivateBuff("ShotgunBuff");
			}

			if (Input.GetKeyDown(KeyCode.O))
			{
				UnlockBuff("ShieldBuff"); // Unlock ShieldBuff
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				ActivateBuff("ShieldBuff");
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				UnlockBuff("StopEnemiesBuff"); // Unlock StopEnemiesBuff
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				ActivateBuff("StopEnemiesBuff");
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				ActivateBuff("DestroyChildrenBuff");
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				ActivateBuff("StopEnemiesBuff");
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				ActivateBuff("SpawnRockBuff");
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				ActivateBuff("ShieldBuff");
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

			if (!_buffUnlockedStates[buff.buffName])
			{
				Debug.Log($"Buff '{buffName}' is locked. Please unlock it first.");
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

			foreach (var b in _buffs)
			{
				if (b != buff)
				{
					if (b.buffButton != null)
					{
						b.buffButton.SetActive(false); // Disable other buffs
					}
					if (b.cooldownImage != null)
					{
						b.cooldownImage.fillAmount = 0f; // Set fill amount to zero
						b.cooldownImage.gameObject.SetActive(false); // Hide cooldown images for inactive buffs

					}
				}
			}



			switch (buff.buffName)
			{
				case "StopEnemiesBuff":
					StopEnemies(buff);
					PlayAudioGameManagerClip(0);
					break;

				case "DestroyChildrenBuff":
					foreach (var target in buff.targetGameObjects)
					{
						DestroyAllChildren(target);
						PlayAudioGameManagerClip(0);
					}
					break;

				case "SpawnRockBuff":
					SpawnRock(buff);
					PlayAudioGameManagerClip(0);
					break;

				case "ShieldBuff":
					foreach (var target in buff.targetGameObjects)
					{
						ActivateShield(target);
					}
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
			var enemies = FindObjectsOfType<MonoBehaviour>().Where(obj => obj is Barrel || obj is TransportZigzag || obj is TransportBomb).ToList();
			// Set the speed of each enemy to 0
			foreach (var enemy in enemies)
			{
				if (enemy is Barrel barrel)
				{
					barrel.SetSpeed(0); // Set speed to 0 for Barrel type
				}
				else if (enemy is TransportZigzag transportZigZag)
				{
					transportZigZag.SetSpeed(0);
				}
				else if (enemy is TransportBomb transportBomb)
				{
					transportBomb.SetSpeed(0);
				}


			}

			// Stop spawning
			var barrelSpawner = FindObjectOfType<BarrelSpawner>();
			if (barrelSpawner != null)
			{
				barrelSpawner.SetSpawning(false);
			}



			// Wait for the duration of the buff before restoring the speed and resuming spawning
			StartCoroutine(RestoreEnemiesAfterDelay(enemies, barrelSpawner, buff.cooldownTime));
			StartCoroutine(SoundQueue());
		}

		//Restore Enemies
		private IEnumerator RestoreEnemiesAfterDelay(List<MonoBehaviour> enemies, BarrelSpawner barrelSpawner, float delay)
		{
			yield return new WaitForSeconds(delay);


			// Restore speed of each enemy to its original speed
			foreach (var enemy in enemies)
			{
				if (enemy is Barrel barrel)
				{
					barrel.RestoreSpeed(); // Assuming OriginalSpeed is the default speed
				}
				else if (enemy is TransportZigzag transportZigZag)
				{
					transportZigZag.RestoreSpeed();
				}
				else if (enemy is TransportBomb transportBomb)
				{
					transportBomb.RestoreSpeed();
				}

			}

			// Resume spawning
			if (barrelSpawner != null)
			{
				barrelSpawner.SetSpawning(true);
			}
		}

		private IEnumerator SoundQueue()
		{
			yield return new WaitForSeconds(0.1f);
			PlayAudioTimeStopBuffClip(0);
		}
		#endregion

		//Blast Buff
		#region Blast Buff
		[Header("Blast Buff")]
		[SerializeField]
		private GameObject _destroyEffectPrefab; // Assign in Inspector
		[SerializeField]
		private Sprite[] _destroyEffectSprites;

		private void DestroyAllChildren(GameObject target)
		{
			if (target == null)
			{
				Debug.LogWarning("DestroyAllChildren: Target is null!");
				return;
			}

			if (target.transform.childCount == 0)
			{
				Debug.LogWarning($"DestroyAllChildren: '{target.name}' has no children.");
				return;
			}


			Debug.Log($"Destroying all children of '{target.name}'.");

			// Spawn effect BEFORE destroying children
			GameObject effect = Instantiate(_destroyEffectPrefab, target.transform.position, Quaternion.identity);
			// Start animation
			PlayAudioBlastBuffClip(0);
			StartCoroutine(PlayDestroyEffect(effect));

			// Destroy all children immediately
			for (int i = target.transform.childCount - 1; i >= 0; i--)
			{
				Destroy(target.transform.GetChild(i).gameObject);
			}
		}

		private IEnumerator PlayDestroyEffect(GameObject effect)
		{
			SpriteRenderer spriteRenderer = effect.GetComponent<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				Debug.LogWarning("No SpriteRenderer found on effect!");
				yield break;
			}

			// Cycle through effect sprites
			foreach (Sprite sprite in _destroyEffectSprites)
			{
				spriteRenderer.sprite = sprite;
				yield return new WaitForSeconds(0.1f);
			}

			Destroy(effect); // Remove effect after animation
		}
		#endregion

		//Rock Buff
		#region Rock Buff

		private void SpawnRock(BuffData buff)
		{
			if (buff.targetGameObjects == null || buff.targetGameObjects.Length == 0)
			{
				Debug.LogWarning("SpawnRock: No target GameObjects assigned for this buff!");
				return;
			}

			foreach (var target in buff.targetGameObjects)
			{
				if (target == null)
				{
					Debug.LogWarning("SpawnRock: Found a null GameObject in targetGameObjects!");
					continue;
				}

				RockSpawner spawner = target.GetComponent<RockSpawner>();
				if (spawner != null)
				{
					Debug.Log($"SpawnRock: Found RockSpawner on {target.name}. Spawning rocks...");
					spawner.SpawnRock(2);
				}
				else
				{
					Debug.LogWarning($"SpawnRock: No RockSpawner component found on {target.name}!");
				}
			}
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

			PlayerShield shield = player.GetComponentInChildren<PlayerShield>();
			if (shield == null)
			{
				Debug.LogWarning("ActivateShield: No Shield component found on the player!");
				return;
			}
			PlayAudioShieldBuffClip(0);
			shield.ActivateShield(3); // Shield can take 3 hits
			Debug.Log("Shield Buff activated: Shield absorbs 3 hits.");
		}
		#endregion

		// Starts the cooldown for a specific buff.
		private IEnumerator StartBuffCooldown(BuffData activeBuff)
		{
			_buffCooldownStates[activeBuff.buffName] = true;
			Debug.Log($"Buff '{activeBuff.buffName}' is on cooldown for {activeBuff.cooldownTime} seconds.");

			float elapsedTime = 0f;
			while (elapsedTime < activeBuff.cooldownTime)
			{
				elapsedTime += Time.deltaTime;
				float fillValue = elapsedTime / activeBuff.cooldownTime;
				if (activeBuff.cooldownImage != null)
				{
					activeBuff.cooldownImage.fillAmount = fillValue;
					activeBuff.cooldownImage.gameObject.SetActive(true); // Show cooldown image for active buff
				}
				yield return null;
			}

			_buffCooldownStates[activeBuff.buffName] = false;
			_isAnyBuffActive = false;
			Debug.Log($"Cooldown finished for Buff: {activeBuff.buffName}");

			// Re-enable buttons for unlocked buffs only\
			foreach (var b in _buffs)
			{
				if (b.buffButton != null) { b.buffButton.SetActive(true); }
				if (b.cooldownImage != null)
				{
					b.cooldownImage.gameObject.SetActive(true); // Re-enable cooldown images after cooldown ends
					b.cooldownImage.fillAmount = 1f;
				}
			}

			UpdateBuffButtonStates();

		}
		#endregion

		//Buff Audio
		#region Buff Audio

		private void PlayAudioShieldBuffClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _shieldBuffClips.Length)
			{
				_shieldAudioSource.clip = _shieldBuffClips[clipIndex];
				_shieldAudioSource.Play();
			}
		}


		private void PlayAudioTimeStopBuffClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _timeStopBuffClips.Length)
			{
				_timeStopAudioSource.clip = _timeStopBuffClips[clipIndex];
				_timeStopAudioSource.Play();
			}
		}

		private void PlayAudioBlastBuffClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _blastBuffClips.Length)
			{
				_blastBuffAudioSource.clip = _blastBuffClips[clipIndex];
				_blastBuffAudioSource.Play();
			}
		}

		private void PlayAudioGameManagerClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _gameManagerClips.Length)
			{
				_gameManagerAudioSource.clip = _gameManagerClips[clipIndex];
				_gameManagerAudioSource.Play();
			}
		}
		#endregion

		//Buff Usages with Backend
		#region Buff Usages with Backend
		/*
				private void LowerBuffUsage(BuffData buff)
				{
					if (buff.buffName == "DestroyChildrenBuff")
					{
						buff.textMeshProUGUI.text = (MainMenu.GameManager.instance.playerData.sweepBuffI - 1).ToString();
						if ((MainMenu.GameManager.instance.playerData.sweepBuffI - 1) <= 0)
						{
							//Logic to block sweepBuff
							UnlockBuff(buff.buffName);
						}
						MainMenu.GameManager.instance.boomBuffDecrease.ActionHandler("decrease_sweep");
					}
					else if (buff.buffName == "StopEnemiesBuff")
					{
						buff.textMeshProUGUI.text = (MainMenu.GameManager.instance.playerData.timeBuffI - 1).ToString();
						if ((MainMenu.GameManager.instance.playerData.timeBuffI - 1) <= 0)
						{
							//Logic to block sweepBuff
							UnlockBuff(buff.buffName);
						}
						MainMenu.GameManager.instance.boomBuffDecrease.ActionHandler("decrease_time");
					}
					else if (buff.buffName == "SpawnRockBuff")
					{
						buff.textMeshProUGUI.text = (MainMenu.GameManager.instance.playerData.rockBuffI - 1).ToString();
						if ((MainMenu.GameManager.instance.playerData.rockBuffI - 1) <= 0)
						{
							//Logic to block sweepBuff
							UnlockBuff(buff.buffName);
						}
						MainMenu.GameManager.instance.boomBuffDecrease.ActionHandler("decrease_rock");
					}
					else if (buff.buffName == "ShieldBuff")
					{
						buff.textMeshProUGUI.text = (MainMenu.GameManager.instance.playerData.shieldBuffI - 1).ToString();
						if ((MainMenu.GameManager.instance.playerData.shieldBuffI - 1) <= 0)
						{
							//Logic to block sweepBuff
							UnlockBuff(buff.buffName);
						}
						MainMenu.GameManager.instance.boomBuffDecrease.ActionHandler("decrease_shield");
					}
					else if (buff.buffName == "ShotgunBuff")
					{
						buff.textMeshProUGUI.text = (MainMenu.GameManager.instance.playerData.tripleBuffI - 1).ToString();
						if ((MainMenu.GameManager.instance.playerData.tripleBuffI - 1) <= 0)
						{
							//Logic to block sweepBuff
							UnlockBuff(buff.buffName);
						}
						MainMenu.GameManager.instance.boomBuffDecrease.ActionHandler("decrease_triple");
					}
				}*/
		#endregion
	}

	[System.Serializable]
	public class BuffData
	{
		public string buffName; // Name of the buff
		public GameObject[] targetGameObjects; // The GameObject affected by the buff
		public float cooldownTime; // Cooldown time for the buff
		public Image cooldownImage;
		public TextMeshProUGUI textMeshProUGUI;
		public GameObject buffButton;

	}
}


