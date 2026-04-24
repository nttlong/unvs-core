using game2d.scenes;
using System;
using System.Reflection;

using UnityEngine;
using unvs.game2d.scenes;
using UnityEditor;

using game2d.ext;
using System.Linq;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
namespace editor.game2d
{
   

    [CustomEditor(typeof(UnvsBaseComponent), true)]
    public class UnvsComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetType = target.GetType();

            // Lấy tất cả các method (Public, NonPublic, Instance)
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                               BindingFlags.Public | BindingFlags.NonPublic);
            
            // Thiết lập Style cho nút (Căn giữa, Cao, Gọn)
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 30,
                fixedWidth = 180,
                fontStyle = FontStyle.Bold
            };
            var errMsg = string.Empty;
            ((UnvsBaseComponent)target).OnEditorError = err =>
            {
                errMsg = err;
               
            };
            foreach (var method in methods)
            {
                // Kiểm tra xem method có gắn attribute không
                var attr = method.GetCustomAttribute<UnvsButtonAttribute>();
                if (attr != null)
                {
                    // Lấy tên hiển thị: Nếu attr.ButtonName trống thì dùng tên Method
                    string displayName = string.IsNullOrEmpty(attr.ButtonName) ? method.Name : attr.ButtonName;

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    
                    if (GUILayout.Button(displayName, buttonStyle))
                    {
                        // Thực thi method
                        method.Invoke(target, null);
                        target.EditorSetDirty();
                    }
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox(errMsg, MessageType.Warning);
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
            }
            // Lấy danh sách các Property có gắn UnvsPropertyAttribute
            var fieldList = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<UnvsPropertyAttribute>() != null);

            foreach (var property in fieldList)
            {
                var attr = property.GetCustomAttribute<UnvsPropertyAttribute>();

                EditorGUILayout.BeginHorizontal();

                // 1. Hiển thị Label và Value của Property
                // Lấy giá trị hiện tại của Property
                object value = property.GetValue(target);

                // Tùy vào PType mà bạn có thể vẽ các kiểu ô nhập khác nhau
                // Ở đây tôi ví dụ vẽ một TextField chung
                EditorGUILayout.PrefixLabel(property.Name);
                string stringValue = value?.ToString() ?? "";
                string newValue = EditorGUILayout.TextField(stringValue);

                // Nếu giá trị thay đổi thì cập nhật lại
                if (newValue != stringValue)
                {
                    property.SetValue(target, newValue);
                    target.EditorSetDirty();
                }

                // 2. Vẽ nút bấm bên cạnh (Ví dụ nút Fix AI)
                //if (GUILayout.Button("Fix", GUILayout.Width(50)))
                //{
                //    // Thực hiện logic xử lý dựa trên PType
                //    HandlePropertyAction(property, attr.PType);
                //}

                EditorGUILayout.EndHorizontal();
            }

        }
    }
}
