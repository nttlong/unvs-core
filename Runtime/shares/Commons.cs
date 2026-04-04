using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets; // Cần cái này
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using unvs.ui;
namespace unvs.shares
{
    public enum DockDirection
    {
        Left,
        Right,
        Top,
        Bottom,
        Full
    }
    /// <summary>
    /// Enum đại diện cho loại platform/environment đang chạy (runtime).
    /// Dùng Application.platform để detect.
    /// </summary>
    /// <summary>
    /// Enum đơn giản để biết game đang chạy trên Mobile hay Console (hoặc PC/Editor).
    /// Detect đúng trong Editor nếu bạn switch platform trong Build Settings.
    /// </summary>
    public enum GameRuntimePlatform
    {
        Mobile,     // Android hoặc iOS
        Console,    // Xbox, PS, Switch...
        PC,         // Windows/Mac/Linux
        EditorOnly, // Chỉ trong Editor, chưa switch platform
        Unknown
    }

    public static class Commons
    {
        
        public static Vector2 GetScreenSize()
        {
            return new Vector2(Screen.width, Screen.height);
        }
        public static GameRuntimePlatform GetCurrent()
        {
            // Ưu tiên preprocessor để detect simulate trong Editor
#if UNITY_ANDROID || UNITY_IOS
        return GameRuntimePlatform.Mobile;
#elif UNITY_XBOXONE || UNITY_PS4 || UNITY_PS5 || UNITY_SWITCH || UNITY_GAMECORE
        return GameRuntimePlatform.Console;
#elif UNITY_EDITOR
            return GameRuntimePlatform.EditorOnly;  // Editor chưa switch
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
        return GameRuntimePlatform.PC;
#else
        // Runtime fallback nếu không match preprocessor
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return GameRuntimePlatform.Mobile;

        if (Application.platform == RuntimePlatform.XboxOne ||
            Application.platform == RuntimePlatform.PS4 ||
            Application.platform == RuntimePlatform.PS5 ||
            Application.platform == RuntimePlatform.Switch)
            return GameRuntimePlatform.Console;

        return GameRuntimePlatform.Unknown;
#endif
        }

        // Tiện ích nhanh
        public static bool IsMobile()
        {
#if UNITY_ANDROID || UNITY_IOS
        return true;
#elif UNITY_EDITOR
            // Trong Editor: kiểm tra target build hiện tại
            // Nếu đang switch sang Android hoặc iOS → coi như mobile
            return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android ||
                   UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS;
#else
        // Runtime build: dùng Application.platform
        return Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer;
#endif
        }

        // Các hàm tiện ích bổ sung (nếu cần)
        public static bool IsAndroid()
        {
#if UNITY_ANDROID
        return true;
#elif UNITY_EDITOR
            return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android;
#else
        return Application.platform == RuntimePlatform.Android;
#endif
        }

        public static bool IsIOS()
        {
#if UNITY_IOS
        return true;
#elif UNITY_EDITOR
            return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS;
#else
        return Application.platform == RuntimePlatform.IPhonePlayer;
#endif
        }

      
      
        static Dictionary<string, Scene> sceneLoaded = new Dictionary<string, Scene>();



        public static async UniTask<Scene> LoadNewScene(string startScene)
        {
            await UniTask.Yield();
            // doc dict va unload
            await SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);
            return SceneManager.GetSceneByName(startScene);
        }
        public static async UniTask<Scene> LoadScene(string sceneName)
        {
            // 1. Dọn dẹp: Tìm và Unload tất cả các Scene cũ đang có trong Dictionary
            // Chúng ta chuyển ToList để tránh lỗi "Collection was modified" khi đang duyệt
            var keys = sceneLoaded.Keys.ToList();
            foreach (var name in keys)
            {
                if (sceneLoaded.TryGetValue(name, out Scene oldScene) && oldScene.isLoaded)
                {
                    await SceneManager.UnloadSceneAsync(oldScene);
                }
            }
            sceneLoaded.Clear(); // Xóa sạch danh sách cũ

            // 2. Nạp Scene mới (vẫn dùng Additive để kiểm soát mượt mà)
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // 3. Lấy Scene vừa nạp và lưu vào Dictionary
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid())
            {
                sceneLoaded[sceneName] = newScene;
                // Quan trọng: Set Active để Lighting/Navmesh của cảnh mới được ưu tiên
                SceneManager.SetActiveScene(newScene);

            }

