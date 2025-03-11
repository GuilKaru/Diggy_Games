using UnityEngine;
using System.Collections;
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
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha6) && !_isCooldown && !_isBuffActive)
			{
				ActivateInfluencerBuff();
			}
		}


		private IEnumerator StartupCooldown()
		{
			yield return new WaitForSeconds(_initialCooldown);
			_isCooldown = false;
		}
		#endregion

		//Buff Activation
		#region Buff Activation

		private void ActivateInfluencerBuff()
		{
			PlayAudioInfluencerManagerClip(0);

			StartCoroutine(ApplySpeedBuff());
			StartCoroutine(ApplyScoreBuff());
			StartCoroutine(ApplyFireRateBuff());
			StartCoroutine(ApplyAppearanceBuff());
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


		private IEnumerator ApplyAppearanceBuff()
		{
			_isBuffActive = true;
			_player.SetBuffedAppearance(); // Switch animator & sprite
			yield return new WaitForSeconds(_buffDuration);
			_player.ResetAppearance(); // Reset animator & sprite
			StartCoroutine(BuffCooldown());
		}
		#endregion

		//Buff Cooldown
		#region Buff Cooldown

		private IEnumerator BuffCooldown()
		{
			_isCooldown = true;
			yield return new WaitForSeconds(_initialCooldown);
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
