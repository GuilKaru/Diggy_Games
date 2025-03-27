using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Boom;
using Boom.Utility;
using Boom.Values;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Boom.Tutorials;
using Diggy_MiniGame_1;

namespace MainMenu
{
    [Serializable]
    public class LBEntry
    {
        public string Username {get; set;}
        public double Score {get; set;}
        public string Principal {get; set;}

        public LBEntry(string username, int score, string principal)
        {
            Username = username;
            Score = score;
            Principal = principal;
        }
    }
    public class BoomLeaderboard : MonoBehaviour
    {

        private List<LBEntry> LBEntries = new List<LBEntry>();
        private List <GameObject> _playerObjects = new List<GameObject>();
        public GameObject playerStatsPrefab;
        
        string actionMode;

        //[SerializeField] private GameObject playerStatsPrefab;
        [SerializeField] private Transform playerStatsContainer;
        [SerializeField] private GameObject playerPrefab;

        [SerializeField] public TextMeshProUGUI playerRank;
        [SerializeField] public TextMeshProUGUI playerName;
        [SerializeField] public TextMeshProUGUI playerScore;
        [SerializeField] public Image playerRankTier;
        
        [SerializeField] private List<Sprite> playerSprites = new List<Sprite>();
        
        //[SerializeField] TMP_Text content;

        /*private void Awake()
        {
            UserUtil.AddListenerMainDataChange<MainDataTypes.LoginData>(LoginDataChangeHandler, new() { invokeOnRegistration = true });
            //UpdateLeaderboard();
            //addButton.onClick.AddListener(SetLeaderboardEntry);
            
        }*/

        private void OnDestroy()
        {
            UserUtil.RemoveListenerMainDataChange<MainDataTypes.LoginData>(LoginDataChangeHandler);

        }

        private void LoginDataChangeHandler(MainDataTypes.LoginData data)
        {
            if (data.state == MainDataTypes.LoginData.State.LoggedIn)
            {
                UpdateLeaderboard();
            }
        }

        public void UpdateLeaderboard()
        {
            //content.text = "";

            var loginData = UserUtil.GetLogInData().AsOk();
            var ownPrincipal = loginData.principal;

            DisplayLeaderboardWithEntityAsEntries(ownPrincipal);
        }

