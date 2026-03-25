using System;
using System.Collections;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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