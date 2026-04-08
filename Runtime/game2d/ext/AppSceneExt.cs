using Cysharp.Threading.Tasks.Triggers;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.scenes;
using static game2d.ext.AppSceneExt;

namespace game2d.ext
{
    public static class AppSceneExt
    {
#if UNITY_EDITOR
        public class EditorCreateResult<T>
        {
            public T value;
            public string PrefabPath;
        }
        public static EditorCreateResult<T> EditorCreatePrefab<T>(this Object obj,string name, string folderPath = null) where T : Component
        {
            // 1. Determine the path of the AppScene script/asset to place the prefab in the same folder
            if(string.IsNullOrEmpty(folderPath))
             folderPath = EditorGetAssetFolder(obj);
            string prefabPath = Path.Combine(folderPath, $"{name}.prefab");

            // 2. Create a new GameObject with the required component
            GameObject go = new GameObject("Cinema", typeof(RectTransform), typeof(T));

            // 3. Save as a prefab and cleanup the temporary object
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);

            // 4. Assign the reference to the AppScene instance
            var ret = prefabAsset.GetComponent<T>();
           
            // 5. Mark the object as dirty so the reference is saved in the scene/asset
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();

            return new EditorCreateResult<T>
            {
                value = ret,
                PrefabPath = prefabPath.Replace('\\', '/')
            };
        }
        public static AppCinema EditorCreateCinema(this AppScene appscene)
        {
           var ret= appscene.EditorCreatePrefab<AppCinema>("Cinema");
            appscene.cinema = ret.value;
            appscene.cinemaPath = ret.PrefabPath;
            return ret.value;

        }
        public static void EditorCreateScene(this AppScene appscene)
        {

        }
        /// <summary>
        /// Create Main Menu, this is a prefab in the same folder of AppScene
        /// After create Main menu prefab set value of AppScene.Instance.MainMenu is MainMenu prefab
        /// </summary>
        public static AppMainMenu EditorCreateMainMenu(this AppScene appscene)
        {
            var ret= appscene.EditorCreatePrefab<AppMainMenu>("MainMenu");
            appscene.mainMenuPath = ret.PrefabPath;
            appscene.MainMenu=ret.value;
            return ret.value;
            
        }

        /// <summary>
        /// Create Pause Menu, this is a prefab in the same folder of AppScene
        /// After create Pause menu prefab set value of AppScene.Instance.PauseMenu is PauseMenu prefab
        /// </summary>
        public static AppPauseMenu EditorCreatePauseMenu(this AppScene appscene)
        {
            var ret = appscene.EditorCreatePrefab<AppPauseMenu>("PauseMenu");
            appscene.pauseMenuPath = ret.PrefabPath;
            appscene.PauseMenu=ret.value;
            appscene.EditorSave();
            return ret.value ;
           
        }

        public static string EditorGetAssetFolder(this Object asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
            {
                return "Assets"; // Fallback
            }
            return Path.GetDirectoryName(path);
        }
#endif
    }
}