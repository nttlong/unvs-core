
#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using unvs.editor.components;
using unvs.game2d.scenes;
using unvs.shares.editor;


namespace unvs.core.editorlibs
{
    public class Dialogs
    {
        public static void Show(string msg)
        {
            EditorUtility.DisplayDialog(
                "Thông báo",
                msg,
                "OK"
            );
        }
    }
    [System.Serializable]
    public class PythonSenderData
    {
        [SerializeField]
        public Vector2[] Data;
    }
    [System.Serializable]
    public class PythonResponse
    {
        public string status;
        public object result;
        public string detail;
    }

    public class UnvsPythonCall
    {
        private string _pythonPath = "";
        private const string BaseUrl = "http://127.0.0.1:8000";
        /// <summary>
        /// Chuyển đổi đường dẫn từ Assets/ path sang đường dẫn tuyệt đối của hệ điều hành.
        /// </summary>
        /// <param name="relativePath">Đường dẫn bắt đầu bằng Assets/...</param>
        /// <returns>Đường dẫn tuyệt đối (C:/...)</returns>
        public static string ToAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;

            // Path.GetFullPath khi chạy trong Unity Editor sẽ tự động lấy 
            // thư mục chứa project làm gốc.
            string absolutePath = Path.GetFullPath(relativePath);

            // Chuẩn hóa dấu gạch chéo sang "/" để tránh lỗi khi gửi sang Python/JSON
            return absolutePath.Replace("\\", "/");
        }
        public static async UniTask<bool> HealthCheck()
        {
            try
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get($"{BaseUrl}/healthcheck"))
                {
                    await webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        string result = webRequest.downloadHandler.text.Trim('"'); // Xóa dấu ngoặc kép nếu FastAPI trả về dạng JSON string
                       
                        Dialogs.Show($" API server can be connected");
                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Dialogs.Show($"[UnvsPythonCall] HealthCheck failed: {e.Message}");
               
            }

            return false;
        }

        public static async UniTask<string> Call(string module, string func, object data)
        {
            string url = $"{BaseUrl}/call";
            try
            {
                // Sử dụng Newtonsoft.Json để serialize toàn bộ payload một cách an toàn
                var payload = new
                {
                    module = module,
                    func = func,
                    json_data = data
                };
                string fullJson = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

                using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
                {
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(fullJson);
                    webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.SetRequestHeader("Content-Type", "application/json");

                    await webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        string jsonResponse = webRequest.downloadHandler.text;
                        // Sử dụng Newtonsoft để deserialize linh hoạt hơn
                        PythonResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<PythonResponse>(jsonResponse);
                        
                        if (response != null && response.status == "success")
                        {
                            return response.result?.ToString();
                        }
                        else
                        {
                            string detail = response?.detail ?? "Unknown Python error";
                            Dialogs.Show($"[Python API Error]\nModule: {module}\nFunc: {func}\nDetail: {detail}");
                        }
                    }
                    else
                    {
                        Dialogs.Show($"[Network Error]\nURL: {url}\nError: {webRequest.error}\nCode: {webRequest.responseCode}");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[UnvsPythonCall] Call failed: {e.Message}");
                Dialogs.Show($"[Exception Occurred]\nMessage: {e.Message}\nURL: {url}");
            }

            return null;
        }
    }
    
    public class EditorTools
    {
        public static string EditorGetAddressPath(AssetReference myRef)
        {

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(myRef.AssetGUID);
            if (entry != null)
            {
                return entry.address;
            }
            return string.Empty;
        }
        public static string ToAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;

            // Path.GetFullPath khi chạy trong Unity Editor sẽ tự động lấy 
            // thư mục chứa project làm gốc.
            string absolutePath = Path.GetFullPath(relativePath);

            // Chuẩn hóa dấu gạch chéo sang "/" để tránh lỗi khi gửi sang Python/JSON
            return absolutePath.Replace("\\", "/");
        }
        public static SceneInfoResut GetFolderOfGameObjectByScene(GameObject obj)
        {
            var scene = obj.GetComponentInParent<UnvsScene>();
            if (scene != null)
            {
                if(scene.selRef==null)
                {
                    Dialogs.Show($"{scene}.selRef is null or not set");
                    return default;
                }
                var pathToAsset = UnvsEditorUtils.EditorGetAddressPath(scene.selRef);
                var folder = System.IO.Path.GetDirectoryName(pathToAsset);
                return new SceneInfoResut
                {
                    FolderPath= ToAbsolutePath(folder),
                    AssetPath= ToAbsolutePath(pathToAsset),
                    Name=scene.name
                };
            }
            return default;
        }
        public static SceneInfoResut GetFolderOfGameObjectByScene(UnvsScene scene)
        {

            if (scene.selRef == null)
            {
                Dialogs.Show($"{scene}.selRef is null or not set");
                return default;
            }
            var pathToAsset = UnvsEditorUtils.EditorGetAddressPath(scene.selRef);
            var folder = System.IO.Path.GetDirectoryName(pathToAsset);
            return new SceneInfoResut
            {
                FolderPath = ToAbsolutePath(folder),
                AssetPath = ToAbsolutePath(pathToAsset),
                Name = scene.name
            };
        }
    }
}
#endif
