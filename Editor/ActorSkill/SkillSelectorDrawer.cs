namespace unvs.editor.actorskill
{
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Linq;
    using unvs.actor.skills; // Namespace chứa ActorBaseSkill của bạn

    [CustomPropertyDrawer(typeof(SkillSelectorAttribute))]
    public class SkillSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.LabelField(position, label.text, "Chỉ dùng với [SerializeReference]");
                return;
            }

            // 1. Lấy tất cả class con của ActorBaseSkill
            var baseType = typeof(ActorBaseSkill);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract && p.IsClass)
                .ToArray();

            // 2. Vẽ label cho phần tử mảng
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // 3. Vẽ nút Dropdown (Nằm bên phải label)
            Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            string currentTypeName = property.managedReferenceFullTypename.Split(' ').Last();
            if (string.IsNullOrEmpty(currentTypeName)) currentTypeName = "Null (Click to Select)";

            if (GUI.Button(buttonRect, currentTypeName, EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () =>
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                });

                foreach (var type in types)
                {
                    menu.AddItem(new GUIContent(type.Name), currentTypeName == type.Name, () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            // 4. Vẽ các thuộc tính bên trong (name, animator) bên dưới dropdown
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}