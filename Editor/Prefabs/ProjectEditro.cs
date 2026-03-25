#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class HierarchyWorldEditor
{
    private const string LastPrefabKey = "LastOpenedPrefabPath";

    [MenuItem("GameObject/Play This World", false, 10)]
    private static void PlayWorldFromHierarchy(MenuCommand menuCommand)
    {
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        string assetPath = "";

        if (stage != null)
        {
            // 1. Tự động Save Prefab trước khi chạy để không mất công sức vừa sửa
            // Đây là bước "Auto-Save" bạn cần
            assetPath = stage.assetPath;

            // Ép Unity lưu dữ liệu hiện tại vào file .prefab
            bool saveSuccess = PrefabUtility.SaveAsPrefabAsset(stage.prefabContentsRoot, assetPath);

            if (saveSuccess)
            {
                Debug.Log($"<color=green>Auto-Save:</color> Đã lưu thay đổi cho {stage.prefabContentsRoot.name}");
            }

            // 2. Ghi nhớ đường dẫn để sau khi Stop Play sẽ mở lại đúng chỗ này
            EditorPrefs.SetString(LastPrefabKey, assetPath);
        }
        else
        {
            // Nếu ở Hierarchy thông thường
            GameObject target = menuCommand.context as GameObject;
            if (target == null) return;

            GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(target);
            if (sourcePrefab != null) assetPath = AssetDatabase.GetAssetPath(sourcePrefab);
        }

        if (!string.IsNullOrEmpty(assetPath))
        {
            EditorPrefs.SetString("PendingWorldPath", assetPath);

            // Chuyển Scene và Play
            OpenSingleSceneAndPlay();
        }
    }

    // --- PHẦN TỰ ĐỘNG MỞ LẠI PREFAB SAU KHI STOP ---
    [InitializeOnLoadMethod]
    private static void MonitorPlayModeState()
    {
        EditorApplication.playModeStateChanged += (state) =>
        {
            // Khi bạn nhấn nút Stop (thoát khỏi Play Mode và quay về Editor)
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (EditorPrefs.HasKey(LastPrefabKey))
                {
                    string path = EditorPrefs.GetString(LastPrefabKey);
                    EditorPrefs.DeleteKey(LastPrefabKey); // Xóa để không bị mở lặp lại vô tận

                    // Mở lại Prefab Mode cho file đó
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                        Debug.Log("<color=cyan>Magic:</color> Đã mở lại Prefab để bạn tiếp tục Edit!");
                    }
                }
            }
        };
    }

    private static void OpenSingleSceneAndPlay()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            string[] guids = AssetDatabase.FindAssets("Single t:Scene");
            if (guids.Length > 0)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
            }
        }
    }
}
#endif