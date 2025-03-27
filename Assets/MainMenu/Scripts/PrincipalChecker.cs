using System;
using UnityEngine;
using System.Collections.Generic;
using EdjCase.ICP.Candid.Models;
using Newtonsoft.Json;
using Boom;

namespace MainMenu
{
    public class PrincipalChecker : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] public string currentPrincipalId;

        private List<string> loadedPrincipals;
        private List<string> extraPrincipals = new List<string>();

        public void CreatePrincipalList()
        {
            ConfigUtil.TryGetConfig("bg4su-6iaaa-aaaap-anxsa-cai", "whitelistExtras", out var outConfig);
            extraPrincipals.AddRange(outConfig.fields.Values);
            
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

        public void CheckPrincipal()
        {
            ConfigUtil.TryGetConfig("bg4su-6iaaa-aaaap-anxsa-cai", "whitelistExtras", out var outConfig);
            if (outConfig != null && outConfig.fields != null)
            {
                extraPrincipals.Clear();
                extraPrincipals.AddRange(outConfig.fields.Values);
            }
            
            if (loadedPrincipals.Contains(currentPrincipalId) || extraPrincipals.Contains(currentPrincipalId))
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
            Debug.Log("Principal found in first Whitelist");
            GameManager.instance.mainMenu.playerWhitelisted = true;
        }

        private void PrincipalNotFound()
        {
            Debug.Log("Principal not found in first Whitelist");
            GameManager.instance.mainMenu.playerWhitelisted = false;
        }
    }
}
