#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;

using System.Reflection;

using UnityEditor;

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;



using unvs.components;
using unvs.game2d.scenes;
using unvs.shares;
using unvs.types;
using unvs.ui;
using static System.Net.WebRequestMethods;
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
        public static string GetAbsFolderPathOfGameObject(GameObject go)
        {
            var folder = EditorGetFolder(go);
            return EditorTools.ToAbsolutePath(folder);
        }
        public static string GetAbsFilePathOfGameObject(Texture2D txt)
        {
            var folder = EditorGetAssetPath(txt);
            return EditorTools.ToAbsolutePath(folder);
        }
        public static void OpenSpriteEditor(string assetPath)
        {
            // 1. Chuẩn hóa đường dẫn (Quan trọng nhất)
            string cleanPath = assetPath.Replace("\\", "/");
            if (cleanPath.StartsWith(Application.dataPath))
            {
                cleanPath = "Assets" + cleanPath.Substring(Application.dataPath.Length);
            }

            // 2. Load Asset
            UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(cleanPath);
            if (asset == null)
            {
                Debug.LogError($"[Unvs] Không tìm thấy Asset tại: {cleanPath}. Hãy đảm bảo path bắt đầu bằng 'Assets/'.");
                return;
            }

            // 3. Chọn đối tượng để Sprite Editor có ngữ cảnh làm việc
            Selection.activeObject = asset;

            // 4. Tìm Type bằng cách quét tất cả Assembly (Fix cho Unity mới)
            Type spriteEditorWindowType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == "SpriteEditorWindow");

            if (spriteEditorWindowType != null)
            {
                EditorWindow.GetWindow(spriteEditorWindowType);
                Debug.Log($"[Unvs] Đã mở Sprite Editor cho: {cleanPath}");
            }
            else
            {
                Debug.LogError("Vẫn không tìm thấy Sprite Editor Window. " +
                               "Kiểm tra Window > Package Manager xem '2D Sprite' đã installed chưa.");
            }
        }
        public static string EditorGetAssetPath(Texture2D txt)
        {
            return AssetDatabase.GetAssetPath(txt);
        }

        public static string GetAbsFilePathOfGameObject(GameObject go)
        {
            var folder = EditorGetAssetPath(go);
            return EditorTools.ToAbsolutePath(folder);
        }
        public static string EditorGetAssetPath(GameObject go)
        {
            if (go == null) return string.Empty;

            // 1. Tìm đường dẫn Asset của GameObject (ví dụ: Assets/Prefabs/Player.prefab)
            string assetPath = AssetDatabase.GetAssetPath(go);

            // Nếu GameObject này không phải là Asset (chỉ là object tạm trong Scene chưa lưu)
            if (string.IsNullOrEmpty(assetPath))
            {

                return "";
            }
            return assetPath;
           
        }
        public static string EditorGetFolder(GameObject go)
        {
            if (go == null) return string.Empty;

            // 1. Tìm đường dẫn Asset của GameObject (ví dụ: Assets/Prefabs/Player.prefab)
            string assetPath = GetAddress(go);// AssetDatabase.GetAssetPath(go);

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
        public static AssetReference EditorGetAssetReference(GameObject go)
        {
            return CreateAssetReference(EditorGetAddressFromInspector(go));
        }
        public static string EditorGetAddressFromInspector(GameObject go)
        {
            if (go == null) return "Null";

            string guid = string.Empty;

            // 1. Kiểm tra nếu đang ở trong Prefab Mode (Cửa sổ chỉnh sửa Prefab riêng biệt)
            var prefabStage = PrefabStageUtility.GetPrefabStage(go);
            if (prefabStage != null)
            {
                // Trong Prefab Mode, đường dẫn nằm ở prefabAssetPath
                guid = AssetDatabase.AssetPathToGUID(prefabStage.assetPath);
            }
            else
            {
                // 2. Nếu ở Scene bình thường, tìm file gốc trên đĩa
                UnityEngine.Object source = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (source != null)
                {
                    string path = AssetDatabase.GetAssetPath(source);
                    guid = AssetDatabase.AssetPathToGUID(path);
                }
                else
                {
                    // 3. Trường hợp cuối: Thử lấy path trực tiếp (nếu go chính là Asset đang được chọn)
                    string path = AssetDatabase.GetAssetPath(go);
                    guid = AssetDatabase.AssetPathToGUID(path);
                }
            }

            if (string.IsNullOrEmpty(guid))
                return "Not an Asset (Hierarchy Only)";

            // 4. Truy vấn Address từ GUID
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return "Settings Missing";

            var entry = settings.FindAssetEntry(guid);
            return entry != null ? entry.address : "Not Addressable";
        }
        public static string EditorGetTrueFolder(GameObject go)
        {
            if (go == null) return string.Empty;

            // 1. Tìm đường dẫn Asset của GameObject (ví dụ: Assets/Prefabs/Player.prefab)
            string assetPath = GetAssetTruePath(go);// AssetDatabase.GetAssetPath(go);

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
        /// <summary>
        /// Get folder path of any avalable object
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static string EditorGetAbsFolderPath(UnityEngine.Object go)
        {
            return EditorTools.ToAbsolutePath(EditorGetTrueAssetPath(go));
        }
        public static string EditorGetTrueAssetPath(UnityEngine.Object go)
        {
            if (go == null) return string.Empty;

            // 1. Tìm đường dẫn Asset của GameObject (ví dụ: Assets/Prefabs/Player.prefab)
            string assetPath = GetAssetTruePath(go);// AssetDatabase.GetAssetPath(go);

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
        public static string GetAssetTruePath(UnityEngine.Object obj)
        {
            if (obj == null) return string.Empty;

            // 1. Thử lấy path trực tiếp (Dành cho file gốc trong Project window)
            string path = AssetDatabase.GetAssetPath(obj);

            // 2. Nếu path rỗng, có thể đây là một Instance của Prefab
            if (string.IsNullOrEmpty(path))
            {
                // Tìm đối tượng gốc (Source Asset) từ Instance
                UnityEngine.Object sourceAsset = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (sourceAsset != null)
                {
                    path = AssetDatabase.GetAssetPath(sourceAsset);
                }
            }

            // 3. Nếu vẫn rỗng (Trường hợp đang mở Prefab Mode hoặc đối tượng tạm)
            if (string.IsNullOrEmpty(path))
            {
                // Lấy path của Stage hiện tại (nếu đang mở cửa sổ edit Prefab)
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    path = stage.assetPath;
                }
            }

            return path;
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
            foreach (var item in items)
            {
                if (typeof(T) == typeof(Transform))
                {
                    if (((Transform)(object)item).parent == tr.parent)
                    {
                        ((Transform)(object)item).SetParent(tr, true);
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
                }
                else
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
        public static AssetReference GetReference(MonoBehaviour target)
        {
            if (target == null) return null;

            // Lấy tất cả các field (biến) của class, bao gồm cả biến private
            // Chúng ta tìm các field mà kiểu dữ liệu của nó là AssetReference hoặc kế thừa từ AssetReference
            FieldInfo field = target.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(f => typeof(AssetReference).IsAssignableFrom(f.FieldType));

            if (field != null)
            {
                // Trả về giá trị thực tế của biến đó từ đối tượng target
                return field.GetValue(target) as AssetReference;
            }

            Debug.LogWarning($"Class {target.GetType().Name} không chứa bất kỳ biến AssetReference nào.");
            return null;
        }

        public static FieldInfo[] GetAllGenericFieldsOfType(Type typ, Type Ftyp)
        {

            return typ.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f =>
                {
                    Type t = f.FieldType;

                    // 1. Kiểm tra trực tiếp xem có khớp với typ không (nếu typ không phải generic)
                    if (typ.IsAssignableFrom(t)) return true;

                    // 2. Leo ngược cây thừa kế để tìm Open Generic (UnvsProperty<>)
                    while (t != null && t != typeof(object))
                    {
                        if (t.IsGenericType && t.GetGenericTypeDefinition() == Ftyp)
                            return true;
                        t = t.BaseType;
                    }
                    return false;
                }).ToArray();

        }
        public static PropertyInfo[] GetAllGenericProperties(object instance, Type typ)
        {
            if (instance == null) return null;
            return instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f =>
                {
                    Type t = f.PropertyType;

                    // 1. Kiểm tra trực tiếp xem có khớp với typ không (nếu typ không phải generic)
                    if (typ.IsAssignableFrom(t)) return true;

                    // 2. Leo ngược cây thừa kế để tìm Open Generic (UnvsProperty<>)
                    while (t != null && t != typeof(object))
                    {
                        if (t.IsGenericType && t.GetGenericTypeDefinition() == typ)
                            return true;
                        t = t.BaseType;
                    }
                    return false;
                }).ToArray();

        }
        public static PropertyInfo[] GetAllGenericPropertiesOfType(Type typ, Type Ftyp)
        {

            return typ.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f =>
                {
                    Type t = f.PropertyType;

                    // 1. Kiểm tra trực tiếp xem có khớp với typ không (nếu typ không phải generic)
                    if (typ.IsAssignableFrom(t)) return true;

                    // 2. Leo ngược cây thừa kế để tìm Open Generic (UnvsProperty<>)
                    while (t != null && t != typeof(object))
                    {
                        if (t.IsGenericType && t.GetGenericTypeDefinition() == Ftyp)
                            return true;
                        t = t.BaseType;
                    }
                    return false;
                }).ToArray();

        }

        public static void OpenSpriteEditor(Texture2D texture)
        {
            var spriteRenderPath = unvs.editor.utils.UnvsEditorUtils.EditorGetAssetPath(texture);
            OpenSpriteEditor(spriteRenderPath);
        }

        public static void ForceEnableZWriteAllMaterial()
        {
            // 1. Quét toàn bộ Material trong Project
            string[] guids = AssetDatabase.FindAssets("t:Material");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

                // 2. Ép mọi Material tuân thủ luật Z-Buffer thông minh'
                if (mat.shader.name.Contains("Universal Render Pipeline"))
                {
                    Debug.Log($"Universal Render Pipeline={mat.shader.name}");
                }
                mat.SetFloat("_DepthBias", -1.0f);
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
                if (mat.HasProperty("_AlphaClip"))
                {
                    mat.SetFloat("_AlphaClip", 1);
                    // Ép Cutoff xuống thấp để giữ lại chân nhân vật, 
                    // nhưng phải đủ cao để bỏ qua phần rỗng hoàn toàn.
                    mat.SetFloat("_Cutoff", 0.05f);

                    // QUAN TRỌNG: Ép Render Queue về đúng thứ tự 3D chuyên nghiệp
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest; // 2450
                    mat.EnableKeyword("_ALPHATEST_ON");
                }
                // CỰC KỲ QUAN TRỌNG: 

            }
            AssetDatabase.SaveAssets();
            Debug.Log("Hệ thống đã được tổng quát hóa sang chuẩn Depth-Buffer 3D.");

            List<Material> allMaterials = new List<Material>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null) allMaterials.Add(mat);
            }

            // Thực hiện logic xử lý
            Undo.RecordObjects(allMaterials.ToArray(), "Globalize Materials");
            foreach (var mat in allMaterials)
            {
                // Code xử lý ZWrite và AlphaClip của bạn ở đây
                ProcessMaterial(mat);
            }

            AssetDatabase.SaveAssets();
            Dialogs.Show($"Đã xử lý {allMaterials.Count} materials.");
        }
        public static void ProcessMaterial(Material mat)
        {
            // 1. Thiết lập ghi chiều sâu
            mat.SetInt("_ZWrite", 1);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);

            // 2. Ép Material mặc định từ Transparent sang Opaque để hỗ trợ Z-Write
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 0); // 0 = Opaque
            if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 0);     // 0 = Alpha Test

            // 3. Kích hoạt Alpha Clipping để xóa phần trắng thừa
            if (mat.HasProperty("_AlphaClip"))
            {
                mat.SetFloat("_AlphaClip", 1);
                mat.SetFloat("_Cutoff", 0.1f);
                mat.EnableKeyword("_ALPHATEST_ON");

                // Tắt keyword của Transparent để tránh xung đột
                mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 2450;
            }

            EditorUtility.SetDirty(mat);
        }
      
        /// <summary>
        /// This function will load scene as editor mode and add goScene to scene
        /// Before add, clean all object in scene
        /// Note: function just serve for editor who woul like to review goScene beneath Cam, Light,...
        /// </summary>
        /// <param name="testSceneAssetPath"></param>
        /// <param name="sceneName"></param>
        /// <param name="unvsScene"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static async UniTask EditorReview(string testSceneAssetPath, string sceneName,  AssetReference selRef)
        {
           
            var scenePath = $"{testSceneAssetPath}/{sceneName}.unity";
            // 1. Kiểm tra file scene có tồn tại không
            if (string.IsNullOrEmpty(testSceneAssetPath) || AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
               
                Dialogs.Show($"[EditorReview] Can not find scene at: {scenePath}");
                return;
            }

            // 2. Mở Scene ở chế độ Single (đóng các scene hiện tại)
            // Dùng OpenSceneMode.Single để đảm bảo môi trường sạch
            UnityEngine.SceneManagement.Scene testScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            if (!testScene.IsValid())
            {
                Dialogs.Show($"[EditorReview] Can not create or initialize scene {sceneName}");
                return;
            }

            // 3. Dọn dẹp tất cả các đối tượng cũ trong Scene
            // Chúng ta duyệt qua các root objects để xóa
            GameObject root = testScene.GetRootGameObjects().FirstOrDefault(p=>p.name=="root");
            if(root == null)
            {
                Dialogs.Show($"[EditorReview] {sceneName} do not have root");
                return;
            }
            var rootObjects =root.GetComponentsInChildren<Transform>();
            foreach (var obj in rootObjects)
            {
                // Lưu ý: Chỉ xóa các object không phải là hệ thống quan trọng nếu cần
                // Ở đây xóa tất cả theo yêu cầu "clean all object in scene"
                UnityEngine.Object.DestroyImmediate(obj);
            }
            var goScene = await Commons.LoadPrefabsAsync<UnvsInteractUI>(selRef, root.transform, true);
            // 4. Instantiate goScene vào Scene vừa mở
            if (goScene != null)
            {
                // Tạo bản sao của goScene vào Scene hiện tại
                //GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(goScene, testScene);

                //if (instance == null)
                //{
                //    // Nếu không phải prefab, dùng Instantiate thông thường
                //    instance = UnityEngine.Object.Instantiate(goScene);
                //    SceneManager.MoveGameObjectToScene(instance, testScene);
                //}

                //instance.name = $"[Review] {goScene.name}";
                //instance.transform.position = Vector3.zero;

                // 5. Đánh dấu Scene đã thay đổi (nhưng không lưu để tránh hỏng file gốc)
                //EditorSceneManager.MarkSceneDirty(testScene);

                Debug.Log($"[EditorReview] Đã chuẩn bị xong vùng Review cho: {sceneName}");

                // Focus vào object mới trong Scene View
                //Selection.activeGameObject = instance;
                //SceneView.FrameLastActiveSceneView();
            }
        }
        public static void FullRestoreMaterials()
        {
            string[] guids = AssetDatabase.FindAssets("t:Material");
            List<Material> allMaterials = new List<Material>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null) allMaterials.Add(mat);
            }

            Undo.RecordObjects(allMaterials.ToArray(), "Full Restore Materials");

            foreach (var mat in allMaterials)
            {
                // 1. Khôi phục thông số cơ bản
                if (mat.HasProperty("_DepthBias")) mat.SetFloat("_DepthBias", 0.0f);

                // Trả về mặc định của Shader (Opaque thường là 2000)
                mat.renderQueue = -1;

                // 2. Đưa Surface Type về Opaque (Giá trị 0)
                if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 0);
                if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 0);

                // 3. Tắt Alpha Clip (Thứ gây rắc rối cho bóng đổ nếu không setup đúng)
                if (mat.HasProperty("_AlphaClip"))
                {
                    mat.SetFloat("_AlphaClip", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                }

                // 4. Thiết lập Z-Buffer chuẩn cho vật thể 3D
                mat.SetInt("_ZWrite", 1);
                mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);

                // 5. KHÔI PHỤC KEYWORDS (Rất quan trọng)
                // Tắt các keyword do hàm cũ ép vào
                mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

                // Kích hoạt lại các keyword mặc định của Opaque nếu cần
                // (Unity sẽ tự động cập nhật lại dựa trên các SetFloat ở trên khi gọi Reset)

                // 6. ÉP UNITY UPDATE LẠI MATERIAL UI VÀ INTERNAL STATE
                // Điều này giúp Inspector hiển thị đúng các mục đã bị script cũ ẩn đi
                if (mat.shader.name.Contains("Universal Render Pipeline/Lit"))
                {
                    mat.SetOverrideTag("RenderType", "Opaque");
                }

                EditorUtility.SetDirty(mat);
            }

            AssetDatabase.SaveAssets();
            Dialogs.Show($"Đã khôi phục hoàn toàn {allMaterials.Count} materials. Hãy thử tạo Cube mới để test bóng.");
        }
    }



}
#endif