using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MainMenu
{
    public class LeaderboardPlayerStats : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName; 
        [SerializeField] private TextMeshProUGUI playerRank;
        [SerializeField] private TextMeshProUGUI playerScore;
        [SerializeField] private TextMeshProUGUI playerPrincipal;

        public void PutPlayerStats(string score, string name, string principal, string rank)
        {
            playerName.text = name;
            playerScore.text = score;
            playerPrincipal.text = principal;
            playerRank.text = rank;
        }
        
    }
}