
#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using unvs.data;
using unvs.editor.components;
using unvs.game2d.scenes;

namespace unvs.editor.utils
{



    public static class GlobalRenderArchitect
    {
        [MenuItem("Unvs/Globalize 3D Render Pipeline")]
        static void Globalize()
        {
            // 1. Quét toàn bộ Material trong Project
            string[] guids = AssetDatabase.FindAssets("t:Material");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

                // 2. Ép mọi Material tuân thủ luật Z-Buffer thông minh
                if (mat != null && mat.shader.name.Contains("Universal Render Pipeline"))
                {
                    // Cho phép ghi chiều sâu
                    mat.SetInt("_ZWrite", 1);
                    // Chỉ vẽ nếu gần Camera hơn hoặc bằng
                    mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);

                    // MẤU CHỐT: Bật Alpha Clipping cho mọi thứ
                    // Điều này giúp phần trong suốt không bao giờ "đục lỗ" vật thể phía sau
                    if (mat.HasProperty("_AlphaClip"))
                    {
                        mat.SetFloat("_AlphaClip", 1);
                        mat.SetFloat("_Cutoff", 0.01f); // Ngưỡng cực nhỏ để giữ chân nhân vật
                        mat.EnableKeyword("_ALPHATEST_ON");
                        mat.renderQueue = (int)RenderQueue.AlphaTest; // 2450
                    }
                }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("Hệ thống đã được tổng quát hóa sang chuẩn Depth-Buffer 3D.");
        }
    }

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
        public static bool Confirm(string msg)
        {
            bool ok = EditorUtility.DisplayDialog("Confirm", msg, "OK", "Cancel");

            return ok;
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
            return absolutePath.Replace("/", System.IO.Path.PathSeparator.ToString());
        }
        public static SceneInfoResut GetFolderOfGameObjectByScene(GameObject obj)
        {
            var scene = obj.GetComponentInParent<UnvsScene>();
            if (scene != null)
            {
                //if (scene.selRef == null)
                //{
                //    Dialogs.Show($"{scene}.selRef is null or not set");
                //    return default;
                //}
                var pathToAsset = UnvsEditorUtils.GetAddress(scene.gameObject);
                var folder = System.IO.Path.GetDirectoryName(pathToAsset);
                var ret = new SceneInfoResut
                {
                    FolderPath = System.IO.Path.Join(ToAbsolutePath(folder), scene.name),
                    AssetPath = ToAbsolutePath(pathToAsset),
                    Name = scene.name
                };
                if (!System.IO.Directory.Exists(ret.FolderPath))
                    try
                    {
                        System.IO.Directory.CreateDirectory(ret.FolderPath);
                    }
                    catch (Exception)
                    {

                    }
                return ret;
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
    public class PsdFile
    {
        public class ShapeData
        {

            public int index;
            public string name;
            public PointData[] points;
            public PointData pivot;
        }
        public class PointData
        {
            public float x;
            public float y;
        }
        public class PsdLayersInfo
        {
            public string file_path;
            public List<ShapeData> shapes;
            public void AddBox(string name, float width, float height)
            {
                var shape = new ShapeData
                {

                    index = 0,
                    name = name,
                    pivot= new PointData
                    {
                        x= width/2,
                        y= height/2
                    },
                    points = new PointData[]
                {
                    new PointData
                    {
                        x=0,y=0
                    },
                    new PointData
                    {
                        x=width,
                        y=0,
                    },
                    new PointData
                    {
                        x=width,y=height
                    }, new PointData
                    {
                        x=0,y=height
                    }
                }

                };
                if (this.shapes == null) this.shapes = new List<ShapeData>();
                this.shapes.Add(shape);
            }

            public void AddBox(Transform item)
            {
                var collider2d=item.GetComponent<Collider2D>();
                if(collider2d is BoxCollider2D  box  && box != null){
                    this.AddBox(box);
                    return;
                }
                if (collider2d is PolygonCollider2D poly && poly != null)
                {
                    this.AddBox(poly);
                    return;
                }
            }

            public void AddBox(PolygonCollider2D poly)
            {
                var shape = new ShapeData
                {

                    index = 0,
                    name = poly.name,
                    pivot = new PointData
                    {
                        x = poly.bounds.size.x/ 2,
                        y = poly.bounds.size.y / 2
                    },
                    points = poly.points.Select(p=> new PointData
                    {
                        x=(float)p.x,
                        y=(float)p.y,
                    }).ToArray()

                };
                if (this.shapes == null) this.shapes = new List<ShapeData>();
                this.shapes.Add(shape);
            }

            public void AddBox(BoxCollider2D box)
            {
                this.AddBox(box.name, box.size.x, box.size.y);
            }
        }

        public static PsdLayersInfo Createlayers(string file_path)
        {
            return new PsdLayersInfo
            {
                file_path = file_path,
            };
        }
    }
    public class SpriteTools
    {

        public static Sprite GetOriginalSprite(Sprite cloneSprite, string folderPath)
        {
            if (cloneSprite == null) return null;

            // 1. Lấy tên sạch (ví dụ: "Lớp 2")
            string cleanName = cloneSprite.name.Replace("(Clone)", "").Trim();

            // 2. Tìm GUID của file chứa Sprite đó (ví dụ file pickable.psd)
            string[] guids = AssetDatabase.FindAssets($"{cleanName} t:Sprite", new string[] { folderPath });

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // 3. QUAN TRỌNG: Load tất cả các Sprite con bên trong file đó
                UnityEngine.Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);

                foreach (var asset in allAssets)
                {
                    // 4. So sánh tên chính xác với Sub-Asset
                    if (asset is Sprite s && s.name == cleanName)
                    {
                        return s;
                    }
                }
            }
            return null;
        }

        public static void ApplyCastShadows2Side(GameObject gameObject)
        {
            var list = new List<SpriteRenderer>();
            var sp = gameObject.GetComponent<SpriteRenderer>();
            if (sp != null)
                list.Add(sp);

            list.AddRange(gameObject.GetComponentsInChildren<SpriteRenderer>(true));

            foreach (var s in list)
            {
                // Sử dụng Reflection để truy cập thuộc tính shadowCastingMode từ lớp cha Renderer
                PropertyInfo shadowProperty = typeof(SpriteRenderer).GetProperty("shadowCastingMode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (shadowProperty != null)
                {
                    // Thiết lập giá trị là TwoSided (tương đương với giá trị 2 trong Debug mode)
                    shadowProperty.SetValue(s, ShadowCastingMode.TwoSided);

                    // Đánh dấu Object có thay đổi để Unity không bỏ qua khi Save Scene/Prefab
                    UnityEditor.EditorUtility.SetDirty(s);
                }
            }

            Debug.Log($"Đã chuyển đổi {list.Count} SpriteRenderer sang Cast Shadows: Two Sided.");
        }
    }
    public static class SceneReviewUtility
    {
        public static UnvsScenePreviewSettings FindSettingsUpwards(string startPath)
        {
            // Đảm bảo đường dẫn bắt đầu bằng Assets
            if (!startPath.StartsWith("Assets"))
            {
                Debug.LogError("Path must start with 'Assets'");
                return null;
            }

            // Nếu startPath là đường dẫn đến file, lấy thư mục cha của nó
            string currentFolder = AssetDatabase.IsValidFolder(startPath)
                ? startPath
                : Path.GetDirectoryName(startPath);

            while (!string.IsNullOrEmpty(currentFolder))
            {
                // Tìm tất cả asset có kiểu UnvsScenePreviewSettings trong thư mục hiện tại
                // "t:UnvsScenePreviewSettings" là cú pháp lọc của AssetDatabase
                string[] guids = AssetDatabase.FindAssets("t:UnvsScenePreviewSettings", new[] { currentFolder });

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    // Kiểm tra chính xác asset có nằm TRỰC TIẾP trong thư mục này không 
                    // (để tránh lấy nhầm asset ở thư mục con sâu hơn)
                    if (Path.GetDirectoryName(assetPath).Replace("\\", "/") == currentFolder.Replace("\\", "/"))
                    {
                        return AssetDatabase.LoadAssetAtPath<UnvsScenePreviewSettings>(assetPath);
                    }
                }

                // Thoát nếu đã lên đến thư mục Assets
                if (currentFolder == "Assets") break;

                // Di chuyển lên thư mục cha
                currentFolder = Path.GetDirectoryName(currentFolder).Replace("\\", "/");
            }

            Debug.LogWarning("No UnvsScenePreviewSettings found in the hierarchy.");
            return null;
        }
    }
}
#endif
