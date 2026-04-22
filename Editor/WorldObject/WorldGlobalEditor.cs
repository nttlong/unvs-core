//using System;
//using System.Linq;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine;
//
//
//using unvs.players;
//using unvs.shares;
//[CustomEditor(typeof(WorldObject), true)]
//public class WorldGlobalEditor :  Editor
//{
//    private string inputSceneName = "Enter scene name";

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        // Ép kiểu về class CHA (Base)
//        WorldObject script = (WorldObject)target;

//        GUILayout.Space(10);
//        // 1. Cập nhật giá trị chuỗi mỗi khi người dùng gõ phím
       
//        if (GUILayout.Button("Play this scene", GUILayout.Height(30)))
//        {
//            //var sceneName = Constants.Scenes.TEST_SCENE;
//            string[] guids = AssetDatabase.FindAssets($"{script.TestSceneName} t:Scene");
//            if (guids.Length > 0)
//            {
//                UnityEditor.EditorPrefs.SetString("PendingWorldPath", WorldObjectEditor.GetAddressableOf(script));
               
//                string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
//                script.TestScensGuid = guids[0];
//                var scene=EditorSceneManager.OpenScene(scenePath);
//                var singeSinge = scene.GetRootGameObjects().FirstOrDefault(p => p.GetComponent<ISingleScene>()!=null );
//                if (singeSinge != null)
//                {
                   
                  
//                    EditorApplication.isPlaying = true;

//                } else
//                {
//                    throw new Exception($"Please add {typeof(ISingleScene)} to {scenePath} ");
//                }


//            } else
//            {
//                throw new Exception($"{script.TestSceneName} was not found");
//            }


//        }
        
//    }
//}