#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls; // Thư viện cần thiết cho Search
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using unvs.actions;








[CustomPropertyDrawer(typeof(ActionEntry))]
public class ActionEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var targetScriptProp = property.FindPropertyRelative("targetScript");
        var classNameProp = property.FindPropertyRelative("scriptClassName");
        var funcProp = property.FindPropertyRelative("functionName");

        // Layout chia cột
        float gap = 5;
        float leftWidth = position.width * 0.4f;
        Rect leftRect = new Rect(position.x, position.y, leftWidth, position.height);
        Rect rightRect = new Rect(position.x + leftWidth + gap, position.y, position.width - leftWidth - gap, position.height);

        // 1. Cột trái: Xử lý kéo thả MonoScript
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(leftRect, targetScriptProp, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            UpdateClassName(targetScriptProp, classNameProp);
        }

        // 2. Cột phải: Xử lý Dropdown chọn hàm
        if (targetScriptProp.objectReferenceValue != null)
        {
            // ĐẢM BẢO: Nếu className vẫn trống mà targetScript có giá trị (do copy/paste), hãy cập nhật lại ngay
            if (string.IsNullOrEmpty(classNameProp.stringValue))
            {
                UpdateClassName(targetScriptProp, classNameProp);
            }

            MonoScript ms = targetScriptProp.objectReferenceValue as MonoScript;
            System.Type type = ms.GetClass();

            if (type != null)
            {
                // Lọc hàm theo signature: 1 param (ActionSender), return UniTask
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => {
                        if (m.GetCustomAttribute<ActionFuncAttr>() == null) return false;
                        var pars = m.GetParameters();
                        return pars.Length == 1 && pars[0].ParameterType.Name == "ActionSender"
                               && m.ReturnType.Name.Contains("UniTask");
                    })
                    .Select(m => m.Name).ToList();

                string currentFunc = string.IsNullOrEmpty(funcProp.stringValue) ? "Select Function..." : funcProp.stringValue;

                if (GUI.Button(rightRect, currentFunc, EditorStyles.popup))
                {
                    // Sử dụng GenericMenu cho đơn giản hoặc AdvancedDropdown như cũ
                    GenericMenu menu = new GenericMenu();
                    foreach (string mName in methods)
                    {
                        menu.AddItem(new GUIContent(mName), mName == funcProp.stringValue, () => {
                            funcProp.stringValue = mName;
                            // QUAN TRỌNG: Lưu lại sau khi chọn hàm
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    menu.DropDown(rightRect);
                }
            }
        }
        else
        {
            EditorGUI.LabelField(rightRect, "Kéo script vào...");
            if (!string.IsNullOrEmpty(classNameProp.stringValue)) classNameProp.stringValue = "";
        }

        EditorGUI.EndProperty();
    }

    // Hàm phụ trợ để ép Unity lưu tên Class chính xác
    private void UpdateClassName(SerializedProperty targetScriptProp, SerializedProperty classNameProp)
    {
        if (targetScriptProp.objectReferenceValue != null)
        {
            MonoScript ms = targetScriptProp.objectReferenceValue as MonoScript;
            System.Type type = ms.GetClass();
            if (type != null)
            {
                // Sử dụng AssemblyQualifiedName để Type.GetType hoạt động chính xác khi Runtime
                classNameProp.stringValue = type.AssemblyQualifiedName;
            }
        }
        else
        {
            classNameProp.stringValue = "";
        }
        // Ép Unity ghi nhận sự thay đổi dữ liệu
        classNameProp.serializedObject.ApplyModifiedProperties();
    }
}



// Lớp hỗ trợ cửa sổ Search chuyên nghiệp
public class MethodSearchDropdown : AdvancedDropdown
{
    private List<string> methods;
    private System.Action<string> onSelected;

    public MethodSearchDropdown(AdvancedDropdownState state, List<string> methods, System.Action<string> onSelected) : base(state)
    {
        this.methods = methods;
        this.onSelected = onSelected;
        this.minimumSize = new Vector2(200, 300);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Functions");
        foreach (var m in methods)
        {
            root.AddChild(new AdvancedDropdownItem(m));
        }
        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        onSelected?.Invoke(item.name);
    }
}
#endif