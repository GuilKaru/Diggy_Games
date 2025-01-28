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
        [SerializeField] private GameObject _gameSelectorMenu;
        [SerializeField] private GameObject _furnaceFrenzyMenu;
        
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
            _gameSelectorMenu.SetActive(true);
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
            _gameSelectorMenu.SetActive(true);
        }
        #endregion
    }
}

