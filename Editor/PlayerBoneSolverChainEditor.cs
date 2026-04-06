using UnityEngine;
using UnityEditor;
using unvs.players; // Phải có namespace này

[CustomEditor(typeof(PlayerBoneBase), true)] // 'true' để áp dụng cho cả các class con
public class PlayerBoneBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Ép kiểu về class CHA (Base)
        PlayerBoneBase script = (PlayerBoneBase)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Solve Chain", GUILayout.Height(30)))
        {
            script.GenerateSolve();
        }
        if (GUILayout.Button("Clear Solve Chain", GUILayout.Height(30)))
        {
            script.ClearSolve();
        }
    }
}