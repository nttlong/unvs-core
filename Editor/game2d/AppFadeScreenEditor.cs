using game2d.ext;
using game2d.scenes;
using UnityEditor;
using UnityEngine;
using unvs.game2d.ext;
using unvs.game2d.scenes;
namespace editor.game2d
{
    [CustomEditor(typeof(AppFadeScreen),true)]
    public class AppFadeScreenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate"))
            {
                var cinema = (AppFadeScreen)target;
                AppFadeScreenExt.EditorGenerate(cinema);
                cinema.EditorSave();
            }
        }
    }
}