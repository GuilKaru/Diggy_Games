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

namespace MainMenu
{
    public class DiggyCoinEntity : MonoBehaviour
    {
        #region FIELDS

        private Coroutine logCoroutine;
        [SerializeField] DiggyCoinPayment diggyCoinPayment;

        //The action ID
        string actionId;
        [SerializeField] private GameObject loadingPanel;
        
        [SerializeField] Button actionButtonx1;
        [SerializeField] Button actionButtonx5;
        [SerializeField] Button actionButtonx10;
        [SerializeField] Button actionButtonx20;

        readonly string diggyIdx1 = "diggy_for_entity_x1";
        readonly string diggyIdx5 = "diggy_for_entity_x5";
        readonly string diggyIdx10 = "diggy_for_entity_x10";
        readonly string diggyIdx20 = "diggy_for_entity_x20";

        private readonly string buyDiggyx1 = "buy_coin_diggy";
        private readonly string buyDiggyx5 = "buy_coin_diggy_x5";
        private readonly string buyDiggyx10 = "buy_coin_diggy_x10";
        private readonly string buyDiggyx20 = "buy_coin_diggy_x20";
        #endregion

        #region ACTION

        private void Awake()
        {
            //This is to clear out any unwanted listener
            actionButtonx1.onClick.RemoveAllListeners();
            actionButtonx5.onClick.RemoveAllListeners();
            actionButtonx10.onClick.RemoveAllListeners();
            actionButtonx20.onClick.RemoveAllListeners();

            //Register to action button click
            actionButtonx1.onClick.AddListener(() => ActionButtonClickHandler(diggyIdx1, buyDiggyx1));
            actionButtonx5.onClick.AddListener(() => ActionButtonClickHandler(diggyIdx5, buyDiggyx5));
            actionButtonx10.onClick.AddListener(() => ActionButtonClickHandler(diggyIdx10, buyDiggyx10));
            actionButtonx20.onClick.AddListener(() => ActionButtonClickHandler(diggyIdx20, buyDiggyx20));

            //We register LoginDataChangeHandler to MainDataTypes.LoginData change event to initialize userNameInputField with the user's username
            //UserUtil.AddListenerMainDataChange<MainDataTypes.LoginData>(LoginDataChangeHandler);

            //We register EntiyDataChangeHandler to the user's DataTypes.Entity change event to update inventoryText with the user's entities
            //UserUtil.AddListenerDataChangeSelf<DataTypes.Entity>(EntiyDataChangeHandler);

            //We register TokenDataChangeHandler to the user's DataTypes.Token change event to update userBalances field with the user's balance
            //UserUtil.AddListenerDataChangeSelf<DataTypes.Token>(TokenDataChangeHandler);
        }

        private void OnDestroy()
        {
            //Unregister to action button click
            actionButtonx1.onClick.RemoveListener(() => ActionButtonClickHandler(diggyIdx1, buyDiggyx1));
            actionButtonx5.onClick.RemoveListener(() => ActionButtonClickHandler(diggyIdx5, buyDiggyx5));
            actionButtonx10.onClick.RemoveListener(() => ActionButtonClickHandler(diggyIdx10, buyDiggyx10));
            actionButtonx20.onClick.RemoveListener(() => ActionButtonClickHandler(diggyIdx20, buyDiggyx20));

            //We unregister from MainDataTypes.LoginData change event
            //UserUtil.RemoveListenerMainDataChange<MainDataTypes.LoginData>(LoginDataChangeHandler);

            //We unregister from DataTypes.Entity change event
            //UserUtil.RemoveListenerDataChangeSelf<DataTypes.Entity>(EntiyDataChangeHandler);

            //We unregister from DataTypes.Token change event
            //UserUtil.RemoveListenerDataChangeSelf<DataTypes.Token>(TokenDataChangeHandler);
        }

        private void OnEnable()
        {
            Debug.Log("...");
        }
        //This function is just a wrapper so that we can register "ExecuteAction" function on the Action Button's onClick event
        public void ActionButtonClickHandler(string action, string diggyId)
        {
            if (diggyId == "x1" || diggyId == "x5" || diggyId == "x10" || diggyId == "x20")
            {
                //Error when payment fails
                ExecuteDecreaseAction(action);
            }
            else
            {
                //Forget() is included as we dont care awaiting for the result
                ExecuteAction(action, diggyId).Forget();
            }
        }

