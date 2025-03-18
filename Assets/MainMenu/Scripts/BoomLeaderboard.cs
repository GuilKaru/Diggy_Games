using System;
using System.Collections.Generic;
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

        public GameObject playerStatsPrefab;

        public LBEntry(string username, int score, string principal, GameObject playerStats)
        {
            Username = username;
            Score = score;
            Principal = principal;
            this.playerStatsPrefab = playerStats;
        }
    }
    public class BoomLeaderboard : MonoBehaviour
    {

        private List<LBEntry> LBEntries = new List<LBEntry>();
        private List <GameObject> _playerObjects = new List<GameObject>();
        
        string actionMode;

        //[SerializeField] private GameObject playerStatsPrefab;
        [SerializeField] private Transform playerStatsContainer;
        [SerializeField] private GameObject playerPrefab;

        [SerializeField] public TextMeshProUGUI playerRank;
        [SerializeField] public TextMeshProUGUI playerName;
        [SerializeField] public TextMeshProUGUI playerScore;
        
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
                LBEntry entries = new LBEntry(username, int.Parse(score), entity.eid, playerPrefab);
                
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
                GameObject playerObject = Instantiate(LBEntries[i].playerStatsPrefab, playerStatsContainer);
                playerObject.transform.SetParent(playerStatsContainer);
                LeaderboardPlayerStats playerStats = playerObject.GetComponent<LeaderboardPlayerStats>();
                
                playerStats.PutPlayerStats(LBEntries[i].Score.ToString(), LBEntries[i].Username, LBEntries[i].Principal, $"#{i + 1}");
                _playerObjects.Add(playerObject);

                if (LBEntries[i].Principal == ownPrincipal)
                {
                    playerRank.text = $"#{i + 1}";
                    playerName.text = LBEntries[i].Username;
                    playerScore.text = LBEntries[i].Score.ToString();
                }
            }
            foreach (LBEntry entry in LBEntries)
            {
                Debug.Log($"Username: {entry.Username} // Score: {entry.Score} // Principal: {entry.Principal}");
            }
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
        }

        public void SetLeaderboardEntry(string actionM, string score, string userName)
        {
            actionMode = actionM;

            var loginData = UserUtil.GetLogInData().AsOk();

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

            ActionUtil.ProcessAction(actionM, new()
            {
                new Candid.World.Models.Field() { FieldName = "username", FieldValue = string.IsNullOrEmpty(userName)? loginData.principal.SimplifyAddress() : userName },
                new Candid.World.Models.Field() { FieldName = "maxscore", FieldValue = score },
            });
            
                CoroutineManagerUtil.DelayAction(UpdateLeaderboardGameOver, 3f, transform);
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
                LBEntry entries = new LBEntry(username, int.Parse(score), entity.eid, playerPrefab);
                
                LBEntries.Add(entries);
                
            }


            if (LBEntries.Count > 0)
            {
                LBEntries.Sort((x, y) => y.Score.CompareTo(x.Score));
            }


            for (int i = 0; i < LBEntries.Count; i++)
            {
                GameObject playerObject = Instantiate(LBEntries[i].playerStatsPrefab, playerStatsContainer);
                playerObject.transform.SetParent(playerStatsContainer);
                LeaderboardPlayerStats playerStats = playerObject.GetComponent<LeaderboardPlayerStats>();
                
                playerStats.PutPlayerStats(LBEntries[i].Score.ToString(), LBEntries[i].Username, LBEntries[i].Principal, $"#{i + 1}");
                _playerObjects.Add(playerObject);

                if (LBEntries[i].Principal == ownPrincipal)
                {
                    playerRank.text = $"#{i + 1}";
                    playerName.text = LBEntries[i].Username;
                    playerScore.text = LBEntries[i].Score.ToString();

                    Diggy_MiniGame_1.GameManager.Instance.ScoreText.text = LBEntries[i].Score.ToString();
                    Diggy_MiniGame_1.GameManager.Instance.UsernameText.text = LBEntries[i].Username;
                    Diggy_MiniGame_1.GameManager.Instance.RankText.text = $"{i + 1}";

                    return;
                }
            }
        }
    }
}