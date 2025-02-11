using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
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


		[Header("Game Manager Audio")]
		[SerializeField]
		private AudioSource _gameManagerAudioSource;
		[SerializeField]
		private AudioClip[] _gameManagerClips;

		[Header("Premium Buff Audio")]
		[SerializeField]
		private AudioSource _blastBuffAudioSource;
		[SerializeField]
		private AudioClip[] _blastBuffClips;
		[SerializeField]
		private AudioSource _shotgunAudioSource;
		[SerializeField]
		private AudioClip[] _shotgunBuffClips;

		[SerializeField]
		private AudioSource _shieldAudioSource;
		[SerializeField]
		private AudioClip[] _shieldBuffClips;

		[SerializeField]
		private AudioSource _timeStopAudioSource;
		[SerializeField]
		private AudioClip[] _timeStopBuffClips;
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


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				ActivateBuff("DestroyChildrenBuff");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				ActivateBuff("SpawnRockBuff");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				ActivateBuff("ShotgunBuff");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				ActivateBuff("ShieldBuff");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				ActivateBuff("StopEnemiesBuff");
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
					DestroyAllChildren(buff.targetGameObject);
					PlayAudioGameManagerClip(0);
					break;

				case "SpawnRockBuff":
					SpawnRock(buff);
					PlayAudioGameManagerClip(0);
					break;

				case "ShotgunBuff":
					ActivateShotgun(buff.targetGameObject);
					PlayAudioGameManagerClip(0);
					break;

				case "ShieldBuff":
					ActivateShield(buff.targetGameObject);
					//PlayAudioGameManagerClip(0);
					break;

				default:
					Debug.LogWarning($"No effect implemented for buff '{buff.buffName}'.");
					break;
			}
		}

		//Stop Enemies Buff
		#region Stop Enemies Buff
		[Header("Stop Enemies Buff")]
		[SerializeField]
		private AudioSource _backgroundMusicSource;
		[SerializeField]
		private AudioSource _lavaBgSource;
		[SerializeField]
		private Animator _objectAnimator;
		private void StopEnemies(BuffData buff)
		{
			// Pause Background Music
			if (_backgroundMusicSource != null)
			{
				_backgroundMusicSource.Pause();
			}

			if (_lavaBgSource != null)
			{
				_lavaBgSource.Pause();
			}

			// Pause Animation
			if (_objectAnimator != null)
			{
				_objectAnimator.speed = 0; // Pause animation
			}
			StartCoroutine(SoundQueue());

			// Find all enemies with relevant scripts
			var enemies = FindObjectsOfType<MonoBehaviour>().Where(obj => obj is Barrel || obj is TarBarrel || obj is BarrelTNT || obj is BarrelPush || obj is LavaDrop).ToList();
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
				else if (enemy is LavaDrop lavaDrop)
				{
					lavaDrop.SetSpeed(0); // Set speed to 0 for BarrelPush type
				}
			}

			// Stop spawning
			var enemySpawner = FindObjectOfType<EnemySpawner>();
			if (enemySpawner != null)
			{
				enemySpawner.SetSpawning(false);
			}

			var lavaDropSpawner = FindObjectOfType<LavaDropSpawner>();
			if (lavaDropSpawner != null)
			{
				lavaDropSpawner.SetSpawning(false);
			}

			// Wait for the duration of the buff before restoring the speed and resuming spawning
			StartCoroutine(RestoreEnemiesAfterDelay(enemies, enemySpawner, lavaDropSpawner, buff.cooldownTime));
		}

		private IEnumerator SoundQueue()
		{
			yield return new WaitForSeconds(0.1f);
			PlayAudioTimeStopBuffClip(0);
		}

		//Restore Enemies
		private IEnumerator RestoreEnemiesAfterDelay(List<MonoBehaviour> enemies, EnemySpawner enemySpawner, LavaDropSpawner lavaDropSpawner, float delay)
		{
			yield return new WaitForSeconds(delay);

			// Restore Background Music
			if (_backgroundMusicSource != null)
			{
				_backgroundMusicSource.UnPause();
			}

			if (_lavaBgSource != null)
			{
				_lavaBgSource.UnPause();
			}

			// Restore Animation
			if (_objectAnimator != null)
			{
				_objectAnimator.speed = 1; // Resume animation
			}

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
				else if (enemy is LavaDrop lavaDrop)
				{
					lavaDrop.RestoreSpeed(); // Set speed to 0 for BarrelPush type
				}
			}

			if (lavaDropSpawner != null)
			{
				lavaDropSpawner.SetSpawning(true);
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
			PlayAudioBlastBuffClip(0);
			// Start animation
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
			RockSpawner spawner = buff.targetGameObject.GetComponent<RockSpawner>();
			if (spawner == null)
			{
				Debug.LogWarning("No RockSpawner component found on the target GameObject!");
				return;
			}

			spawner.SpawnRock(2);

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
			PlayAudioShieldBuffClip(0);
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

			foreach (var b in _buffs)
			{
				if (b.buffButton != null)
				{
					b.buffButton.SetActive(true);
				}
				if (b.cooldownImage != null)
				{
					b.cooldownImage.gameObject.SetActive(true); // Re-enable cooldown images after cooldown ends
					b.cooldownImage.fillAmount = 1f;
				}
			}
			Debug.Log("All buffs are now available again!");
		}

		#endregion

		//Buff Audio
		#region Buff Audio

		private void PlayAudioBlastBuffClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _blastBuffClips.Length)
			{
				_blastBuffAudioSource.clip = _blastBuffClips[clipIndex];
				_blastBuffAudioSource.Play();
			}
		}

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

		private void PlayAudioGameManagerClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _gameManagerClips.Length)
			{
				_gameManagerAudioSource.clip = _gameManagerClips[clipIndex];
				_gameManagerAudioSource.Play();
			}
		}



		#endregion
	}

	[System.Serializable]
	public class BuffData
	{
		public string buffName; // Name of the buff
		public GameObject targetGameObject; // The GameObject affected by the buff
		public float cooldownTime; // Cooldown time for the buff
		public Image cooldownImage;
		public GameObject buffButton;
		
	}
}



