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
using Candid.WorldDeployer.Models;
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


            ConfigUtil.TryGetConfig("bg4su-6iaaa-aaaap-anxsa-cai", "diggy_price", out var outConfig);
            
            outConfig.fields.TryGetValue("pricex10", out  var pricex10);
            outConfig.fields.TryGetValue("pricex50", out var pricex50);
            outConfig.fields.TryGetValue("pricex100", out var pricex100);
            outConfig.fields.TryGetValue("pricex200", out var pricex200);

            GameManager.instance.mainMenu.UpdatePriceStore(pricex10, pricex50, pricex100, pricex200);
            
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
            
            EntityUtil.TryGetFieldAsDouble(principal, "diggycoin", "amount", out var diggyCoinD, 0);
            
            EntityUtil.TryGetFieldAsDouble(principal, "sweepbuff", "amount", out var sweepBuffD, 0);
            EntityUtil.TryGetFieldAsDouble(principal, "timebuff", "amount", out var timeBuffD, 0);
            EntityUtil.TryGetFieldAsDouble(principal, "rockbuff", "amount", out var rockBuffD, 0);
            EntityUtil.TryGetFieldAsDouble(principal, "shieldbuff", "amount", out var shieldBuffD, 0);
            EntityUtil.TryGetFieldAsDouble(principal, "triplebuff", "amount", out var tripleBuffD, 0);
            
            GameManager.instance.mainMenu.CoinsSafe(diggyCoinD, sweepBuffD, timeBuffD, rockBuffD, shieldBuffD, tripleBuffD);

        }
    }
}
