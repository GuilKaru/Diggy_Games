using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        
        public void PlayGameFF()
        {
            loadingScreen.SetActive(true);
            Time.timeScale = 1;
            StartCoroutine(LoadLevelFFAsync());
        }

        public void Restart(string gameName)
        {
            SceneManager.UnloadSceneAsync(gameName);
            //Reload the current Scene
            SceneManager.LoadSceneAsync(gameName, LoadSceneMode.Additive);
            //Set the time scale to normal
            Time.timeScale = 1.0f;
        }

        public void BackToMenu(string gameName)
        {
            SceneManager.UnloadSceneAsync(gameName);
            Time.timeScale = 1.0f;
            GameManager.instance.ActivateMainMenu(true);
            
            GameManager.instance.ScoreUpdateFF();
        }

        IEnumerator LoadLevelFFAsync()
        {
            AsyncOperation loadOperations = SceneManager.LoadSceneAsync("Game1", LoadSceneMode.Additive);

            loadOperations.allowSceneActivation = false;

            while (loadOperations.progress < 0.9f) // wait for it to load
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(2f); //Simulate Delay so there are no errors
            
            loadOperations.allowSceneActivation = true;

            loadingScreen.SetActive(false);

            GameManager.instance.ActivateMainMenu(false);
        }
    }
}