            return newScene;
        }

        public static float HeightRatio(int v)
        {
            return GetScreenSize().y / v;
        }
        public static GameObject CreateClone(this Canvas rootCanvas, GameObject original, Vector2 position, string cloneName = "DragClone")
        {
            // 1. Tạo GameObject và đảm bảo nó có RectTransform ngay từ đầu
            GameObject _draggingClone = new GameObject(cloneName, typeof(RectTransform));

            // 2. Thiết lập Parent TRƯỚC khi chỉnh thông số để tránh sai lệch tọa độ
            _draggingClone.transform.SetParent(rootCanvas.transform, false);
            _draggingClone.transform.SetAsLastSibling();

            // 3. Sao chép thông số RectTransform (Cực kỳ quan trọng để fix lỗi Scale/Size)
            RectTransform oriRT = original.GetComponent<RectTransform>();
            RectTransform rt = _draggingClone.GetComponent<RectTransform>();

            // Copy mọi thông số để tỉ lệ giống hệt bản gốc
            rt.anchorMin = oriRT.anchorMin;
            rt.anchorMax = oriRT.anchorMax;
            rt.pivot = oriRT.pivot;
            rt.sizeDelta = oriRT.sizeDelta;
            rt.localScale = oriRT.localScale; // Giữ nguyên tỉ lệ Scale của bản gốc

            // 4. Sao chép Image
            Image originalImage = original.GetComponent<Image>();
            if (originalImage != null)
            {
                Image cloneImage = _draggingClone.AddComponent<Image>();
                cloneImage.sprite = originalImage.sprite;
                cloneImage.raycastTarget = false;
                // Giữ nguyên thuộc tính Preserve Aspect nếu bản gốc có dùng
                cloneImage.preserveAspect = originalImage.preserveAspect;
            }

            // 5. Vị trí và Hiệu ứng
            _draggingClone.transform.position = position;
            CanvasGroup group = _draggingClone.AddComponent<CanvasGroup>();
            group.alpha = 0.6f;
            group.blocksRaycasts = false; // Đảm bảo OnPointerUp truyền xuống dưới được

            return _draggingClone;
        }
        public static T LoadPrefab<T>(string path,Transform parent=null)
        {
            var go = LoadPrefabs(path, parent);
            return go.GetComponent<T>();
        }
        public static T LoadAsset<T>(string key) where T : UnityEngine.Object
        {
            
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

            
            T result = handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return result;
            }

            Debug.LogError($"Fail to load: {key}. Status: {handle.Status}");
            return null;
        }
        public static GameObject LoadPrefabs(string v, Transform parent = null)
        {
            string address = $"Assets/Prefabs/{v}.prefab";
            if (v.Contains(".prefab"))
            {
                address = v;
            }
            // 1. Tạo handle. Lưu ý: InstantiateAsync trả về Handle ngay lập tức (chưa xong)
            var handle = Addressables.InstantiateAsync(address, parent);

            // 2. Ép chạy đồng bộ và lấy kết quả
            // Cảnh báo: Việc này sẽ làm khựng game một chút
            GameObject ret = handle.WaitForCompletion();
            ret.name = address;
            // 3. Kiểm tra trạng thái sau khi đã đợi xong
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                // Giải phóng handle nếu lỗi để tránh rác bộ nhớ
                Addressables.Release(handle);
                throw new Exception($"Không thể load Prefab tại: Assets/Prefabs/{v}. Có thể file chưa được tích 'Addressable' hoặc sai đường dẫn.");
            }

            return ret;
        }
        public static async UniTask<T> LoadPrefabsAsync<T>(string v, Transform parent = null)
        {
            GameObject go=await LoadPrefabsAsync(v, parent);
            go.SetActive(false);
            return go.GetComponent<T>();    
        }
        public static async UniTask<GameObject> LoadPrefabsAsync(string v, Transform parent = null)
        {

            string address = $"Assets/Prefabs/{v}.prefab";
            if (v.EndsWith(".prefab"))
            {
                address = v;
            }
            var handle = Addressables.InstantiateAsync(address, parent);

            // Chờ nhưng không làm treo Main Thread
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                handle.Result.name = address;
                return handle.Result;
            }
            else
            {
                Addressables.Release(handle);
                Debug.LogError($"Lỗi load prefab: {address}");
                return null;
            }
        }
        
