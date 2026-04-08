using game2d.ext;
using UnityEditor;
using UnityEngine;
using unvs.game2d.ext;
using unvs.game2d.scenes;
namespace editor.game2d
{
    [CustomEditor(typeof(AppMainMenu),true)]
    public class AppMainMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create layout"))
            {
                var menu = (AppMainMenu)target;
               
                menu.EditorCreateMenuLayout();
                menu.EditorSetDirty();
            }
        }
    }
}