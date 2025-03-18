using UnityEngine;
using System.Collections;
using MainMenu;
using UnityEngine.SceneManagement;
using TMPro;

namespace Diggy_MiniGame_1
{
	public class GameManager : MonoBehaviour
	{
		// Singleton Pattern
		public static GameManager Instance { get; private set; }

		// Serialized Variables
		[SerializeField] private GameObject _pauseMenuUI;
		[SerializeField] private GameObject _gameOverMenuUI;
		[SerializeField] private PlayerController _playerController; 
		[SerializeField] private PlayerHealth _playerHealth;
		[SerializeField] private ScoreManager _scoreManager;
		//[SerializeField] private InfluencerManager _influencerManager;
		
		[SerializeField] public TextMeshProUGUI ScoreText;
		[SerializeField] public TextMeshProUGUI UsernameText;
		[SerializeField] public TextMeshProUGUI RankText;

		[Header("Game Manager Audio")]
		[SerializeField]
		private AudioSource _gameManagerAudioSource;
		[SerializeField]
		private AudioClip[] _gameManagerClips;

		private bool _isGamePaused;
		private bool _isGameOver;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject); // Optional if GameManager should persist across scenes
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Update()
		{
			// Handle Pause toggle with Escape key
			if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver )//&& !_influencerManager.IsInfluencerMenuOpen)
			{
				if (!_isGameOver)
				{
					TogglePause();
				}
			}
		}

		// Toggle Pause Menu
		public void TogglePause()
		{
			_isGamePaused = !_isGamePaused;
			_pauseMenuUI.SetActive(_isGamePaused);
			PlayAudioGameManagerClip(0);
			// Freeze/unfreeze game time
			Time.timeScale = _isGamePaused ? 0 : 1;

			// Enable/disable player controls
			if (_playerController != null)
			{
				_playerController.enabled = !_isGamePaused;
			}
		}

		// Game Over Logic
		public void GameOver()
		{
			_isGameOver = true;
			_gameOverMenuUI.SetActive(true);
			
			MainMenu.GameManager.instance.boomScoreFF.ActionButtonClickHandler(_scoreManager.scoreCount);

			// Freeze game time
			Time.timeScale = 0;

			// Disable player controls
			if (_playerController != null)
			{
				_playerController.enabled = false;
			}
		}

		// Restart Game Logic
		public void RestartGame()
		{
			// Reset game state
			_isGameOver = false;
			_isGamePaused = false;

			// Unfreeze game time
			Time.timeScale = 1;
			PlayAudioGameManagerClip(0);
			// Reload the active scene
			StartCoroutine(SoundQueue());
			
		}

		// Back to Main Menu Logic (placeholder, to be implemented later)
		public void BackToMainMenu()
		{
			MainMenu.GameManager.instance.sceneController.BackToMenu("Game1");
		}

		private void ResetGameState()
		{
			// Placeholder for additional reset logic, if needed
			Debug.Log("Resetting game state.");
		}

		private IEnumerator SoundQueue()
		{
			yield return new WaitForSeconds(0.2f);
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}


		private void PlayAudioGameManagerClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _gameManagerClips.Length)
			{
				_gameManagerAudioSource.clip = _gameManagerClips[clipIndex];
				_gameManagerAudioSource.Play();
			}
		}
	}
}