#if UNITY_EDITOR
        public static Vector2 GetCameraWorldSizeEditorMode(float orthographicSize)
        {
            float height = orthographicSize * 2f;
            float aspect = 1.7777f; // Mặc định 16:9 nếu không tìm thấy camera

            // 1. Thử lấy Camera chính trong Scene (hoạt động cả khi không Play)
            //Camera cam = Camera.main;

            // 2. Nếu Camera.main null (thường bị trong Editor), tìm tất cả camera
            //if (cam == null) cam = GameObject.FindObjectOfType<Camera>();

            //if (cam != null)
            //{
            //    aspect = cam.aspect;
            //}
            //else
            //{
            //    // 3. Dự phòng: Lấy aspect ratio từ cửa sổ Game hiện tại trong Editor

            //    System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            //    UnityEditor.EditorWindow gameView = UnityEditor.EditorWindow.GetWindow(T);
            //    var prop = gameView.GetType().GetProperty("targetSize",
            //        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            //    if (prop != null)
            //    {
            //        Vector2 size = (Vector2)prop.GetValue(gameView);
            //        aspect = size.x / size.y;
            //    }

            //}

            return new Vector2(height * aspect, height);
        }
#endif
        public static Vector2 GetCameraWorldSize()
        {
            Camera cam = Camera.main;
            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect; // cam.aspect = Screen.width / Screen.height
            return new Vector2(width, height);
        }

        public static void DoMapAction(object source, object target)
        {
            var sourceMapType = source.GetType();

            // 2. Get properties from your target 'Player' object (the one receiving the actions)
            var targetProperties = target.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(InputAction) && p.CanWrite);

            foreach (var targetProp in targetProperties)
            {
                // 3. Find the matching property in the source Action Map
                var sourceProp = sourceMapType.GetProperty(targetProp.Name);

                if (sourceProp != null)
                {
                    // 4. CRITICAL: Pass 'sourcePlayerMap' as the instance, NOT 'this.inputs'
                    var actionValue = sourceProp.GetValue(source);

                    targetProp.SetValue(target, actionValue, null);

                    
                }
            }
        }
        public static void DoMapPalerAndUIAction(object source, object target)
        {
            var sourcePlayerProperty = source.GetType().GetProperty("Player");
            if (sourcePlayerProperty == null) return;
            var sourcePlayerValue = sourcePlayerProperty.GetValue(source);
            if (sourcePlayerValue == null) return;
            var targetPlayerProperty = target.GetType().GetProperty("Player");
            if (targetPlayerProperty == null) return;
            var targetPlayerValue = targetPlayerProperty.GetValue(target);
            if (targetPlayerValue == null) return;
            Commons.DoMapAction(sourcePlayerValue, targetPlayerValue);
            targetPlayerProperty.SetValue(target, targetPlayerValue);
            var sourceUIProperty = source.GetType().GetProperty("UI");
            if (sourceUIProperty == null) return;
            var sourceUIValue = sourceUIProperty.GetValue(source);
            if (sourceUIValue == null) return;
            var targetUIProperty = target.GetType().GetProperty("UI");
            if (targetUIProperty == null) return;
            var targetUIValue = targetUIProperty.GetValue(target);
            Commons.DoMapAction(sourceUIValue, targetUIValue);
            targetUIProperty.SetValue(target, targetUIValue);
            
            
        }
    }
}