        private void DisplayLeaderboardWithEntityAsEntries(string ownPrincipal)
        {
            //I use try query all entities with a predefined filter that specifies that
            //I only want the entities from the world canister with a field tag of value "lb"
            EntityUtil.TryQueryEntities(EntityUtil.Queries.worldEntityFieldTagLb, out var lbEntries);
            //EntityUtil.TryQueryEntities(BoomManager.Instance.WORLD_CANISTER_ID, e => e.eid.Contains("lb_"), out var lbEntries);
            //We initialize the entry list in case it is null
            if (lbEntries == null)
            {
                lbEntries = new();
            }
            
            LBEntries.Clear();
            LBEntries = new List<LBEntry>();

            if (_playerObjects == null || _playerObjects.Count == 0)
            {
                _playerObjects = new();
            }
            else
            {
                foreach (var playerObject in _playerObjects)
                {
                    Destroy(playerObject);
                }

                _playerObjects.Clear();
            }
            
            //bool userEntryExist = false;
            
            foreach (var entity in lbEntries)
            {
                /*if (entity.eid == ownPrincipal)
                {
                    userEntryExist = true;
                }*/
                
                entity.TryGetFieldAsText("username", out var username);
                entity.TryGetFieldAsText("maxscore", out var score);

                if (username is null or "None")
                {
                    username = entity.eid;
                }

                if (score is null or "None")
                {
                    score = "0";
                }
                LBEntry entries = new LBEntry(username, int.Parse(score), entity.eid);
                
                LBEntries.Add(entries);
                
                // GameObject playerObject = Instantiate(playerStatsPrefab, playerStatsContainer);
                // playerObject.transform.SetParent(playerStatsContainer);
                // LeaderboardPlayerStats playerStats = playerObject.GetComponent<LeaderboardPlayerStats>();
                //
                // playerStats.PutPlayerStats(score, username, entity.eid);
                //_playerObjects.Add(playerObject);

                //content.text += $" -> Username: {username}, Score: {score}\n";
            }
            //if (userEntryExist == false) content.text += $" -> Username: {ownPrincipal.SimplifyAddress()}, Score: {0}\n";

            if (LBEntries.Count > 0)
            {
                LBEntries.Sort((x, y) => y.Score.CompareTo(x.Score));
            }


            for (int i = 0; i < LBEntries.Count; i++)
            {
                GameObject playerObject = Instantiate(playerStatsPrefab, playerStatsContainer);
                playerObject.transform.SetParent(playerStatsContainer);
                LeaderboardPlayerStats playerStats = playerObject.GetComponent<LeaderboardPlayerStats>();

                string firstThree = LBEntries[i].Principal.Substring(0, 3);
                string lastThree = LBEntries[i].Principal.Substring(LBEntries[i].Principal.Length - 3, 3);
                string shortened = $"{firstThree} ... {lastThree}";
                Sprite currentSprite;

                if (i < 3)
                {
                    currentSprite = playerSprites[0];
                }
                else if (i < 10)
                {
                    currentSprite = playerSprites[1];
                }
                else if (i < 30)
                {
                    currentSprite = playerSprites[2];
                }
                else if (i < 100)
                {
                    currentSprite = playerSprites[3];
                }
                else
                {
                    currentSprite = playerSprites[4];
                }
                
                playerStats.PutPlayerStats(LBEntries[i].Score.ToString(), LBEntries[i].Username, shortened, $"#{i + 1}", currentSprite);
                _playerObjects.Add(playerObject);

                if (LBEntries[i].Principal == ownPrincipal)
                {
                    playerRank.text = $"#{i + 1}";
                    playerName.text = LBEntries[i].Username;
                    playerScore.text = LBEntries[i].Score.ToString();
                    playerRankTier.sprite = currentSprite;
                }
            }
            /*foreach (LBEntry entry in LBEntries)
            {
                Debug.Log($"Username: {entry.Username} // Score: {entry.Score} // Principal: {entry.Principal}");
            }*/
            /*if(userEntryExist == false)
            {
                names.Add(ownPrincipal.SimplifyAddress());
                numbers.Add(0);
            }*/

            /*var pairedList = names.Zip(numbers, (name, number) => new { Name = name, Number = number }).ToList();
            var sortedList = pairedList.OrderByDescending(pair => pair.Number).ToList();

            List<string> sortedNames = sortedList.Select(pair => pair.Name).ToList();
            List<int> sortedNumbers = sortedList.Select(pair => pair.Number).ToList();

            for(int i = 0; i < sortedNumbers.Count; i++)
            {
                content.text += $" -> {i}) Username: {sortedNames[i]}, Score: {sortedNumbers[i]}\n";
            }*/
            
            //SaveLeaderboard(LBEntries);
        }

        public void SaveLeaderboard(List<LBEntry> entries)
        {
            string json = JsonConvert.SerializeObject(entries, Formatting.Indented);
            
            string filePath = Application.persistentDataPath + "/leaderboard.json";

            File.WriteAllText(filePath, json);
        }
        public void SetLeaderboardEntry(string actionM, string score, string userName)
        {
            actionMode = actionM;

            var loginData = UserUtil.GetLogInData().AsOk();
            Diggy_MiniGame_1.GameManager.Instance.ScoreText.text = $"Total Score: {score}";
            Diggy_MiniGame_1.GameManager.Instance.UsernameText.text = $"{GameManager.instance.playerData.username}";
            SetEntityAsEntry(loginData, score, userName, actionM);
        }


