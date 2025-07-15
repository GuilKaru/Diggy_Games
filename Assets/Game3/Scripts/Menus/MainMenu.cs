using Boom;
using MainMenu;
using TMPro;
using UnityEngine;
namespace Diggy_MiniGame_3
{
	public class MainMenu : MonoBehaviour
	{
		#region Private Variables

		[SerializeField]
		private GameObject _loginMenu;
		[SerializeField]
		public GameObject _mainMenu;
		[SerializeField]
		public GameObject _gameCanvas;
		[SerializeField]
		private GameObject _tutorialCanvas;

		#endregion

		public void ChangeLoginMenu()
		{
			_loginMenu.SetActive(false);
		}

		public void StartGame()
		{
			_mainMenu.SetActive(false);
			_gameCanvas.SetActive(true);

			//Start Game Logic
			GameManager.gameManager.StartGame();
		}

		public void TutorialToggle(bool active)
		{
			_tutorialCanvas.SetActive(active);
		}
		
	}
}


