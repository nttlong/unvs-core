using System;
using System.Collections;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D.Animation;
using unvs.game2d.scenes.actors;

namespace unvs.ext
{
   public static class MonoBehaviourExt
    {

        public static T GetComponentInChildrenByName<T>(this MonoBehaviour obj, string Name, bool includeInactive = true) 
        {
            // Cách này nhanh hơn: Tìm tất cả component T trong các con trước
            var components = obj.GetComponentsInChildren<Transform>(includeInactive).Where(p => !p.gameObject.IsDestroyed());

            foreach (var comp in components)
            {
                // Kiểm tra tên (bỏ qua hoa thường và có thể Trim khoảng trắng nếu cần)
                if (comp.gameObject.name.Equals(Name, StringComparison.OrdinalIgnoreCase) && !comp.gameObject.IsDestroyed())
                {
                    var ret= comp.GetComponent<T>();
                    if(ret != null) return ret;
                    return default(T);
                }
            }

            return default(T);
        }
        public static T AddChildIfNotExist<T>(this MonoBehaviour obj, string Name, bool includeInactive = true) where T : Component
        {
            var ret= obj.GetComponentInChildrenByName<T>(Name, includeInactive);
            if (ret != null) return ret;
            var go=new GameObject(Name);
            go.transform.SetParent(obj.transform,true);
            if (typeof(T) == typeof(Transform)){
                return (T)((object) go.transform);
            }
            ret = go.AddComponent<T>();
            //ret.transform.SetParent(obj.transform);
            return ret;
        }
        public static SpriteRenderer FindSpriteRenderer(this MonoBehaviour obj, Transform tr)
        {
            var lst = obj.GetComponentsInChildren<SpriteSkin>(true);
            var item= lst.FirstOrDefault(p=>p.boneTransforms.Contains(tr));
            if(item==null) return null;
            return item.GetComponent<SpriteRenderer>();

        }
        public static void ApplyShadowCasterForAllSpriteRenderers(this MonoBehaviour obj)
        {
            var sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
               var sc= sr.AddComponentIfNotExist<ShadowCaster2D>();
                sc.selfShadows=true;
                sc.castingOption = ShadowCaster2D.ShadowCastingOptions.SelfShadow;
            }
            foreach (var item in obj.GetComponentsInChildren<SpriteRenderer>(true))
            {
                var sc = item.AddComponentIfNotExist<ShadowCaster2D>();
                sc.selfShadows = true;
                sc.castingOption = ShadowCaster2D.ShadowCastingOptions.SelfShadow;
            }
        }
        public static void SetSortingOrder(this MonoBehaviour obj, int order,string sortingLayerName, bool all=true)
        {
            var sr=obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = order;
                sr.sortingLayerName = sortingLayerName;
            }
            if (all)
            {
                foreach (var item in obj.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    item.sortingOrder = order;
                    item.sortingLayerName = sortingLayerName;
                }
            }
        }
        /// <summary>
        /// Check MonoBehaviour is null or destroyed return false 
        /// </summary>
        /// <param name="mono"></param>
        /// <returns></returns>
        public static bool IsValidate(this MonoBehaviour mono)
        {
            if (mono == null || mono.IsDestroyed()|| mono.gameObject==null || mono.gameObject.IsDestroyed()) return false;              // destroyed hoặc chưa gán
            ////if (!mono) return false;                     // Unity fake-null check
            //if (mono.gameObject == null) return false;   // GameObject bị destroy
            //if (!mono.gameObject) return false;
            

            return true;
        }
        /// <summary>
        /// Synchronizes the sorting order and layer of this object (and optionally its children) 
        /// relative to a root SpriteRenderer.
        /// </summary>
        /// <param name="obj">The target MonoBehaviour calling the extension.</param>
        /// <param name="srRoot">The reference SpriteRenderer (e.g., the Character's body).</param>
        /// <param name="isAbove">If true, sets order to Root+1 (Front); if false, sets to Root-1 (Back).</param>
        /// <param name="includeChildren">If true, applies the logic to all SpriteRenderers in the hierarchy.</param>
        public static void SetSortingOrder(this MonoBehaviour obj, SpriteRenderer srRoot, bool isAbove, bool includeChildren = true)
        {
            if (srRoot == null) return;

            // 1. Handle the SpriteRenderer on the object itself
            var sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = srRoot.sortingLayerName;
                sr.sortingOrder = isAbove ? srRoot.sortingOrder + 1 : srRoot.sortingOrder - 1;
            }

            // 2. Handle all children SpriteRenderers
            if (includeChildren)
            {
                // Set 'true' in GetComponentsInChildren to include inactive game objects
                foreach (var item in obj.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    // Sync the layer name first
                    item.sortingLayerName = srRoot.sortingLayerName;

                    // Calculate new order. 
                    // Note: Using a fixed offset (+1/-1) relative to root is best for simple overlays.
                    item.sortingOrder = isAbove ? srRoot.sortingOrder + 1 : srRoot.sortingOrder - 1;
                }
            }
        }
#if UNITY_EDITOR


        /// <summary>
        /// Lấy đường dẫn thư mục chứa script (bắt đầu bằng Assets/...)
        /// </summary>
        public static string EditorModeGetAssetPath(this MonoBehaviour monoBehaviour)
    {
        MonoScript ms = MonoScript.FromMonoBehaviour(monoBehaviour);

        // Lấy đường dẫn đến file .cs (VD: Assets/Scripts/MyFolder/MyScript.cs)
        string fullFilePath = AssetDatabase.GetAssetPath(ms);

        // Giật lùi về 1 cấp để lấy thư mục (VD: Assets/Scripts/MyFolder)
        // Lưu ý: Path.GetDirectoryName sẽ dùng dấu gạch chéo ngược trên Windows, 
        // nên cần Replace lại thành gạch chéo xuôi cho đúng format của Unity.
        string folderPath = Path.GetDirectoryName(fullFilePath).Replace("\\", "/");

        return folderPath;
    }

    /// <summary>
    /// Lấy đường dẫn Addressable của một asset tên 'name' nằm cùng thư mục với script
    /// </summary>
    public static string EditorModeGetAddressablePath(this MonoBehaviour monoBehaviour, string name)
    {
        var folderPath = monoBehaviour.EditorModeGetAssetPath();

        // Kết hợp thư mục và tên file để ra đường dẫn đầy đủ
        return $"{folderPath}/{name}";
    }

       


#endif
    }
}