using Boom;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject _usernameMenu;
        [SerializeField] private GameObject _loginMenu;
        [SerializeField] private GameObject _gameSelectorMenu;
        [SerializeField] private GameObject _furnaceFrenzyMenu;
		[SerializeField] private GameObject _furnaceFrenzyTutorial;
		[SerializeField] private GameObject _furnaceFrenzyLeaderBoard;
		[SerializeField] private GameObject _furnaceFrenzyStore;

		[Header("Game Manager Audio")]
		[SerializeField]
		private AudioSource _gameManagerAudioSource;
		[SerializeField]
		private AudioClip[] _gameManagerClips;

		#endregion
		#region Unity Methods

		public void LoggedIn()
        {
            _loginMenu.SetActive(false);
            _usernameMenu.SetActive(true);
        }

        public void UsernameMenuChange()
        {
            _usernameMenu.SetActive(false);
            //_gameSelectorMenu.SetActive(true);
            _furnaceFrenzyMenu.SetActive(true);
        }

        public void GameMenuOpen(string gameName)
        {
            if (gameName == "FurnaceFrenzy")
            {
                _furnaceFrenzyMenu.SetActive(true);
            }
        }

        public void NameSafe(string username)
        {
            GameManager.instance.playerData.username = username;
            _usernameMenu.SetActive(false);
            //_gameSelectorMenu.SetActive(true);
            _furnaceFrenzyMenu.SetActive(true);
        }

        public void OpenTutorial()
        {
			_furnaceFrenzyTutorial.SetActive(true);
            PlayAudioMainMenuClip(0);
		}

		public void CloseTutorial()
		{
			_furnaceFrenzyTutorial.SetActive(false);
			PlayAudioMainMenuClip(0);
		}

		public void OpenLeaderBoard()
		{
			_furnaceFrenzyLeaderBoard.SetActive(true);
			PlayAudioMainMenuClip(0);

		}

		public void CloseLeaderBoard()
		{
			_furnaceFrenzyLeaderBoard.SetActive(false);
			PlayAudioMainMenuClip(0);
		}

		public void OpenStore()
		{
			_furnaceFrenzyStore.SetActive(true);
			PlayAudioMainMenuClip(0);
		}

		public void CloseStore()
		{
			_furnaceFrenzyStore.SetActive(false);
			PlayAudioMainMenuClip(0);
		}

		public void PlayGame()
		{
			SceneManager.LoadScene(0);
		}

		private void PlayAudioMainMenuClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _gameManagerClips.Length)
			{
				_gameManagerAudioSource.clip = _gameManagerClips[clipIndex];
				_gameManagerAudioSource.Play();
			}
		}
		#endregion
	}
}

