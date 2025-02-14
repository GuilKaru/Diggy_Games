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
using EdjCase.ICP.Candid.Models;

namespace MainMenu
{
    public class BoomUsername : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private Button actionButton;

        private Coroutine logCoroutine;

        [SerializeField] private string actionId = "set_username";

        private void Awake()
        {
            AutoNameLogin();
            
            actionButton.onClick.RemoveAllListeners();

            actionButton.onClick.AddListener(ActionButtonClickHandler);
        }

        private void OnDestroy()
        {
            actionButton.onClick.RemoveListener(ActionButtonClickHandler);
        }

        public void ActionButtonClickHandler()
        {
            ExecuteAction().Forget();
        }

        public async UniTaskVoid ExecuteAction()
        {
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            var newUsername = _usernameInputField.text;
            
            if (string.IsNullOrEmpty(newUsername)) return;

            List<Field> fields = new()
            {
                new Field("username", newUsername),
            };

            var actionResult = await ActionUtil.ProcessAction(actionId, fields);
            
            //Update Score 

            bool isError = actionResult.IsErr;

            if (isError)
            {
                string errorMessage = actionResult.AsErr().content;
                
                Debug.LogError(errorMessage);

                return;
            }
            else
            {
                GameManager.instance.mainMenu.NameSafe(newUsername);
            }
        }

        private void AutoNameLogin()
        {
            //Update Score
            
            var principal = UserUtil.GetPrincipal();
            
            EntityUtil.TryGetFieldAsText(principal, "user_profile", "username", out var outVal, "None");
            
            UpdateCoins();
            UpdateUsername(outVal);
        }

        private void UpdateUsername(string value)
        {
            if (value is "None" or null)
            {
                return;
            }
            else
            {
                GameManager.instance.mainMenu.NameSafe(value);
            }
        }

        public void UpdateCoins()
        {

            var principal = UserUtil.GetPrincipal();
            
            double diggyCoin;

            float sweepBuff;
            float timeBuff;
            float rockBuff;
            float shieldBuff;
            float tripleBuff;
            
            EntityUtil.TryGetFieldAsText(principal, "diggycoin", "amount", out var diggyCoinS, "None");
            
            EntityUtil.TryGetFieldAsText(principal, "sweepbuff", "amount", out var sweepBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "timebuff", "amount", out var timeBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "rockbuff", "amount", out var rockBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "shieldbuff", "amount", out var shieldBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "triplebuff", "amount", out var tripleBuffS, "None");

            if (diggyCoinS is "None" or null)
            {
                diggyCoin = 0;
            }
            else
            {
                diggyCoin = double.Parse(diggyCoinS);
            }

            if (sweepBuffS is "None" or null)
            {
                sweepBuff = 0;
            }
            else
            {
                sweepBuff = float.Parse(sweepBuffS);
            }

            if (timeBuffS is "None" or null)
            {
                timeBuff = 0;
            }
            else
            {
                timeBuff = float.Parse(timeBuffS);
            }

            if (rockBuffS is "None" or null)
            {
                rockBuff = 0;
            }
            else
            {
                rockBuff = float.Parse(rockBuffS);
            }

            if (shieldBuffS is "None" or null)
            {
                shieldBuff = 0;
            }
            else
            {
                shieldBuff = float.Parse(shieldBuffS);
            }

            if (tripleBuffS is "None" or null)
            {
                tripleBuff = 0;
            }
            else
            {
                tripleBuff = float.Parse(tripleBuffS);
            }
            
            GameManager.instance.mainMenu.CoinsSafe(diggyCoin, sweepBuff, timeBuff, rockBuff, shieldBuff, tripleBuff);

        }
    }
}
