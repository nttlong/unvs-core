//using UnityEngine;
//using UnityEditor;
//using unvs.players;
//using unvs.playerobjects; // Phải có namespace này

//[CustomEditor(typeof(PlayerInteractObject), true)] // 'true' để áp dụng cho cả các class con
//public class PlayerInteractObjectEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        // Ép kiểu về class CHA (Base)
//        PlayerInteractObject script = (PlayerInteractObject)target;

//        GUILayout.Space(10);

//        if (GUILayout.Button("Resize Collider2D", GUILayout.Height(30)))
//        {
//            script.EditorResizeCollider2D();
//            // THÊM DÒNG NÀY ĐỂ LƯU DỮ LIỆU
//            UnityEditor.EditorUtility.SetDirty(script);

           
//        }
//    }
//}