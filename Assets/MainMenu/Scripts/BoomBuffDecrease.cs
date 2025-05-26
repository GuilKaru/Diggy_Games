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
    public class BoomBuffDecrease : MonoBehaviour
    {
        #region FIELDS

        private Coroutine logCoroutine;

        //The action ID
        string actionId;
        [SerializeField] private GameObject loadingPanel;
        #endregion

        #region ACTION
        
        //This function is just a wrapper so that we can register "ExecuteAction" function on the Action Button's onClick event
        public void ActionHandler(string action)
        {
            actionId = action;
            //Forget() is included as we dont care awaiting for the result
            ExecuteAction().Forget();
        }

        private async UniTaskVoid ExecuteAction()
        {
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

            //logCoroutine = StartCoroutine(DisplayTempLog(message));
            
            GameManager.instance.boomUsername.UpdateCoins();
        }
        
        public async UniTaskVoid PlayCoinsDecrease(string playAction, int game)
        {
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            //SECTION A: Action execution

            //Here we execute the action by passing the actionId we want
            //to execute, in this case it is "add_gem"
            //actionLogText.text = $"Processing Action of id: {actionId}";
            var actionResult = await ActionUtil.ProcessAction(playAction);

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

            //logCoroutine = StartCoroutine(DisplayTempLog(message));
            if (game == 1)
            {
                GameManager.instance.sceneController.PlayGameFF();
            }
            else if (game == 2)
            {
                GameManager.instance.sceneController.PlayGameDD();
            }
            
            GameManager.instance.boomUsername.UpdateCoins();
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

