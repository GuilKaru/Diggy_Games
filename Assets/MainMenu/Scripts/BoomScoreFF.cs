using Boom.Utility;
using Boom.Values;
using Boom;
using Candid;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Candid.World.Models;
using Newtonsoft.Json;

namespace MainMenu
{
    public class BoomScoreFF : MonoBehaviour
    {
        #region FIELDS
        //This is the button that triggers the action "set_username"

        //This is the a coroutine cache from displaying the logs. It is used to stop it when required.
        private Coroutine logCoroutine;

        //The action ID
        [SerializeField] string actionId1 = "set_max_score_1";
        [SerializeField] string actionId2 = "set_max_score_2";

        #endregion


        #region ACTION 

        //This function is just a wrapper so that we can register "ExecuteAction" function on the Action Button's onClick event
        public void ActionButtonClickHandler(int score, int game)
        {
            //Forget() is included as we dont care awaiting for the result
            MaxScoreCheck(score, game);
        }

        private void MaxScoreCheck(int score, int game)
        {
            var principal = UserUtil.GetPrincipal();
            
            if (game == 1)
            {
                EntityUtil.TryGetFieldAsText(principal, "score_1", "maxscore", out var outVal, "None");
                
                if(outVal == "None" || outVal == null)
                {
                    ExecuteAction(score, game).Forget();
                }
                else
                {
                    int oldScore = int.Parse(outVal);
                
                    score += oldScore;
                
                    ExecuteAction(score, game).Forget();
                }
            }
            else
            {
                EntityUtil.TryGetFieldAsText(principal, "score_2", "maxscore", out var outVal, "None");
                
                if(outVal == "None" || outVal == null)
                {
                    ExecuteAction(score, game).Forget();
                }
                else
                {
                    int oldScore = int.Parse(outVal);
                
                    score += oldScore;
                
                    ExecuteAction(score, game).Forget();
                }
            }
        }
        
        

        public async UniTaskVoid ExecuteAction(int score, int game)
        {
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            //SECTION A: Set up arguments

            var newMaxScore = score.ToString();

            //if (string.IsNullOrEmpty(newUsername)) return;

            List<Field> fields = new()
            {
                new("maxscore", newMaxScore),
                //new("animal", newUsername),
            };


            //SECTION B: Action execution

            //Here we execute the action by passing the actionId we wantto execute.
            //actionLogText.text = $"Processing Action of id: \"{actionId}\" with arguments:\n{JsonConvert.SerializeObject(fields)}";
            if (game == 1)
            {
                var actionResult = await ActionUtil.ProcessAction(actionId1, fields);
                
                bool isError = actionResult.IsErr;

                if (isError)
                {
                    string errorMessage = actionResult.AsErr().content;

                    Debug.LogError(errorMessage);
                    //logCoroutine = StartCoroutine(DisplayTempLog(errorMessage));

                    return;
                }
            
                //GameManager.instance.playerData.furnaceFrenzyMaxScore = score;
                GameManager.instance.ScoreUpdateFF();
                GameManager.instance.boomLeaderboard.SetLeaderboardEntry("set_leaderboard_1", score.ToString(), GameManager.instance.playerData.username, game);
            }
            else
            {
                var actionResult = await ActionUtil.ProcessAction(actionId2, fields);
                
                bool isError = actionResult.IsErr;

                if (isError)
                {
                    string errorMessage = actionResult.AsErr().content;

                    Debug.LogError(errorMessage);
                    //logCoroutine = StartCoroutine(DisplayTempLog(errorMessage));

                    return;
                }
            
                //GameManager.instance.playerData.furnaceFrenzyMaxScore = score;
                GameManager.instance.ScoreUpdateDD();
                GameManager.instance.boomLeaderboard.SetLeaderboardEntry("set_leaderboard_1", score.ToString(), GameManager.instance.playerData.username, game);
            }
            
            
        }

        #endregion
    }
}