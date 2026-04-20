using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using unvs.unvsobjects;
using unvs.editor.utils;

namespace unvs.editor.objects
{
    [CustomPropertyDrawer(typeof(TraitSelectorAttribute))]
    public class TraitSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.LabelField(position, label, new GUIContent("Use only with [SerializeReference]"));
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // 1. Draw Label
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // 2. Draw Dropdown Button
            Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            string fullTypeName = property.managedReferenceFullTypename;
            string currentTypeName = string.IsNullOrEmpty(fullTypeName) ? "Null (Click to Select)" : fullTypeName.Split(' ').Last().Split('.').Last();

            if (GUI.Button(buttonRect, currentTypeName, EditorStyles.popup))
            {
                ShowTypeMenu(property);
            }

            // 3. Draw child properties
            if (property.managedReferenceValue != null)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }

        private void ShowTypeMenu(SerializedProperty property)
        {
            var baseType = typeof(InteractionTrait);
            var types = TypeCacheHelper.GetDerivedTypes(baseType);

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            foreach (var type in types)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
