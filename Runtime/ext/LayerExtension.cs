using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace unvs.ext
{
    public static class LayerExtension
    {
       // [System.Diagnostics.DebuggerStepThrough]
        public static void SetMeOnLayer(this Behaviour behaviour, string layerName, bool applyAllChildren = false)
        {
            var index = LayerMask.NameToLayer(layerName);
            if (index == -1)
            {
                throw new System.Exception($"Please create layer '{layerName}'");
            }

            // Gán layer cho đối tượng chính
            behaviour.gameObject.layer = index;

            if (applyAllChildren)
            {
                // Sử dụng Transform để lấy tất cả thành phần Transform của con và chính nó
                var allTransforms = behaviour.GetComponentsInChildren<Transform>(true);
                foreach (var t in allTransforms)
                {
                    t.gameObject.layer = index;
                }
            }
            
        }
      //  [System.Diagnostics.DebuggerStepThrough]
        //public static void SetMeSortingGroup(this Behaviour behaviour, string layerName, bool applyAllChildren = false)
        //{
        //    var st = behaviour.GetComponent<SortingGroup>();
        //    if (st != null)
        //    {
        //        st.sortingLayerName = layerName;
        //    }

        //    if (applyAllChildren)
        //    {
        //        // Sử dụng Transform để lấy tất cả thành phần Transform của con và chính nó
        //        var allTransforms = behaviour.GetComponentsInChildren<SortingGroup>(true);
        //        foreach (var t in allTransforms)
        //        {
        //            st.sortingLayerName = layerName;
        //        }
        //    }
        //}
        public static void SetMeOnLayer(this GameObject go, string layerName, bool applyAllChildren = false)
        {
            var index = LayerMask.NameToLayer(layerName);
            if (index == -1)
            {
                throw new System.Exception($"Please create layer '{layerName}'");
            }
            go.layer = index;

            if (applyAllChildren)
            {
                // Sử dụng Transform để lấy tất cả thành phần Transform của con và chính nó
                var allTransforms = go.GetComponentsInChildren<Transform>(true);
                foreach (var t in allTransforms)
                {
                    t.gameObject.layer = index;
                }
            }
        }
    }
}