        private void SetEntityAsEntry(MainDataTypes.LoginData loginData, string score, string userName, string actionM)
        {
            //I use try query all entities with a predefined filter that specifies that
            //I only want the entities from the world canister with a field tag of value "lb"
            EntityUtil.TryQueryEntities(EntityUtil.Queries.worldEntityFieldTagLb, out var lbEntries);

            //We initialize the entry list in case it is null
            if (lbEntries == null) lbEntries = new();

            DataTypes.Entity lbEntry = null;

            foreach (var entity in lbEntries)
            {
                if (entity.eid == loginData.principal)
                {
                    lbEntry = entity;
                    break;
                }
            }

            /*double currentScore = 0;
            if (lbEntry != null)
            {
                lbEntry.TryGetFieldAsDouble("score", out currentScore);
            }*/
            Debug.Log("It's making this action");
            ActionUtil.ProcessAction(actionM, new()
            {
                new Candid.World.Models.Field() { FieldName = "username", FieldValue = string.IsNullOrEmpty(userName)? loginData.principal.SimplifyAddress() : userName },
                new Candid.World.Models.Field() { FieldName = "maxscore", FieldValue = score },
            });
            Debug.Log("Before CoroutineManager");
            //CoroutineManagerUtil.DelayAction(UpdateLeaderboardGameOver, 1f, transform);
            UpdateLeaderboardGameOver();
        }
        
        public void UpdateLeaderboardGameOver()
        {
            //content.text = "";

            var loginData = UserUtil.GetLogInData().AsOk();
            var ownPrincipal = loginData.principal;

            DisplayLeaderboardWithEntityAsEntriesGameOver(ownPrincipal);
        }

        private void DisplayLeaderboardWithEntityAsEntriesGameOver(string ownPrincipal)
        {
            
            //I use try query all entities with a predefined filter that specifies that
            //I only want the entities from the world canister with a field tag of value "lb"
            EntityUtil.TryQueryEntities(EntityUtil.Queries.worldEntityFieldTagLb, out var lbEntries);
            //EntityUtil.TryQueryEntities(BoomManager.Instance.WORLD_CANISTER_ID, e => e.eid.Contains("lb_"), out var lbEntries);
            //We initialize the entry list in case it is null
            if (lbEntries == null)
            {
                lbEntries = new();
            }
            
            LBEntries.Clear();
            LBEntries = new List<LBEntry>();

            if (_playerObjects == null || _playerObjects.Count == 0)
            {
                _playerObjects = new();
            }
            else
            {
                foreach (var playerObject in _playerObjects)
                {
                    Destroy(playerObject);
                }

                _playerObjects.Clear();
                _playerObjects = new();
            }
            
            //bool userEntryExist = false;
            
            foreach (var entity in lbEntries)
            {
                /*if (entity.eid == ownPrincipal)
                {
                    userEntryExist = true;
                }*/
                
                entity.TryGetFieldAsText("username", out var username);
                entity.TryGetFieldAsText("maxscore", out var score);

                if (username is null or "None")
                {
                    username = entity.eid;
                }

                if (score is null or "None")
                {
                    score = "0";
                }
                LBEntry entries = new LBEntry(username, int.Parse(score), entity.eid);
                
                LBEntries.Add(entries);
                
            }


            if (LBEntries.Count > 0)
            {
                LBEntries.Sort((x, y) => y.Score.CompareTo(x.Score));
            }


            for (int i = 0; i < LBEntries.Count; i++)
            {
                /*GameObject playerObject = Instantiate(LBEntries[i].playerStatsPrefab, playerStatsContainer);
                playerObject.transform.SetParent(playerStatsContainer);
                LeaderboardPlayerStats playerStats = playerObject.GetComponent<LeaderboardPlayerStats>();
                
                Sprite currentSprite;

                if (i < 3)
                {
                    currentSprite = playerSprites[0];
                }
                else if (i < 10)
                {
                    currentSprite = playerSprites[1];
                }
                else if (i < 30)
                {
                    currentSprite = playerSprites[2];
                }
                else if (i < 100)
                {
                    currentSprite = playerSprites[3];
                }
                else
                {
                    currentSprite = playerSprites[4];
                }
                
                playerStats.PutPlayerStats(LBEntries[i].Score.ToString(), LBEntries[i].Username, LBEntries[i].Principal, $"#{i + 1}", currentSprite);
                _playerObjects.Add(playerObject);*/

                if (LBEntries[i].Principal == ownPrincipal)
                {
                    /*playerRank.text = $"#{i + 1}";
                    playerName.text = LBEntries[i].Username;
                    playerScore.text = LBEntries[i].Score.ToString();*/
                    Diggy_MiniGame_1.GameManager.Instance.RankText.text = $"Rank: {i + 1}";

                    return;
                }
            }
        }
    }
}