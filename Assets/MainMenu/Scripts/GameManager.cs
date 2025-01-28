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
            EntityUtil.TryGetFieldAsText(principal, "scoreFF", "maxscoreFF", out var outScore, "None");

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

            scoreTextFF.text = "Max Score: " + score;
        }
    }
}
