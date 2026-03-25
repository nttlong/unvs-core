#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
internal class EditorDataConfig
{
    internal ReorderableList reorderableList;

    public Type[] actionTypes { get; internal set; }
}
public static class EditorExtension
{
    internal static EditorDataConfig OnEnableByTypes(this Editor editor, string dataPropertyName, params Type[] types)
    {
        var ret = new EditorDataConfig();

        // 1. Filter types based on the provided params array
        ret.actionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && types.Any(baseType => baseType.IsAssignableFrom(t)))
            .OrderBy(t => t.Name) // Optional: keeps your "+" menu organized
            .ToArray();

        // 2. Initialize ReorderableList
        SerializedProperty actionsProp = editor.serializedObject.FindProperty(dataPropertyName);
        ret.reorderableList = new ReorderableList(editor.serializedObject, actionsProp, true, true, true, true);

        // 3. Draw Header
        ret.reorderableList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Interaction Actions (Drag to Sort)");
        };

        // 4. Draw Element
        ret.reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            SerializedProperty element = actionsProp.GetArrayElementAtIndex(index);

            // Clean up the display name from the managed reference
            string fullType = element.managedReferenceFullTypename;
            string typeName = string.IsNullOrEmpty(fullType) ? "Null" : fullType.Split(' ').Last().Split('.').Last();

            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element,
                new GUIContent($"{index}: {typeName}"),
                true
            );
        };

        // 5. Dynamic Height Calculation
        ret.reorderableList.elementHeightCallback = (int index) => {
            return EditorGUI.GetPropertyHeight(actionsProp.GetArrayElementAtIndex(index), true) + 5;
        };

        // 6. Refined Add Dropdown
        ret.reorderableList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu menu = new GenericMenu();
            foreach (var type in ret.actionTypes)
            {
                // Use full path or category if you want to group them in the menu later
                menu.AddItem(new GUIContent(type.Name), false, () => {
                    // Ensure the editor logic for AddAction is called
                    // Note: You likely need to call serializedObject.ApplyModifiedProperties() inside AddAction
                    editor.AddAction(type, dataPropertyName);
                });
            }
            menu.ShowAsContext();
        };

        return ret;
    }
    internal static EditorDataConfig OnEnable2Args<T1,T2>(this Editor editor, string dataPropertyName,params Type[] types)
    {
        var ret= new EditorDataConfig();
        // Lấy tất cả các class trong Project
        ret.actionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => !t.IsAbstract && (
                typeof(T1).IsAssignableFrom(t) ||
                typeof(T2).IsAssignableFrom(t)
            ))
            .ToArray();
        // 2. Khởi tạo ReorderableList cho mảng "actions"
        SerializedProperty actionsProp = editor.serializedObject.FindProperty(dataPropertyName);
        ret.reorderableList = new ReorderableList(editor.serializedObject, actionsProp, true, true, true, true);

        // 3. Vẽ tiêu đề cho danh sách
        ret.reorderableList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Interaction Actions (Drag to Sort)");
        };

        // 4. Vẽ từng phần tử trong danh sách
        ret.reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            SerializedProperty element = actionsProp.GetArrayElementAtIndex(index);

            // Lấy tên class để hiển thị tiêu đề gọn hơn
            string fullType = element.managedReferenceFullTypename;
            string typeName = string.IsNullOrEmpty(fullType) ? "Null" : fullType.Split('.').Last();

            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element, new GUIContent($"{index}: {typeName}"), true);
        };

        // 5. Tính toán độ cao cho mỗi phần tử (vì Action có thể có nhiều dòng)
        ret.reorderableList.elementHeightCallback = (int index) => {
            return EditorGUI.GetPropertyHeight(actionsProp.GetArrayElementAtIndex(index), true) + 5;
        };

        // 6. Xử lý khi nhấn nút "+"
        ret.reorderableList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu menu = new GenericMenu();
            foreach (var type in ret.actionTypes)
            {
                menu.AddItem(new GUIContent(type.Name), false, () => editor.AddAction(type, dataPropertyName));
            }
            menu.ShowAsContext();
        };
        return ret;
    }

    internal static void OnInspectorGUI(this  Editor editor, EditorDataConfig localdata)
    {
        editor.serializedObject.Update();

        // Vẽ toàn bộ danh sách có khả năng kéo thả
        localdata.reorderableList.DoLayoutList();

        editor.serializedObject.ApplyModifiedProperties();
    }

    public static void AddAction(this Editor editor,Type type, string dataPropertyName)
    {
        editor.serializedObject.Update();
        SerializedProperty actionsProp = editor.serializedObject.FindProperty(dataPropertyName);

        int index = actionsProp.arraySize;
        actionsProp.InsertArrayElementAtIndex(index);
        actionsProp.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(type);

        editor.serializedObject.ApplyModifiedProperties();
    }
}
#endif