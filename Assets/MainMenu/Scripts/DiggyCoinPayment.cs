using Boom;
using System.Collections;
using System.Collections.Generic;
using Boom.Utility;
using Boom.Values;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class DiggyCoinPayment : MonoBehaviour
    {
        
        [SerializeField] DiggyCoinEntity diggyCoinEntity;
        #region FIELDS

        //This is the a coroutine cache from displaying the logs. It is used to stop it when required.
        private Coroutine logCoroutine;

        //The action ID
        string actionId = "buy_coin_diggy";
        string diggyId = "diggy_for_entity_x1";
        
        readonly string decreasedcx1 = "decrease_dc_x1";
        readonly string decreasedcx5 = "decrease_dc_x5";
        readonly string decreasedcx10 = "decrease_dc_x10";
        readonly string decreasedcx20 = "decrease_dc_x20";

        //string[] entitiesToDisplayOnTheInventory = new string[1] { "coin" };

        string inventoryContent = "";
        string userBalance = "";
        
        [SerializeField] private GameObject loadingPanel;

        #endregion


        #region MONO

        private void OnEnable()
        {
            //actionLogText.text = "...";
            Debug.Log("...");
        }
        #endregion


        #region ACTION
        
        //This function is just a wrapper so that we can register "ExecuteAction" function on the Action Button's onClick event
        public void ActionButtonClickHandler(string newActionId)
        {
            actionId = newActionId;
            //Forget() is included as we dont care awaiting for the result
            ExecuteAction().Forget();
        }

        private async UniTaskVoid ExecuteAction()
        {
            loadingPanel.SetActive(true);
            
            if (logCoroutine != null) StopCoroutine(logCoroutine);

            //SECTION A: Action execution

            //Here we execute the action by passing the actionId we want to execute.
            //actionLogText.text = $"Processing Action of id: {actionId}";
            Debug.Log($"Processing action: {actionId}");

            var actionResult = await ActionUtil.ProcessAction(actionId);

            //SECTION B: Error handling

            //Here we handle the errors
            bool isError = actionResult.IsErr;

            if (isError)
            {
                string errorMessage = actionResult.AsErr().content;
                Debug.LogError(errorMessage);
                
                //Not Enough Funds
                logCoroutine = StartCoroutine(DisplayTempLog(errorMessage));

                if (actionId == "buy_coin_diggy")
                {
                    diggyCoinEntity.ActionButtonClickHandler(decreasedcx1, "x1");
                }
                else if (actionId == "buy_coin_diggy_x5")
                {
                    diggyCoinEntity.ActionButtonClickHandler(decreasedcx5, "x5");
                }
                else if (actionId == "buy_coin_diggy_x10")
                {
                    diggyCoinEntity.ActionButtonClickHandler(decreasedcx10, "x10");
                }
                else if (actionId == "buy_coin_diggy_x20")
                {
                    diggyCoinEntity.ActionButtonClickHandler(decreasedcx20, "x20");
                }
                
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
                    message = $"Could not find the edited entity field's name : {editedFieldName},  of entityId: {entityId}";
                    message.Warning();
                    break;
                }

                //if (amount.NumericType_ == EntityFieldEdit.Numeric.NumericType.Increment)
                //{
                    outcomesToDisplay.Add(new(entityName, amount.Value));
                //}
            }

            //if (string.IsNullOrEmpty(message)) message = $"Rewards:\n\n{outcomesToDisplay.Reduce(e => $"> +{e.value} {e.key}", "\n")}";
            GameManager.instance.boomUsername.UpdateCoins();
            loadingPanel.SetActive(false);
            //logCoroutine = StartCoroutine(DisplayTempLog(message));
            
            //Purchase Successful
        }

        #endregion


        #region LOG
        IEnumerator DisplayTempLog(string message, float duration = 2.5f)
        {
            //actionLogText.text = message;
            Debug.Log(message);
            yield return new WaitForSeconds(duration);
           // actionLogText.text = "...";
           Debug.Log($"...");
        }
        #endregion


        #region USER DATA

        public void LoginDataChangeHandler()
        {
            //If user is not logged in, return
            //if (data.state != MainDataTypes.LoginData.State.LoggedIn) return;

            //Update Inventory UI with Entities
            var allUserDataResult = UserUtil.GetAllDataSelf();

            if (allUserDataResult.IsErr)
            {
                $"{allUserDataResult.AsErr()}".Error(typeof(DiggyCoinPayment).Name);
                return;
            }

            var allUserDataAsOk = allUserDataResult.AsOk();

            //EntiyDataChangeHandler(allUserDataAsOk.entityData);
            TokenDataChangeHandler(allUserDataAsOk.tokenData);
        }

        private void TokenDataChangeHandler(Data<DataTypes.Token> data)
        {
            //Update action button
            string diggyBalance = "0";

            //actionButton.interactable = ActionUtil.ValidateConstraint(actionId);
            userBalance = data.elements.Reduce(e =>
            {
                var balance = e.Value;
                
                if (balance.TryGetTokenConfig(out var tokenConfig) == false)
                {
                    $"Could not find config for token canister ID: {balance.canisterId}".Warning(typeof(DiggyCoinPayment).Name);
                    return "";
                }

                if (balance.canisterId == "dfg2l-2yaaa-aaaap-akpsa-cai")
                {
                    diggyBalance = $"{TokenUtil.ConvertToDecimal(balance.baseUnitAmount, tokenConfig.decimals)}";
                    return  $"{TokenUtil.ConvertToDecimal(balance.baseUnitAmount, tokenConfig.decimals)}";
                }

                return null;
            });

            GameManager.instance.playerData.diggys = diggyBalance;
            GameManager.instance.mainMenu._diggys.text = diggyBalance;
            //UpdateInventoryText();
        }
        /*
        private void EntiyDataChangeHandler(Data<DataTypes.Entity> data)
        {
            //Update Inventory
            string entitiesToDisplay = "";

            foreach (var entityId in entitiesToDisplayOnTheInventory)
            {
                foreach (var keyValue in data.elements)
                {
                    var entity = keyValue.Value;

                    if (entity.eid == entityId)
                    {
                        //Try get the entity's config field's value
                        bool configEntityNameFound = ConfigUtil.TryGetConfigFieldAs<string>
                            //World Id
                            (BoomManager.Instance.WORLD_CANISTER_ID,
                            //ConfigId. In this case we are looking for the config of an entity, therefore, we use the entityId
                            entityId,
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
                            break;
                        }


                        string fieldName = "amount";

                        bool fieldAmountFound = entity.TryGetFieldAsDouble(fieldName, out var amount);

                        //If the amount field is not found on the entity we just display an error
                        if (fieldAmountFound == false)
                        {
                            $"Could not find the entity field's name : {fieldName},  of entityId: {entityId}".Warning();
                            break;
                        }

                        entitiesToDisplay += $"> {entityName} x {amount}\n";

                        break;
                    }
                }
            }

            inventoryContent = entitiesToDisplay;

            UpdateInventoryText();
        }

        private void UpdateInventoryText()
        {
            inventoryText.text = $"Balance\n-------\n{userBalance}\n\n=======\nInventory\n-------\n{inventoryContent}";
        }*/
        #endregion
    }
}