        private async UniTaskVoid ExecuteAction(string action, string diggyId)
        {
            loadingPanel.SetActive(true);
            
            actionId = action;
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            //SECTION A: Action execution

            //Here we execute the action by passing the actionId we want
            //to execute, in this case it is "add_gem"
            //actionLogText.text = $"Processing Action of id: {actionId}";
            var actionResult = await ActionUtil.ProcessAction(actionId);

            //SECTION B: Error handling

            //Here we handle the errors
            bool isError = actionResult.IsErr;

            if (isError)
            {
                string errorMessage = actionResult.AsErr().content;

                Debug.LogError(errorMessage);
                logCoroutine = StartCoroutine(DisplayTempLog(errorMessage));
                
                loadingPanel.SetActive(false);
                return;
            }

            //SECTION C: Cast action result to the OK return type (action outcomes)

            //This is the result if the execution of the action was successful
            var expectedResult = actionResult.AsOk();

            //SECTION D: Break down the caller action outcome and console log them

            //We get the outcomes of the user who executed the action
            var callerOutcomes = expectedResult.callerOutcomes;

            //This are the entity outcomes of the user who executed the action
            var entityOutcomes = callerOutcomes.entityOutcomes;

            string message = "";
            List<KeyValue<string, double>> outcomesToDisplay = new();

            //We loop through all the entity outcomes
            foreach (var keyValue in entityOutcomes)
            {
                var entityOutcome = keyValue.Value;

                string entityId = entityOutcome.eid;

                //Try get the entity's config field's value
                bool configEntityNameFound = entityOutcome.TryGetConfigFieldAs<string>
                    //World Id
                    (BoomManager.Instance.WORLD_CANISTER_ID,
                        //Config's field name
                        "name",
                        //result
                        out string entityName,
                        //default value of the result
                        "None");

                //If config doesn't exist for the entity we just skip it
                if (configEntityNameFound == false)
                {
                    $"Could not find the config field name of entityId: {entityId}".Warning();
                    continue;
                }

                string editedFieldName = "amount";

                bool fieldAmountFound = entityOutcome.TryGetOutcomeFieldAsDouble(editedFieldName, out var amount);

                //If the amount field is not found on the entity we just display an error
                if (fieldAmountFound == false)
                {
                    message =
                        $"Could not find the edited entity field's name : {editedFieldName},  of entityId: {entityId}";
                    message.Warning();
                    break;
                }

                outcomesToDisplay.Add(new(entityName, amount.Value));
            }

            //if (string.IsNullOrEmpty(message)) message = $"Rewards:\n\n{outcomesToDisplay.Reduce(e => $"> +{e.value} {e.key}", "\n")}";
            
            logCoroutine = StartCoroutine(DisplayTempLog(message));
            
            diggyCoinPayment.ActionButtonClickHandler(diggyId);
            
        }
        
        private async UniTaskVoid ExecuteDecreaseAction(string action)
        {
            actionId = action;
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            //SECTION A: Action execution

            //Here we execute the action by passing the actionId we want
            //to execute, in this case it is "add_gem"
            //actionLogText.text = $"Processing Action of id: {actionId}";
            var actionResult = await ActionUtil.ProcessAction(actionId);

            //SECTION B: Error handling

            //Here we handle the errors
            bool isError = actionResult.IsErr;

            if (isError)
            {
                string errorMessage = actionResult.AsErr().content;

                Debug.LogError(errorMessage);
                logCoroutine = StartCoroutine(DisplayTempLog(errorMessage));
                
                loadingPanel.SetActive(false);
                return;
            }

            //SECTION C: Cast action result to the OK return type (action outcomes)

            //This is the result if the execution of the action was successful
            var expectedResult = actionResult.AsOk();

            //SECTION D: Break down the caller action outcome and console log them

            //We get the outcomes of the user who executed the action
            var callerOutcomes = expectedResult.callerOutcomes;

            //This are the entity outcomes of the user who executed the action
            var entityOutcomes = callerOutcomes.entityOutcomes;

            string message = "";
            List<KeyValue<string, double>> outcomesToDisplay = new();

            //We loop through all the entity outcomes
            foreach (var keyValue in entityOutcomes)
            {
                var entityOutcome = keyValue.Value;

                string entityId = entityOutcome.eid;

                //Try get the entity's config field's value
                bool configEntityNameFound = entityOutcome.TryGetConfigFieldAs<string>
                    //World Id
                    (BoomManager.Instance.WORLD_CANISTER_ID,
                        //Config's field name
                        "name",
                        //result
                        out string entityName,
                        //default value of the result
                        "None");

                //If config doesn't exist for the entity we just skip it
                if (configEntityNameFound == false)
                {
                    $"Could not find the config field name of entityId: {entityId}".Warning();
                    continue;
                }

                string editedFieldName = "amount";

                bool fieldAmountFound = entityOutcome.TryGetOutcomeFieldAsDouble(editedFieldName, out var amount);

                //If the amount field is not found on the entity we just display an error
                if (fieldAmountFound == false)
                {
                    message =
                        $"Could not find the edited entity field's name : {editedFieldName},  of entityId: {entityId}";
                    message.Warning();
                    break;
                }

                outcomesToDisplay.Add(new(entityName, amount.Value));
            }

            //if (string.IsNullOrEmpty(message)) message = $"Rewards:\n\n{outcomesToDisplay.Reduce(e => $"> +{e.value} {e.key}", "\n")}";
            
            logCoroutine = StartCoroutine(DisplayTempLog(message));
            
            loadingPanel.SetActive(false);
        }
        #endregion


        #region LOG

        IEnumerator DisplayTempLog(string message, float duration = 5f)
        {
            //actionLogText.text = message;
            Debug.Log(message);
            yield return new WaitForSeconds(duration);
            Debug.Log("...");
            //actionLogText.text = "...";
        }

        #endregion
    }
}
