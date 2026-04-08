/*
    this file define Virtuale scene editor
    create Button with caption "Create Scene"
    
   
*/

namespace editor.game2d {
    [CustomEditor(typeof(VScene))]
    public class VSceneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Scene"))
            {
                var vscene = (VScene)target;
                vscene.EditorCreateScene();
            }
        }
    }
}