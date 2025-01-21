using Boom;
using UnityEngine;
using TMPro;

namespace MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject _usernameMenu;
        [SerializeField] private GameObject _loginMenu;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _tutorialCanvas;
        
        //[SerializeField] public PlayerData _playerData;

        [SerializeField] private TextMeshProUGUI _scoreText;

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
            _mainMenu.SetActive(true);
        }
        
        #endregion
    }
}

