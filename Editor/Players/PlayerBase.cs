//using UnityEngine;
//using UnityEditor;
//using unvs.players; // Phải có namespace này

//[CustomEditor(typeof(PlayerBase), true)] // 'true' để áp dụng cho cả các class con
//public class PlayerBaseBaseEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        // Ép kiểu về class CHA (Base)
//        PlayerBase script = (PlayerBase)target;

//        GUILayout.Space(10);

//        if (GUILayout.Button("Extract All Anim state", GUILayout.Height(30)))
//        {
//            script.EditorExtractAllAnim();
//            // THÊM DÒNG NÀY ĐỂ LƯU DỮ LIỆU
//            UnityEditor.EditorUtility.SetDirty(script);

           
//        }
//        if (GUILayout.Button("Physical calculate", GUILayout.Height(30)))
//        {
//            script.EditorPhysicalCalculate();
//            UnityEditor.EditorUtility.SetDirty(script);
//        }
//        if (GUILayout.Button("Generate dialogue UI", GUILayout.Height(30)))
//        {
//            script.EditorGenerateDialogueUI();
//            UnityEditor.EditorUtility.SetDirty(script);
//        }
//        if (GUILayout.Button("Generate cam watcher", GUILayout.Height(30)))
//        {
//            script.EditorGenerateCamWatcher();
//            UnityEditor.EditorUtility.SetDirty(script);
//        }
//        if (GUILayout.Button("Generate bagger", GUILayout.Height(30)))
//        {
//            script.EditorGenerateBagger();
//            UnityEditor.EditorUtility.SetDirty(script);
//        }
//    }
//}