using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Diggy_MiniGame_1
{
	public class ScoreManager : MonoBehaviour
	{
		// Serialize Fields
		#region SerializeField
		[SerializeField]
		private TextMeshProUGUI _scoreText;
		#endregion

		// Variables
		#region Variables
		public int scoreCount;
		private float scoreMultiplier = 1f;
		#endregion

		// Points Methods
		#region Point Methods
		public void AddScore(int points)
		{
			scoreCount = Mathf.Max(0, scoreCount + (int)(points * scoreMultiplier));

			ScoreUpdate();

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



		// Set the score multiplier
		public void SetScoreMultiplier(float multiplier)
		{
			scoreMultiplier = multiplier;
		}

		// Reset the score multiplier to 1
		public void ResetScoreMultiplier()
		{
			scoreMultiplier = 1f;
		}

		#endregion
	}
}

