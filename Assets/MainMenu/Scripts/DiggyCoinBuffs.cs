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
    public class DiggyCoinBuffs : MonoBehaviour
    {
        #region FIELDS

        private Coroutine logCoroutine;
        
        [SerializeField] Button actionButtonSweep;
        [SerializeField] Button actionButtonTime;
        [SerializeField] Button actionButtonRock;
        [SerializeField] Button actionButtonShield;
        [SerializeField] Button actionButtonTriple;


        //The action ID
        string actionId;

        readonly string actionSweep = "buy_sweep_buff";
        readonly string actionTime = "buy_time_buff";
        readonly string actionRock = "buy_rock_buff";
        readonly string actionShield = "buy_shield_buff";
        readonly string actionTriple  = "buy_triple_buff";

        [SerializeField] private GameObject loadingPanel;
        #endregion

        #region ACTION

         private void Awake()
        {
            //This is to clear out any unwanted listener
            actionButtonSweep.onClick.RemoveAllListeners();
            actionButtonTime.onClick.RemoveAllListeners();
            actionButtonRock.onClick.RemoveAllListeners();
            actionButtonShield.onClick.RemoveAllListeners();
            actionButtonTriple.onClick.RemoveAllListeners();

            //Register to action button click
            actionButtonSweep.onClick.AddListener(() => ActionButtonClickHandler(actionSweep));
            actionButtonTime.onClick.AddListener(() => ActionButtonClickHandler(actionTime));
            actionButtonRock.onClick.AddListener(() => ActionButtonClickHandler(actionRock));
            actionButtonShield.onClick.AddListener(() => ActionButtonClickHandler(actionShield));
            actionButtonTriple.onClick.AddListener(() => ActionButtonClickHandler(actionTriple));
            
        }

        private void OnDestroy()
        {
            //Unregister to action button click
            actionButtonSweep.onClick.RemoveListener(() => ActionButtonClickHandler(actionSweep));
            actionButtonTime.onClick.RemoveListener(() => ActionButtonClickHandler(actionTime));
            actionButtonRock.onClick.RemoveListener(() => ActionButtonClickHandler(actionRock));
            actionButtonShield.onClick.RemoveListener(() => ActionButtonClickHandler(actionShield));
            actionButtonTriple.onClick.RemoveListener(() => ActionButtonClickHandler(actionTriple));
            
        }

        private void OnEnable()
        {
            //actionLogText.text = "...";
            Debug.Log("...");
        }
        
        //This function is just a wrapper so that we can register "ExecuteAction" function on the Action Button's onClick event
        private void ActionButtonClickHandler(string action)
        {
            actionId = action;
            CoinCheck();
            //Forget() is included as we dont care awaiting for the result
            //ExecuteAction().Forget();
        }

        private void CoinCheck()
        {
            var principal = UserUtil.GetPrincipal();

            EntityUtil.TryGetFieldAsText(principal, "diggycoin", "amount", out var diggyCoinAmount, "None");

            if (diggyCoinAmount is "None" or null)
            {
                Debug.Log("Not enough Diggy Coins");
                loadingPanel.SetActive(false);
            }
            else
            {
                double diggyCoin = double.Parse(diggyCoinAmount);

                if (diggyCoin <= 0)
                {
                    Debug.Log("Not enough Diggy Coins");
                    loadingPanel.SetActive(false);
                }
                else if (actionId == "buy_sweep_buff" && diggyCoin >= 5)
                {
                    ExecuteAction().Forget();
                }
                else if (actionId == "buy_time_buff" && diggyCoin >= 4)
                {
                    ExecuteAction().Forget();
                }
                else if (actionId == "buy_rock_buff" && diggyCoin >= 3)
                {
                    ExecuteAction().Forget();
                }
                else if (actionId == "buy_shield_buff" && diggyCoin >= 2)
                {
                    ExecuteAction().Forget();
                }
                else if (actionId == "buy_triple_buff" && diggyCoin >= 1)
                {
                    ExecuteAction().Forget();
                }
            }
        }

        private async UniTaskVoid ExecuteAction()
        {
            loadingPanel.SetActive(true);
            
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
            loadingPanel.SetActive(false);
            GameManager.instance.boomUsername.UpdateCoins();
            //logCoroutine = StartCoroutine(DisplayTempLog(message));
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
