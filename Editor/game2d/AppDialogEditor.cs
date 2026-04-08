using game2d.ext;
using game2d.scenes;
using UnityEditor;
using UnityEngine;
using unvs.game2d.ext;
using unvs.game2d.scenes;
namespace editor.game2d
{
    [CustomEditor(typeof(AppDialog),true)]
    public class AppDialogEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate"))
            {
                var cinema = (AppDialog)target;
                AppDialogExt.Generate(cinema);
                
            }
        }
    }
}