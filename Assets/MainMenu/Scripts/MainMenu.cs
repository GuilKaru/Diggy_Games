using System;
using Boom;
using UnityEngine;
using System.Collections;
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
		[SerializeField] private GameObject[] _tutorialImages;
		[SerializeField] private GameObject _furnaceFrenzyLeaderBoard;
		[SerializeField] private GameObject _furnaceFrenzyStore;
		[SerializeField] private GameObject _loadingPanel;

		[Header("Game Manager Audio")]
		[SerializeField]
		private AudioSource _gameManagerAudioSource;
		[SerializeField]
		private AudioClip[] _gameManagerClips;

		[Header("Stats")] 
		[SerializeField] private TextMeshProUGUI _username;
		[SerializeField] public TextMeshProUGUI _diggys;
		[SerializeField] private TextMeshProUGUI _diggyCoins;
		[SerializeField] private TextMeshProUGUI _sweepBuff;
		[SerializeField] private TextMeshProUGUI _timeBuff;
		[SerializeField] private TextMeshProUGUI _rockBuff;
		[SerializeField] private TextMeshProUGUI _shieldBuff;
		[SerializeField] private TextMeshProUGUI _tripleBuff;

		[SerializeField] private BoomUsername _boomUsername;

		private int _currentTutorialIndex = 0;

		#endregion
		#region Unity Methods

		public void LoggedIn()
        {
            _loginMenu.SetActive(false);
            _loadingPanel.SetActive(false);
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
            _username.text = username;
            _usernameMenu.SetActive(false);
            //_gameSelectorMenu.SetActive(true);
            _furnaceFrenzyMenu.SetActive(true);
        }

        public void CoinsSafe(string diggyCoin, string sweepBuff, string timeBuff, string rockBuff, string shieldBuff, string tripleBuff)
        {
	        PlayerData playerData = GameManager.instance.playerData;
	        
	        playerData.diggyCoins = diggyCoin;
	        
	        playerData.sweepBuff = sweepBuff;
	        playerData.timeBuff = timeBuff;
	        playerData.rockBuff = rockBuff;
	        playerData.shieldBuff = shieldBuff;
	        playerData.tripleBuff = tripleBuff;

	        double diggyCoinD = Convert.ToDouble(diggyCoin);
	        double sweepBuffD = Convert.ToDouble(sweepBuff);
	        double timeBuffD = Convert.ToDouble(timeBuff);
	        double rockBuffD = Convert.ToDouble(rockBuff);
	        double shieldBuffD = Convert.ToDouble(shieldBuff);
	        double tripleBuffD = Convert.ToDouble(tripleBuff);

	        playerData.diggyCoins = diggyCoinD.ToString("0.0");
	        
	        int sweepBuffI = Convert.ToInt32(sweepBuffD);
	        int timeBuffI = Convert.ToInt32(timeBuffD);
	        int rockBuffI = Convert.ToInt32(rockBuffD);
	        int shieldBuffI = Convert.ToInt32(shieldBuffD);
	        int tripleBuffI = Convert.ToInt32(tripleBuffD);
	        
	        _diggyCoins.text = playerData.diggyCoins;
	        _sweepBuff.text = sweepBuffI.ToString();
	        _timeBuff.text = timeBuffI.ToString();
	        _rockBuff.text = rockBuffI.ToString();
	        _shieldBuff.text = shieldBuffI.ToString();
	        _tripleBuff.text = tripleBuffI.ToString();
        }

        public void UpdateStats()
        {
	        _boomUsername.UpdateCoins();
        }

		public void OpenTutorial()
		{
			_furnaceFrenzyTutorial.SetActive(true);
			_currentTutorialIndex = 0; // Reset to the first tutorial page
			UpdateTutorialView();
			PlayAudioMainMenuClip(0);
		}

		public void CloseTutorial()
		{
			_furnaceFrenzyTutorial.SetActive(false);
			_currentTutorialIndex = 0; // Ensure first image is shown next time
			UpdateTutorialView();
			PlayAudioMainMenuClip(0);
		}

		public void NextTutorial()
		{
			if (_currentTutorialIndex < _tutorialImages.Length - 1)
			{
				_currentTutorialIndex++;
				UpdateTutorialView();
				PlayAudioMainMenuClip(0);
			}
		}

		public void PreviousTutorial()
		{
			if (_currentTutorialIndex > 0)
			{
				_currentTutorialIndex--;
				PlayAudioMainMenuClip(0);
				UpdateTutorialView();
			}
		}

		private void UpdateTutorialView()
		{
			for (int i = 0; i < _tutorialImages.Length; i++)
			{
				_tutorialImages[i].SetActive(i == _currentTutorialIndex);
			}
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
			_loadingPanel.SetActive(true);
			GameManager.instance.sceneController.PlayGameFF();
			PlayAudioMainMenuClip(0);
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

