//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections;
//using System.Reflection;
//using System.Threading.Tasks;
//using UnityEngine;
//using unvs.gameworld;
//using unvs.interfaces;
//using unvs.sys;
//using unvs.ui;

//namespace unvs.shares
//{
    
//    [Serializable]
//    public class UISettingsInfo
//    {

//        public SettingsUIMainMenu SettingsUIMainMenu =null;
//        public SettingsUIPauseMenu SettingsUIPauseMenu = null;
//        public SettingsCinema Cinema = null;
//        public SettingsUIHub Hub = null;
//        public SettingsUIDiscoveryDialog UIDiscoveryDialog = null;
//        public SettingsUISpeaker SettingsUISpeaker = null;
//        public SettingsUIFadeScreen UIFadeScreen = null;
//        public SettingsRealTimeStats RealTimeStats = null;

//        public string cinemaSettingPrefabPath;
//        /// <summary>
//        /// Exmaple "Assets/Prefabs/UI/Hub/Hub.prefab"
//        /// </summary>
//        public string HubPrefabAddressablePath;
//        /// <summary>
//        /// example : "Assets/Prefabs/UI/DiscoveryDiialog/DiscoveryDialog.prefab"
//        /// </summary>
//        public string DiscoveryDialogAddressablePath;
//        /// <summary>
//        /// Exmaple "Assets/Prefabs/UI/Speaker/Speaker.prefab"
//        /// </summary>
//        public string SpeakerDialogAddressablePath;
//        /// <summary>
//        /// Exmaple "Assets/Prefabs/UI/FadeScreenPanel/FadeScreen.prefab"
//        /// </summary>
//        public string FadeSceenAddressablePath;
//        /// <summary>
//        /// "Assets/Prefabs/UI/MainMenu/MainMenu.prefab"
//        /// </summary>
//        public string MainMenuAddressablePath;
//        /// <summary>
//        /// "Assets/Prefabs/UI/PauseMenu/PauseMenu.prefab"
//        /// </summary>
//        public string PauseMenuAddressalbePath;
//        public string RealtimeStatsAddressablePath;

        

//        public void ValidateOnRequires(UnityEngine.Object owner)
//        {
//            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
//            foreach (var field in fields)
//            {
//                if (field.FieldType == typeof(string))
//                {
//                    var value = (string)field.GetValue(this);
//                    if (string.IsNullOrEmpty(value))
//                    {
//                        Debug.LogError($"[{owner.name}] UISettingsInfo: Field {field.Name} is required.");
//                    }
//                }
//            }
//        }
//        public async UniTask InitGameSettingsAsync(Transform owner)
//        {
//            var task1 = Commons.LoadPrefabsAsync<SettingsCinema>(cinemaSettingPrefabPath, owner,true);
//            var task2 = Commons.LoadPrefabsAsync<SettingsUIMainMenu>(MainMenuAddressablePath, owner, true);
//            var task3 = Commons.LoadPrefabsAsync<SettingsUIHub>(HubPrefabAddressablePath, owner, true);
//            var task4 = Commons.LoadPrefabsAsync<SettingsUIPauseMenu>(PauseMenuAddressalbePath, owner, true);
//            var task5 = Commons.LoadPrefabsAsync<SettingsUIDiscoveryDialog>(DiscoveryDialogAddressablePath, owner, true);
//            var task6 = Commons.LoadPrefabsAsync<SettingsUISpeaker>(SpeakerDialogAddressablePath, owner, true);
//            var task7 = Commons.LoadPrefabsAsync<SettingsUIFadeScreen>(FadeSceenAddressablePath, owner, true);
//            var task8 = Commons.LoadPrefabsAsync<SettingsRealTimeStats>(RealtimeStatsAddressablePath, owner, true);

//            // Load all in parallel
//            var (retCinema, retMainMenu, hubMenu, pauseMenu, discoveryDialog, speaker, fadeScreen, realTimeStats) = 
//                await UniTask.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8);

//            // Assign results
//            this.Cinema = retCinema;
//            this.SettingsUIMainMenu = retMainMenu;
//            this.Hub = hubMenu;
//            this.SettingsUIPauseMenu = pauseMenu;
//            this.UIDiscoveryDialog = discoveryDialog;
//            this.SettingsUISpeaker = speaker;
//            this.UIFadeScreen = fadeScreen;
//            this.RealTimeStats = realTimeStats;

//            // Activation for Cinema as per current implementation
//            if (this.Cinema != null)
//            {
//                this.Cinema.enabled = true;
//                this.Cinema.gameObject.SetActive(true);
//            }
//        }
        
//    }
//}