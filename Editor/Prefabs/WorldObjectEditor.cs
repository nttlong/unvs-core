#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using unvs.game2d.scenes;


using unvs.shares;

public class WorldObjectEditor
{
    // Thêm lựa chọn vào menu chuột phải của Hierarchy
    // Priority 10 giúp nó nằm ở nhóm trên cùng
    [MenuItem("Assets/Play This World", false, 100)]
    private static void PlayWorldFromAsset(MenuCommand menuCommand)
    {

        GameObject selectedPrefab = Selection.activeObject as GameObject;

        // Kiểm tra xem có phải là Prefab và có gắn IScene không
        if (selectedPrefab != null)
        {
            UnvsScene sceneObj = selectedPrefab.GetComponent<UnvsScene>();
            if (sceneObj != null)
            {
                // 2. Lấy Addressable Path hoặc Path thông thường
                // Ở đây mình ví dụ lấy path đơn giản để nạp
                string assetPath = AssetDatabase.GetAssetPath(selectedPrefab);

                // Lưu vào EditorPrefs để SingleSceneController đọc
                EditorPrefs.SetString("PendingWorldPath", assetPath);

                // 3. Mở scene Single và Play
                OpenSingleSceneAndPlay();
            }
            else
            {
                EditorUtility.DisplayDialog("Lỗi", "Prefab này không có Component IScene!", "OK");
            }
        }
    }

    [MenuItem("Assets/Play This World", true)]
    private static bool ValidatePlayWorldFromProject()
    {
        return Selection.activeObject is GameObject go && go.GetComponent<UnvsScene>() != null;
    }

    private static void OpenSingleSceneAndPlay()
    {
        Debug.LogError("Not implemement");
        //if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        //{
        //   var s= (Selection.activeObject as GameObject).GetComponent<UnvsScene>();
        //    var sceneName = s.TestSceneName;
        //    if (string.IsNullOrEmpty(sceneName))
        //    {
        //        throw new Exception($"Please, set field TestSceneName for {s.name}");
        //    }
        //    string[] guids = AssetDatabase.FindAssets($"{sceneName} t:Scene");
        //    if (guids.Length > 0)
        //    {
        //        string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        //        s.TestScensGuid = guids[0];
                
        //        var scene = EditorSceneManager.OpenScene(scenePath);
        //        var singeSinge = scene.GetRootGameObjects().FirstOrDefault(p => p.GetComponent<UnvsScene>() != null);
        //        if (singeSinge != null)
        //        {
        //           GlobalApplication.PendingWorldPath = GetAddressableOf(Selection.activeObject);
                   
        //            EditorApplication.isPlaying = true;

        //        }
        //        else
        //        {
        //            throw new Exception($"Please add {typeof(UnvsScene)} to {scenePath} ");
        //        }
        //        EditorApplication.isPlaying = true;
        //    }
        //}
    }

    public static string GetAddressableOf(UnityEngine.Object activeObject)
    {
        if (activeObject == null) return string.Empty;

        // 1. Lấy Asset Path từ Object (ví dụ: Assets/Models/Hero.fbx)
        string assetPath = AssetDatabase.GetAssetPath(activeObject);

        // 2. Chuyển đổi Path sang GUID (Addressables quản lý nội bộ qua GUID)
        string guid = AssetDatabase.AssetPathToGUID(assetPath);

        // 3. Truy cập Settings mặc định của Addressables trong Project
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings != null)
        {
            // 4. Tìm Entry (bản ghi) của Asset này trong các Group
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);

            // 5. Nếu tìm thấy, trả về địa chỉ (Address), nếu không trả về chuỗi rỗng
            if (entry != null)
            {
                return entry.address;
            }
        }

        return string.Empty; // Trả về rỗng nếu object không thuộc Addressables
    }
}



public static class AddressableHelper
{
    public static string GetAddressablePath(GameObject prefab)
    {
        if (prefab == null) return string.Empty;

        // Tìm GUID của Asset dựa trên đối tượng GameObject
        string path = AssetDatabase.GetAssetPath(prefab);
        string guid = AssetDatabase.AssetPathToGUID(path);

        // Truy cập cài đặt Addressables hiện tại của dự án
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
        {
            // Tìm kiếm Entry (thông tin đăng ký) của Asset này trong Addressables
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);

            if (entry != null)
            {
                return entry.address; // Đây chính là Addressable Path bạn cần
            }
        }

        Debug.LogWarning("Object này chưa được đánh dấu là Addressable!");
        return string.Empty;
    }
}
// --- PHẦN TỰ ĐỘNG MỞ LẠI PREFAB SAU KHI STOP ---

#endif