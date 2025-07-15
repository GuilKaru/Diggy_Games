using Boom;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Boom.Utility;
using Boom.Values;
using UnityEngine;
using Candid.World.Models;

public class BoomScore : MonoBehaviour
{
    [SerializeField] string actionId = "set_max_score_2";
    [SerializeField] private string actionId2 = "score_quest_2";
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void ScoreAction(int score)
    {
        var principal = UserUtil.GetPrincipal();
        
        EntityUtil.TryGetFieldAsText(principal, "score_2", "maxscore", out var outVal, "None");
        
        if(outVal == "None" || outVal == null)
        {
            ExecuteAction(score).Forget();
        }
        else if (int.Parse(outVal) >= score)
        {
            if (score >= 200)
            {
                ExecuteQuest().Forget();
            }
        }
        else
        {
            ExecuteAction(score).Forget();
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private async UniTaskVoid ExecuteAction(int score)
    {
        var newMaxScore = score.ToString();

        List<Field> fields = new()
        {
            new("maxscore", newMaxScore),
        };
        
        var actionResult = await ActionUtil.ProcessAction(actionId, fields);
        
        bool isError = actionResult.IsErr;

        if (isError)
        {
            string errorMessage = actionResult.AsErr().content;

            Debug.LogError(errorMessage);
        }

        if (score >= 200)
        {
            ScoreQuest();
        }
    }

    private void ScoreQuest()
    {
        ExecuteQuest().Forget();
    }

    private async UniTaskVoid ExecuteQuest()
    {
        var actionResult = await ActionUtil.ProcessAction(actionId2);

        bool isError = actionResult.IsErr;

        if (isError)
        {
            string errorMessage = actionResult.AsErr().content;

            Debug.LogError(errorMessage);

            return;
        }

        var expectedResult = actionResult.AsOk();
        
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

            outcomesToDisplay.Add(new(entityName, amount.Value));
        }
    }
}
