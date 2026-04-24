using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;

using unvs.types;

[CustomPropertyDrawer(typeof(UnvsEditableProperty),true)]
public class AnimStateInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fixedHeight = 30,
            fixedWidth = 180,
            fontStyle = FontStyle.Bold
        };
        EditorGUI.BeginProperty(position, label, property);

        // 1. Vẽ tiêu đề Foldout
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float currentY = position.y + EditorGUIUtility.singleLineHeight + 2;

            // 2. Tự động vẽ tất cả các field của class AnimStateInfo
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            // Duyệt qua các con của property hiện tại
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                enterChildren = false;
                float height = EditorGUI.GetPropertyHeight(iterator, true);
                Rect fieldRect = new Rect(position.x, currentY, position.width, height);

                EditorGUI.PropertyField(fieldRect, iterator, true);
                currentY += height + 2;
            }

            // 3. Vẽ các nút bấm thông qua Reflection
            object targetObj = GetTargetObjectOfProperty(property);
            if (targetObj != null)
            {
                var methods = targetObj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttribute<UnvsButtonAttribute>() != null);

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<UnvsButtonAttribute>();
                    string displayName = string.IsNullOrEmpty(attr.ButtonName) ? method.Name : attr.ButtonName;
                    ////float btnWidth = 180;
                    ////float btnHeight = 30;
                    //float centerX = position.x + (position.width - btnWidth) / 2f;
                    Rect btnRect = new Rect(position.x + 20, currentY, position.width - 20, 25);
                    if (GUI.Button(btnRect, displayName, buttonStyle))
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, "Execute " + displayName);
                        method.Invoke(targetObj, null);
                        property.serializedObject.Update();
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    }
                    currentY += 27;
                }
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    // Quan trọng: Tính toán tổng chiều cao để các item không đè lên nhau
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        
        float height = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            // Cộng chiều cao của các field
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                enterChildren = false;
                height += EditorGUI.GetPropertyHeight(iterator, true) + 2;
            }

            // Cộng chiều cao của các nút bấm
            object targetObj = GetTargetObjectOfProperty(property);
            if (targetObj != null)
            {
                var methodCount = targetObj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Count(m => m.GetCustomAttribute<UnvsButtonAttribute>() != null);
                height += methodCount * 27 + 5;
            }
        }
        return height;
    }

    // Hàm lấy Object thực tế (giữ nguyên logic bạn đã có hoặc dùng bản sửa lỗi này)
    private object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        string[] elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                string name = element.Substring(0, element.IndexOf("["));
                int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));

                var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var list = field.GetValue(obj) as System.Collections.IList;
                obj = list[index];
            }
            else
            {
                var field = obj.GetType().GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null) return null;
                obj = field.GetValue(obj);
            }
        }
        return obj;
    }
}