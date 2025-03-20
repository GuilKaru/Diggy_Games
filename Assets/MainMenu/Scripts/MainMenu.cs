using System;
using Boom;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Boom.Utility;
using Boom.Values;

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
		[SerializeField] private GameObject _rewardsPanel;
		[SerializeField] private Button _playButton;

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
		[SerializeField] private TextMeshProUGUI _priceDiggyx10;
		[SerializeField] private TextMeshProUGUI _priceDiggyx50;
		[SerializeField] private TextMeshProUGUI _priceDiggyx100;
		[SerializeField] private TextMeshProUGUI _priceDiggyx200;

		[SerializeField] private BoomUsername _boomUsername;
		
		[SerializeField] public GameObject _purchaseSuccessfulObject;
		[SerializeField] public GameObject _purchaseFailedObject;
		[SerializeField] public GameObject _insufficientFundsObject;

		[SerializeField] public bool playerWhitelisted = false;
		[SerializeField] public bool whitelistActivated = false;

		[SerializeField] private GameObject _maintenancePlay;
		[SerializeField] private GameObject _maintenanceStore;
		
		private int _currentTutorialIndex = 0;
		
		//Open New Tab variable
		[DllImport("__Internal")]
		private static extern void OpenNewTab(string url);

		#endregion
		
		#region Unity Methods

		public void LoggedIn()
        {
            _loginMenu.SetActive(false);
            _loadingPanel.SetActive(false);
            _usernameMenu.SetActive(true);
            ConfigUtil.TryGetConfig("bg4su-6iaaa-aaaap-anxsa-cai", "maintenanceConfig", out var outConfig);

            outConfig.fields.TryGetValue("whitelistActivation", out var whitelist);

            if (whitelist == "true") whitelistActivated = true;
            
            GameManager.instance.principalChecker.currentPrincipalId = UserUtil.GetPrincipal();
            GameManager.instance.principalChecker.CreatePrincipalList();
            GameManager.instance.principalChecker.CheckPrincipals();
            
            //Read Config to know if the game is in Maintenance
            outConfig.fields.TryGetValue("playMaintenance", out  var playMaintenance);
            outConfig.fields.TryGetValue("storeMaintenance", out var storeMaintenance);

            if (playMaintenance == "true")
            {
	            _maintenancePlay.SetActive(true);
            }

            if (storeMaintenance == "true")
            {
	            _maintenanceStore.SetActive(true);
            }
			
            ActivatePlayButton();
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
	        GameManager gameManager = GameManager.instance;
            gameManager.playerData.username = username;
            _username.text = username;
            _usernameMenu.SetActive(false);
            //_gameSelectorMenu.SetActive(true);
            _furnaceFrenzyMenu.SetActive(true);

            gameManager.boomLeaderboard.playerRank.text = "No Rank";
            gameManager.boomLeaderboard.playerName.text = username;
            gameManager.ScoreUpdateFF();
            
            ActivatePlayButton();
        }

        public void ActivatePlayButton()
        {
	        if (whitelistActivated)
	        {
		        if (playerWhitelisted)
		        {
			        if (GameManager.instance.playerData.diggyCoinsD > 0)
			        {
				        _playButton.interactable = true;
			        }
			        else
			        {
				        _playButton.interactable = false;
			        }
		        }
		        else
		        {
			        _playButton.interactable = false;
		        }
	        }
	        else
	        {
		        if (GameManager.instance.playerData.diggyCoinsD > 0)
		        {
			        _playButton.interactable = true;
		        }
		        else
		        {
			        _playButton.interactable = false;
		        }
	        }
	        
        }
        public void CoinsSafe(double diggyCoin, double sweepBuff, double timeBuff, double rockBuff, double shieldBuff, double tripleBuff)
        {
	        PlayerData playerData = GameManager.instance.playerData;
	        
	        //playerData.diggyCoins = diggyCoin.ToString();
	        
	        playerData.sweepBuff = sweepBuff.ToString("0.0");
	        playerData.timeBuff = timeBuff.ToString("0.0");
	        playerData.rockBuff = rockBuff.ToString("0.0");
	        playerData.shieldBuff = shieldBuff.ToString("0.0");
	        playerData.tripleBuff = tripleBuff.ToString("0.0");

	        playerData.diggyCoins = diggyCoin.ToString("0.0");
	        playerData.diggyCoinsD = diggyCoin;
	        playerData.sweepBuffI = Convert.ToInt32(sweepBuff);
	        playerData.timeBuffI = Convert.ToInt32(timeBuff);
	        playerData.rockBuffI = Convert.ToInt32(rockBuff);
	        playerData.shieldBuffI = Convert.ToInt32(shieldBuff);
	        playerData.tripleBuffI = Convert.ToInt32(tripleBuff);
	        
	        _diggyCoins.text = diggyCoin.ToString();
	        _sweepBuff.text = playerData.sweepBuffI.ToString();
	        _timeBuff.text = playerData.timeBuffI.ToString();
	        _rockBuff.text = playerData.rockBuffI.ToString();
	        _shieldBuff.text = playerData.shieldBuffI.ToString();
	        _tripleBuff.text = playerData.tripleBuffI.ToString();
	        
	        ActivatePlayButton();
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
			
			_boomUsername.UpdateCoins();
		}

		public void CloseTutorial()
		{
			_furnaceFrenzyTutorial.SetActive(false);
			_currentTutorialIndex = 0; // Ensure first image is shown next time
			UpdateTutorialView();
			PlayAudioMainMenuClip(0);
			
			_boomUsername.UpdateCoins();
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

		public void OpenLeaderboard()
		{
			GameManager.instance.boomLeaderboard.UpdateLeaderboard();
			_furnaceFrenzyLeaderBoard.SetActive(true);
			
			_boomUsername.UpdateCoins();
		}
		public void CloseLeaderBoard()
		{
			_furnaceFrenzyLeaderBoard.SetActive(false);
			PlayAudioMainMenuClip(0);
			
			_boomUsername.UpdateCoins();
		}

		public void OpenRewards(bool active)
		{
			_rewardsPanel.SetActive(active);
		}

		public void OpenStore()
		{
			_furnaceFrenzyStore.SetActive(true);
			PlayAudioMainMenuClip(0);
			
			_boomUsername.UpdateCoins();
		}

		public void CloseStore()
		{
			_furnaceFrenzyStore.SetActive(false);
			PlayAudioMainMenuClip(0);
			
			_boomUsername.UpdateCoins();
		}

		public void PlayGame()
		{
			_loadingPanel.SetActive(true);
			GameManager.instance.boomBuffDecrease.PlayCoinsDecrease("decrease_dc_x1");
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
		
		public void UpdatePriceStore(string pricex10, string pricex50, string pricex100, string pricex200)
		{
			GameManager.instance.playerData.priceDiggyx10 = pricex10;
			GameManager.instance.playerData.priceDiggyx50 = pricex50;
			GameManager.instance.playerData.priceDiggyx100 = pricex100;
			GameManager.instance.playerData.priceDiggyx200 = pricex200;
			
			_priceDiggyx10.text = pricex10;
			_priceDiggyx50.text = pricex50;
			_priceDiggyx100.text = pricex100;
			_priceDiggyx200.text = pricex200;
		}
		
		//Open new tab logic
		public void OpenURL(string url)
		{
			#if !UNITY_EDITOR && UNITY_WEBGL
			OpenNewTab(url);
			#else
			Application.OpenURL(url);
			#endif		
		}
		
		//Purchase Successful
		public void PurchaseSuccessfulClose()
		{
			_purchaseSuccessfulObject.SetActive(false);
		}
		
		//Insufficient Funds
		public void InsufficientFundsClose()
		{
			_insufficientFundsObject.SetActive(false);
		}

		public void PurchaseFailedClose()
		{
			_purchaseFailedObject.SetActive(false);
		}
		#endregion
	}
}

