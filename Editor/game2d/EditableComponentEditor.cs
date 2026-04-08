using game2d.scenes;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using unvs.game2d.scenes;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Linq;
using game2d.ext;
namespace editor.game2d
{
   

    [CustomEditor(typeof(UnvsComponent), true)]
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

            foreach (var method in methods)
            {
                // Kiểm tra xem method có gắn attribute không
                var attr = method.GetCustomAttribute<InspectorButtonAttribute>();
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

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
            }
        }
    }
}
