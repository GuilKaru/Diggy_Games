using TMPro;
using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class ScoreManager : MonoBehaviour
	{
		// Serialize Fields
		#region SerializeField
		[SerializeField]
		private TextMeshProUGUI _scoreText;

		[Header("Score Audio")]
		[SerializeField]
		private AudioSource _scoreAudioSource;
		[SerializeField]
		private AudioClip[] _scoreClips;
		#endregion

		// Variables
		#region Variables
		public int scoreCount;
		#endregion

		// Points Methods
		#region Point Methods
		public void AddScore(int points)
		{
			scoreCount = Mathf.Max(0, scoreCount + points);

			ScoreUpdate();
			PlayAudioScoreClip(0);
			Debug.Log("Score updated: " + scoreCount);

		}

		// Deduct score but keep it at a minimum of zero
		public void DeductScore(int points)
		{
			scoreCount = Mathf.Max(0, scoreCount + points); // Deduct points, but cap at zero
			ScoreUpdate();
			Debug.Log("Score deducted: " + scoreCount);
		}


		// Update the score display
		public void ScoreUpdate()
		{
			_scoreText.text = scoreCount.ToString();

		}


		public int GetScore()
		{
			return scoreCount;
		}

		private void PlayAudioScoreClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _scoreClips.Length)
			{
				_scoreAudioSource.clip = _scoreClips[clipIndex];
				_scoreAudioSource.Play();
			}
		}

		#endregion
	}
}

