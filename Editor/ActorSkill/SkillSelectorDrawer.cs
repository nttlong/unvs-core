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
                EditorGUI.LabelField(position, label, new GUIContent("Chỉ dùng với [SerializeReference]"));
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // 1. Vẽ Label
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // 2. Vẽ nút Dropdown (Nằm bên phải label)
            Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            string fullTypeName = property.managedReferenceFullTypename;
            string currentTypeName = string.IsNullOrEmpty(fullTypeName) ? "Null (Click to Select)" : fullTypeName.Split(' ').Last().Split('.').Last();

            if (GUI.Button(buttonRect, currentTypeName, EditorStyles.popup))
            {
                ShowTypeMenu(property);
            }

            // 3. Vẽ các thuộc tính con nếu có (Dùng PropertyField cho phần nội dung bên dưới)
            // Lưu ý: Không vẽ label lần nữa để tránh chồng lấn
            Rect propertyRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height - EditorGUIUtility.singleLineHeight - 2);
            
            // Nếu có value mới hiện các field con
            if (property.managedReferenceValue != null)
            {
                EditorGUI.PropertyField(position, property, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }

        private void ShowTypeMenu(SerializedProperty property)
        {
            var baseType = typeof(AbstractActorBaseSkill);
            var types = unvs.editor.utils.TypeCacheHelper.GetDerivedTypes(baseType);

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