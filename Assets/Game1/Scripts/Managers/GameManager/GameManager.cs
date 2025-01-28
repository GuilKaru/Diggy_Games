using UnityEngine;
using UnityEngine.SceneManagement;
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
			if (Input.GetKeyDown(KeyCode.Escape))
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

			// Reload the active scene
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		// Back to Main Menu Logic (placeholder, to be implemented later)
		public void BackToMainMenu()
		{
			Debug.Log("Back to Main Menu button clicked. Implement main menu logic here.");
			// Add logic to load the main menu scene when needed
		}

		private void ResetGameState()
		{
			// Placeholder for additional reset logic, if needed
			Debug.Log("Resetting game state.");
		}
	}
}

