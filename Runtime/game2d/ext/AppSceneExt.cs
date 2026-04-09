//using Cysharp.Threading.Tasks.Triggers;
//using System.IO;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UI;
//using unvs.ext;
//using unvs.game2d.scenes;
//using static game2d.ext.AppSceneExt;

using System.IO;
using UnityEditor;
using UnityEngine;

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
        public static EditorCreateResult<T> EditorCreatePrefab<T>(this Object obj, string name, string folderPath = null) where T : Component
        {
            // 1. Determine the path of the AppScene script/asset to place the prefab in the same folder
            if (string.IsNullOrEmpty(folderPath))
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