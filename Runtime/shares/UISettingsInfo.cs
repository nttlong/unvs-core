using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace unvs.shares
{
    [Serializable]
    public struct UISettingsInfo
    {
        public string cinemaSettingPrefabPath;
        /// <summary>
        /// Exmaple "Assets/Prefabs/UI/Hub/Hub.prefab"
        /// </summary>
        public string HubPrefabAddressablePath;
        /// <summary>
        /// example : "Assets/Prefabs/UI/DiscoveryDiialog/DiscoveryDialog.prefab"
        /// </summary>
        public string DiscoveryDialogAddressablePath;
        /// <summary>
        /// Exmaple "Assets/Prefabs/UI/Speaker/Speaker.prefab"
        /// </summary>
        public string SpeakerDialogAddressablePath;
        /// <summary>
        /// Exmaple "Assets/Prefabs/UI/FadeScreenPanel/FadeScreen.prefab"
        /// </summary>
        public string FadeSceenAddressablePath;
        /// <summary>
        /// "Assets/Prefabs/UI/MainMenu/MainMenu.prefab"
        /// </summary>
        public string MainMenuAddressablePath;
        /// <summary>
        /// "Assets/Prefabs/UI/PauseMenu/PauseMenu.prefab"
        /// </summary>
        public string PauseMenuAddressalbePath;
        public string RealtimeStatsAddressablePath;

        public void ValidateOnRequires(UnityEngine.Object owner)
        {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string))
                {
                    var value = (string)field.GetValue(this);
                    if (string.IsNullOrEmpty(value))
                    {
                        Debug.LogError($"[{owner.name}] UISettingsInfo: Field {field.Name} is required.");
                    }
                }
            }
        }
    }
}