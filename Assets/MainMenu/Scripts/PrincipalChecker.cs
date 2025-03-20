using System;
using UnityEngine;
using System.Collections.Generic;
using EdjCase.ICP.Candid.Models;
using Newtonsoft.Json;

namespace MainMenu
{
    public class PrincipalChecker : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] public string currentPrincipalId;

        private List<string> loadedPrincipals;

        public void CreatePrincipalList()
        {
            if (jsonFile == null)
            {
                Debug.LogWarning("No json file selected");
                return;
            }

            if (string.IsNullOrEmpty(currentPrincipalId))
            {
                Debug.LogWarning("No valid principal selected");
                return;
            }

            try
            {
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonFile.text);
                if (jsonObject != null && jsonObject.ContainsKey("principals"))
                {
                    loadedPrincipals = jsonObject["principals"];
                }
                else
                {
                    Debug.LogWarning("Invalid JSON format or missing 'principals' key");
                    return;
                }
            }
            catch (JsonException e)
            {
                Debug.LogWarning($"Error parsing JSON: {e.Message}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"An unexpected error occurred: {e.Message}");
            }
        }

        public void CheckPrincipals()
        {
            if (loadedPrincipals.Contains(currentPrincipalId))
            {
                PrincipalFound();
            }
            else
            {
                PrincipalNotFound();
            }
        }

        private void PrincipalFound()
        {
            Debug.Log("Principal found");
            GameManager.instance.mainMenu.playerWhitelisted = true;
        }

        private void PrincipalNotFound()
        {
            Debug.Log("Principal not found");
            GameManager.instance.mainMenu.playerWhitelisted = false;
        }
    }
}
