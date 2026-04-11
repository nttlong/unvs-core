#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace unvs.shares.editor
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

    }



}
#endif