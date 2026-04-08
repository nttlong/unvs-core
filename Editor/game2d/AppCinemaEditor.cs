using game2d.ext;
using UnityEditor;
using UnityEngine;
using unvs.game2d.ext;
using unvs.game2d.scenes;
namespace editor.game2d
{
    [CustomEditor(typeof(AppCinema),true)]
    public class AppCinemaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate"))
            {
                var cinema = (AppCinema)target;
               AppCinemaExt.GenerateCinema2D(cinema);
             
            }
        }
    }
}