using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Diggy_MiniGame_1
{
	public class InfluencerManager : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Influencer Buff Settings")]
		[SerializeField]
		private float _initialCooldown = 6f; // 6-second startup cooldown
		[SerializeField]
		private float _buffDuration = 5f; // Duration of the effect
		[SerializeField]
		private float _speedMultiplier = 1.5f; // 50% increase in speed
		[SerializeField]
		private float _scoreMultiplier = 2f; // x2 score
		[SerializeField]
		private float _fireRateMultiplier = 0.5f;

		[Header("Influencer Manager Audio")]
		[SerializeField]
		private AudioSource _influencerManagerAudioSource;
		[SerializeField]
		private AudioClip[] _influencerManagerClips;

		[Header("UI Elements")]
		[SerializeField]
		private GameObject _influencerMenu;
		[SerializeField]
		private Button[] _buffButtons;

		[Header("Influencer Cooldown UI")]
		[SerializeField]
		private Button _influencerCooldownButton;
		[SerializeField]
		private Image _cooldownFillImage;

		[Header("Appearance Options")]
		[SerializeField]
		private Sprite[] _buffSprites;
		[SerializeField]
		private RuntimeAnimatorController[] _buffAnimators;

		public bool IsInfluencerMenuOpen => _influencerMenu.activeSelf;

		#endregion

		//Private Variables
		#region Private Variables

		private bool _isCooldown = true;
		private bool _isBuffActive = false;
		private PlayerController _player;
		private ScoreManager _scoreManager;
		#endregion

		//Initialization
		#region Initialization

		private void Start()
		{
			_player = FindObjectOfType<PlayerController>();
			_scoreManager = FindObjectOfType<ScoreManager>();
			StartCoroutine(StartupCooldown());

			_influencerMenu.SetActive(false);

			for (int i = 0; i < _buffButtons.Length; i++)
			{
				int index = i; // Capture index for lambda expression
				_buffButtons[i].onClick.AddListener(() => ActivateInfluencerBuff(index));
			}
		}

		private void Update()
		{
			/*if (Input.GetKeyDown(KeyCode.Alpha6) && !_isCooldown && !_isBuffActive)
			{
				OpenMenu();
			}*/
		}

		public void OpenMenu()
		{
			_influencerMenu.SetActive(true);
			Time.timeScale = 0f; // Pause game
		}

		private void CloseMenu()
		{
			_influencerMenu.SetActive(false);
			Time.timeScale = 1f; // Resume game
		}

		private IEnumerator StartupCooldown()
		{
			yield return new WaitForSeconds(_initialCooldown);
			_isCooldown = false;
		}
		#endregion

		//Buff Activation
		#region Buff Activation

		private void ActivateInfluencerBuff(int appearanceIndex)
		{
			CloseMenu();
			_cooldownFillImage.fillAmount = 0f;
			PlayAudioInfluencerManagerClip(0);

			StartCoroutine(ApplySpeedBuff());
			StartCoroutine(ApplyScoreBuff());
			StartCoroutine(ApplyFireRateBuff());
			StartCoroutine(ApplyAppearanceBuff(appearanceIndex));
		}

		private IEnumerator ApplySpeedBuff()
		{
			_isBuffActive = true;
			_player.SetSpeedMultiplier(_speedMultiplier);
			yield return new WaitForSeconds(_buffDuration);
			_player.ResetSpeedMultiplier();
			StartCoroutine(BuffCooldown());
		}

		private IEnumerator ApplyScoreBuff()
		{
			_isBuffActive = true;
			_scoreManager.SetScoreMultiplier(_scoreMultiplier);
			yield return new WaitForSeconds(_buffDuration);
			_scoreManager.ResetScoreMultiplier();
			StartCoroutine(BuffCooldown());
		}

		private IEnumerator ApplyFireRateBuff()
		{
			_isBuffActive = true;
			_player.SetFireRateMultiplier(_fireRateMultiplier);
			yield return new WaitForSeconds(_buffDuration);
			_player.ResetFireRateMultiplier();
			StartCoroutine(BuffCooldown());
		}


		private IEnumerator ApplyAppearanceBuff(int index)
		{
			_isBuffActive = true;
			_player.SetBuffedAppearance(_buffSprites[index], _buffAnimators[index]);
			yield return new WaitForSecondsRealtime(_buffDuration);
			_player.ResetAppearance();
			StartCoroutine(BuffCooldown());
		}
		#endregion

		//Buff Cooldown
		#region Buff Cooldown

		private IEnumerator BuffCooldown()
		{
			_isCooldown = true;
			float elapsedTime = 0f;

			while (elapsedTime < _initialCooldown)
			{
				elapsedTime += Time.deltaTime;
				_cooldownFillImage.fillAmount = elapsedTime / _initialCooldown;
				yield return null;
			}

			_cooldownFillImage.fillAmount = 1f; // Reset when ready
			_isCooldown = false;
			_isBuffActive = false;
		}
		#endregion

		//Audio
		#region Audio

		private void PlayAudioInfluencerManagerClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _influencerManagerClips.Length)
			{
				_influencerManagerAudioSource.clip = _influencerManagerClips[clipIndex];
				_influencerManagerAudioSource.Play();
			}
		}
		#endregion
	}

}
