using Boom;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public MainMenu mainMenu;
        [SerializeField] public PlayerData playerData;
        
        [SerializeField] private TextMeshProUGUI scoreTextFF;
        
        [SerializeField] public BoomBuffDecrease boomBuffDecrease;
        
        [SerializeField] public SceneController sceneController;
        [SerializeField] public BoomUsername boomUsername;
        [SerializeField] public BoomScoreFF boomScoreFF;
        [SerializeField] public BoomLeaderboard boomLeaderboard;
        
        [Header("Main Menu Objects")]
        [SerializeField] GameObject gamesObject;
        [SerializeField] GameObject cameraAndLightObject;
        
        public static GameManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void ScoreUpdateFF()
        {
            var principal = UserUtil.GetPrincipal();
            EntityUtil.TryGetFieldAsText(principal, "score_1", "maxscore", out var outScore, "None");

            if (outScore is "None" or null)
            {
                ScoreSafeFF("0");
            }
            else
            {
                ScoreSafeFF(outScore);
            }
        }

        public void ScoreSafeFF(string score)
        {
            playerData.furnaceFrenzyMaxScore = int.Parse(score);

            boomLeaderboard.playerScore.text = score;
        }

        public void ActivateMainMenu(bool activate)
        {
            gamesObject.SetActive(activate);
            cameraAndLightObject.SetActive(activate);
        }
    }
}
