using game2d.ext;
using game2d.scenes;
using UnityEditor;
using UnityEngine;
using unvs.game2d.ext;
using unvs.game2d.scenes;
namespace editor.game2d {
    [CustomEditor(typeof(AppScene))]
    public class AppSceneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Cinema"))
            {
                var appscene = (AppScene)target;


                appscene.EditorCreateCinema();
            }
            if (GUILayout.Button("Create Scene"))
            {
                var appscene = (AppScene)target;
                

                appscene.EditorCreateScene();
            }
            if (GUILayout.Button("Create Main Menu "))
            {
                var appscene = (AppScene)target;
                appscene.EditorCreateMainMenu();
            }
            if (GUILayout.Button("Create Pause Menu "))
            {
                var appscene = (AppScene)target;
                appscene.EditorCreatePauseMenu();
            }
            if (GUILayout.Button("Create Fade screen "))
            {
                var appscene = (AppScene)target;
                var ret = appscene.EditorCreatePrefab<AppFadeScreen>("FadeScreen");
                appscene.fadeScreenPath = ret.PrefabPath;
                appscene.fadeScreen = ret.value;
                appscene.EditorSave();
                
            }
            if (GUILayout.Button("Create Dialog "))
            {
                var appscene = (AppScene)target;
                var ret = appscene.EditorCreatePrefab<AppDialog>("Dialog");
                appscene.dialogPath = ret.PrefabPath;
                appscene.dialog = ret.value;
                appscene.EditorSave();

            }
        }
    }
}
