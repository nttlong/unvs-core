#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using unvs.actions;

[CustomPropertyDrawer(typeof(ActionBase), true)]
public class ActionBaseDrawer : PropertyDrawer
{
    private Type[] _derivedTypes;
    private string[] _typeNames;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        // Chỉ xử lý nếu dùng [SerializeReference]
        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // 1. Vẽ Label và nút bấm để chọn Type
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        string fullType = property.managedReferenceFullTypename;
        string typeName = string.IsNullOrEmpty(fullType) ? "None (Null)" : fullType.Split(' ').Last().Split('.').Last();

        // Nút bấm thay đổi type
        if (GUI.Button(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), typeName, EditorStyles.popup))
        {
            ShowTypeMenu(property);
        }

        // 2. Vẽ nội dung bên trong của Action (nếu có)
        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.EndProperty();
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        if (_derivedTypes == null)
        {
            _derivedTypes = unvs.editor.utils.TypeCacheHelper.GetDerivedTypes(typeof(ActionBase));
        }

        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () => {
            property.managedReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
        });

        foreach (var type in _derivedTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            });
        }
        menu.ShowAsContext();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Trả về độ cao thực tế của tất cả các field bên trong Action
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif