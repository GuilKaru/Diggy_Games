using UnityEngine;

namespace MainMenu
{
    public class PlayerData : MonoBehaviour
    {
        [SerializeField] public string username;
        [SerializeField] public string diggys;
        [SerializeField] public string diggyCoins;
        [SerializeField] public double diggyCoinsD;
        [SerializeField] public int furnaceFrenzyMaxScore;

        [SerializeField] public string sweepBuff;
        [SerializeField] public string timeBuff;
        [SerializeField] public string rockBuff;
        [SerializeField] public string shieldBuff;
        [SerializeField] public string tripleBuff;
        
        [SerializeField] public int sweepBuffI;
        [SerializeField] public int timeBuffI;
        [SerializeField] public int rockBuffI;
        [SerializeField] public int shieldBuffI;
        [SerializeField] public int tripleBuffI;

        [SerializeField] public string priceDiggyx10;
        [SerializeField] public string priceDiggyx50;
        [SerializeField] public string priceDiggyx100;
        [SerializeField] public string priceDiggyx200;
    }
}
