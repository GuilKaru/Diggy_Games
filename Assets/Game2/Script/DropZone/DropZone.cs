using System.Collections.Generic;
using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class DropZone : MonoBehaviour
	{
		private List<GameObject> _scorePickUps = new List<GameObject>();
		private List<GameObject> _timePickUps = new List<GameObject>();

		private ScoreManager _scoreManager;
		private TimerManager _timerManager;

		private void Awake()
		{
			_scoreManager = FindObjectOfType<ScoreManager>();
			_timerManager = FindObjectOfType<TimerManager>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("PickUp"))
			{
				if (other.GetComponent<PickUpScore>() != null)
				{
					PickUpScore pickup = other.GetComponent<PickUpScore>();
					_scorePickUps.Add(other.gameObject);
				}
				else if (other.GetComponent<PickUpTime>() != null)
				{
					PickUpTime pickup = other.GetComponent<PickUpTime>();
					_timePickUps.Add(other.gameObject);
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				if (_scorePickUps.Count > 0)
				{
					CalculateScore();
				}

				if (_timePickUps.Count > 0)
				{
					CalculateTime();
				}
			}
		}

		private void CalculateScore()
		{
			int coinCount = _scorePickUps.Count;
			int finalScore = 0;

			switch (coinCount)
			{
				case 1:
					finalScore = 10;
					break;
				case 2:
					finalScore = 25;
					break;
				case 3:
					finalScore = 40;
					break;
				case 4:
					finalScore = 65;
					break;
				case 5:
					finalScore = 80;
					break;
				default:
					finalScore = 80 + (coinCount - 5) * 10;
					break;
			}

			_scoreManager.AddScore(finalScore);
			Debug.Log($"Dropped {coinCount} Score Pickups - Score Added: {finalScore}");

			foreach (var pickup in _scorePickUps)
			{
				Destroy(pickup);
			}

			_scorePickUps.Clear();
		}

		private void CalculateTime()
		{
			foreach (var pickup in _timePickUps)
			{
				PickUpTime timePickup = pickup.GetComponent<PickUpTime>();
				timePickup.HandleDrop();
				Destroy(pickup);
			}

			_timePickUps.Clear();
		}
	}
}


