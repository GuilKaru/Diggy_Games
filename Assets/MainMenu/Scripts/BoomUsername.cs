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
    }
}
