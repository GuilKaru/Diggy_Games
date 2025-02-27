using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickUpScore : MonoBehaviour
	{
		[SerializeField] private int _scoreValue = 10;
		private ScoreManager _scoreManager;

		private void Awake()
		{
			_scoreManager = FindObjectOfType<ScoreManager>();
		}

		public void HandleDrop()
		{
			if (_scoreManager != null)
			{
				_scoreManager.AddScore(_scoreValue);
				Debug.Log($"Score Added: {_scoreValue}");
			}
		}

		
	}
}
