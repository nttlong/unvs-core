#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace unvs.editor.utils
{

    public static class UnvsEditorUtils
    {
        public static AnimatorController EditorCreateAnimatorController(string folderPath, string fileName)
        {
            // 1. Đảm bảo đường dẫn file có đuôi .controller
            if (!fileName.EndsWith(".controller"))
            {
                fileName += ".controller";
            }

            // 2. Kết hợp đường dẫn
            string fullPath = Path.Combine(folderPath, fileName).Replace('\\', '/');

            // 3. Tạo Animator Controller Asset
            // Hàm này sẽ tạo file vật lý trên ổ cứng và trả về object
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(fullPath);

            if (controller != null)
            {
                Debug.Log($"<color=green>Success:</color> Created Animator Controller at: {fullPath}");

                // 4. (Tùy chọn) Thêm các Layer hoặc Parameter mặc định nếu bạn muốn
                // controller.AddParameter("IsWalking", AnimatorControllerParameterType.Bool);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError($"Failed to create Animator Controller at: {fullPath}");
            }

            return controller;
        }

        public static string EditorGetFolder(GameObject go)
        {
            if (go == null) return string.Empty;

            // 1. Tìm đường dẫn Asset của GameObject (ví dụ: Assets/Prefabs/Player.prefab)
            string assetPath = AssetDatabase.GetAssetPath(go);

            // Nếu GameObject này không phải là Asset (chỉ là object tạm trong Scene chưa lưu)
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning($"GameObject {go.name} is not a persistent asset.");
                return "Assets"; // Trả về thư mục gốc mặc định
            }

            // 2. Lấy đường dẫn thư mục chứa file đó
            string folderPath = Path.GetDirectoryName(assetPath);

            // 3. Chuẩn hóa dấu gạch chéo theo chuẩn Unity (/) thay vì chuẩn Windows (\)
            return folderPath.Replace('\\', '/');
        }
        public static string GetAddress(GameObject go)
        {
            // 1. Tìm GUID của Asset từ GameObject
            string path = AssetDatabase.GetAssetPath(go);
            string guid = AssetDatabase.AssetPathToGUID(path);

            // 2. Truy cập vào Settings của Addressables
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings != null)
            {
                // 3. Tìm Entry tương ứng với GUID
                AddressableAssetEntry entry = settings.FindAssetEntry(guid);

                if (entry != null)
                {
                    return entry.address; // Đây là giá trị bạn cần
                }
            }

            return "Not an Addressable";
        }
        public static string EditorGetAddressPath(this AssetReference myRef)
        {

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(myRef.AssetGUID);
            if (entry != null)
            {
                return entry.address;
            }
            return string.Empty;
        }

        public static void CollecteAllTo<T>(Transform tr)
        {
            var items = tr.parent.GetComponentsInChildren<T>();
            foreach ( var item in items )
            {
                if (typeof(T) == typeof(Transform))
                {
                   if(((Transform)(object)item).parent == tr.parent)
                    {
                        ((Transform)(object)item).SetParent(tr,true);
                    }
                }
                if (typeof(T) == typeof(MonoBehaviour))
                {
                    if (((MonoBehaviour)(object)item).transform.parent == tr.parent)
                    {
                        ((MonoBehaviour)(object)item).transform.SetParent(tr, true);
                    }
                }
            }
        }
        public static void EditorOpenClipV2(GameObject target, string clipPath)
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

            if (clip == null)
            {
                Debug.LogError($"Không tìm thấy AnimationClip tại đường dẫn: {clipPath}");
                return;
            }

            // 1. QUAN TRỌNG: Phải chọn Object này trong Hierarchy trước
            Selection.activeGameObject = target;

            // 2. Mở/Lấy cửa sổ Animation
            EditorApplication.ExecuteMenuItem("Window/Animation/Animation");

            Assembly editorAssembly = typeof(EditorWindow).Assembly;
            Type animWindowType = editorAssembly.GetType("UnityEditor.AnimationWindow");
            EditorWindow window = EditorWindow.GetWindow(animWindowType);

            if (window != null)
            {
                // 3. Lấy AnimationWindowState
                var stateProperty = animWindowType.GetProperty("state", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                object state = stateProperty.GetValue(window);

                if (state != null)
                {
                    // 4. Đảm bảo State đã nhận diện Target (Keyable Object)
                    // Đôi khi cần gọi Refresh hoặc gán trực tiếp Selection
                    var selectionProperty = state.GetType().GetProperty("activeKeyableObject", BindingFlags.Instance | BindingFlags.Public);
                    if (selectionProperty != null)
                    {
                        // Lấy Component Animator hoặc Animation từ target
                        Component animationPlayer = target.GetComponent<Animator>();
                        if (animationPlayer == null) animationPlayer = target.GetComponent<Animation>();

                        selectionProperty.SetValue(state, animationPlayer);
                    }

                    // 5. Gán Clip
                    var clipProperty = state.GetType().GetProperty("activeAnimationClip", BindingFlags.Instance | BindingFlags.Public);
                    clipProperty.SetValue(state, clip);

                    // 6. Force Window cập nhật lại giao diện
                    window.Repaint();
                }
                else
                {
                    Debug.LogError($"Không thể lấy state của Animation Window. Hãy thử mở cửa sổ này thủ công một lần.");
                }
            }
        }
        public static void EditorOpenClip(GameObject target, string clipPath)
        {
            // 1. Load AnimationClip từ đường dẫn (Path phải bắt đầu bằng "Assets/...")
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

            if (clip == null)
            {
                Debug.LogError($"Không tìm thấy AnimationClip tại đường dẫn: {clipPath}");
                return;
            }

            // 2. Chọn Object chứa Animator trước
            //Selection.activeGameObject = target;

            // 3. Mở cửa sổ Animation
            EditorApplication.ExecuteMenuItem("Window/Animation/Animation");

            // 4. Dùng Reflection để gán Clip vào cửa sổ đang mở
            Assembly editorAssembly = typeof(EditorWindow).Assembly;
            Type animWindowType = editorAssembly.GetType("UnityEditor.AnimationWindow");

            EditorWindow window = EditorWindow.GetWindow(animWindowType);

            if (window != null)
            {
                // Lấy thuộc tính 'state'
                var stateProperty = animWindowType.GetProperty("state", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                object state = stateProperty.GetValue(window);

                if (state != null)
                {
                    // Gán clip vào 'activeAnimationClip'
                    var clipProperty = state.GetType().GetProperty("activeAnimationClip", BindingFlags.Instance | BindingFlags.Public);
                    clipProperty.SetValue(state, clip);

                    window.Repaint();
                }else
                {
                    Debug.LogError($"Can not get state of {clipPath}");
                }
            }
        }

        public static AssetReference CreateAssetReference(string prefabPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(prefabPath);
            return new AssetReference(guid);
        }
    }



}
#endif