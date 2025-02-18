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
        [SerializeField] private DiggyCoinPayment diggyCoinPayment;

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
            diggyCoinPayment.LoginDataChangeHandler();
            var principal = UserUtil.GetPrincipal();
            
            /*float diggyCoin;

            float sweepBuff;
            float timeBuff;
            float rockBuff;
            float shieldBuff;
            float tripleBuff;*/
            
            EntityUtil.TryGetFieldAsText(principal, "diggycoin", "amount", out var diggyCoinS, "None");
            
            EntityUtil.TryGetFieldAsText(principal, "sweepbuff", "amount", out var sweepBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "timebuff", "amount", out var timeBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "rockbuff", "amount", out var rockBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "shieldbuff", "amount", out var shieldBuffS, "None");
            EntityUtil.TryGetFieldAsText(principal, "triplebuff", "amount", out var tripleBuffS, "None");

            if (diggyCoinS is "None" or null)
            {
                diggyCoinS = "0";
            }
            /*else
            {
                diggyCoin = float.TryParse(diggyCoinS, out diggyCoin) ? diggyCoin : 0;
            }*/

            if (sweepBuffS is "None" or null)
            {
                sweepBuffS = "0";
            }
            /*else
            {
                sweepBuff = float.TryParse(sweepBuffS, out sweepBuff) ? sweepBuff : 0;
            }*/

            if (timeBuffS is "None" or null)
            {
                timeBuffS = "0";
            }
            /*else
            {
                timeBuff = float.TryParse(timeBuffS, out timeBuff) ? timeBuff : 0;
            }*/

            if (rockBuffS is "None" or null)
            {
                rockBuffS = "0";
            }
            /*else
            {
                rockBuff = float.TryParse(rockBuffS, out rockBuff) ? rockBuff : 0;
            }*/

            if (shieldBuffS is "None" or null)
            {
                shieldBuffS = "0";
            }
            /*else
            {
                shieldBuff = float.TryParse(shieldBuffS, out shieldBuff) ? shieldBuff : 0;
            }*/

            if (tripleBuffS is "None" or null)
            {
                tripleBuffS = "0";
            }
            /*else
            {
                tripleBuff = float.TryParse(tripleBuffS, out tripleBuff) ? tripleBuff : 0;
            }*/
            
            GameManager.instance.mainMenu.CoinsSafe(diggyCoinS, sweepBuffS, timeBuffS, rockBuffS, shieldBuffS, tripleBuffS);

        }
    }
}
