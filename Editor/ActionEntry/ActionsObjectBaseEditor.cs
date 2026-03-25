
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using unvs.actions;



[CustomEditor(typeof(ActionsObjectBase), true)]
public class ActionsObjectBaseEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        
        // Khởi tạo ReorderableList để có thể kéo thả thủ công
        reorderableList = new ReorderableList(serializedObject,
            serializedObject.FindProperty("actions"),
            true, true, true, true);

        // Vẽ tiêu đề và nút Sort
        reorderableList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 70, rect.height), "Actions List (Marked with [ActionFuncAttr])");

            if (GUI.Button(new Rect(rect.x + rect.width - 65, rect.y, 65, rect.height), "Sort A-Z"))
            {
                SortActions();
            }
        };

        // Vẽ từng dòng trong danh sách
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
        };
    }

    private void SortActions()
    {
        var listProp = serializedObject.FindProperty("actions");
        int count = listProp.arraySize;

        // 1. Lưu dữ liệu vào danh sách tạm để sắp xếp
        var tempList = new List<(Object target, string funcName)>();
        for (int i = 0; i < count; i++)
        {
            var el = listProp.GetArrayElementAtIndex(i);
            tempList.Add((
                el.FindPropertyRelative("targetComponent").objectReferenceValue,
                el.FindPropertyRelative("functionName").stringValue
            ));
        }

        // 2. Sắp xếp theo tên Component trước, sau đó tới tên Hàm
        var sorted = tempList
            .OrderBy(x => x.target != null ? x.target.name : "zzz")
            .ThenBy(x => x.funcName)
            .ToList();

        // 3. Gán ngược lại vào SerializedProperty
        for (int i = 0; i < count; i++)
        {
            var el = listProp.GetArrayElementAtIndex(i);
            el.FindPropertyRelative("targetComponent").objectReferenceValue = sorted[i].target;
            el.FindPropertyRelative("functionName").stringValue = sorted[i].funcName;
        }

        serializedObject.ApplyModifiedProperties();
        Debug.Log("Đã sắp xếp danh sách Actions theo thứ tự A-Z.");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Hiển thị các field khác nếu có
        DrawPropertiesExcluding(serializedObject, "actions", "m_Script");

        // Hiển thị danh sách Actions có khả năng Sort
        reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif