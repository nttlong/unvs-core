using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using unvs.shares;
namespace unvs.ext
{
    public static class LayerExtension
    {
        // [System.Diagnostics.DebuggerStepThrough]
        public static void SetMeOnTag(this Behaviour behaviour, string tagName)
        {
#if UNITY_EDITOR
            TagHelper.AddTag(tagName);
#endif
            behaviour.gameObject.tag = tagName;
        }
        public static void SetMeOnLayer(this Behaviour behaviour, string layerName, bool applyAllChildren = false)
        {
#if UNITY_EDITOR
            LayerHelper.AddLayer(layerName);
#endif
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
        public static void SetMeOnSortLayer(this Behaviour behaviour, string layerName)
        {
#if UNITY_EDITOR
            if (behaviour.GetComponent<SortingGroup>() == null)
            {
                Debug.LogWarning($"SortingGroup is requirement in {behaviour.name}");
                return;
            }
            LayerHelper.AddSortingLayerIfNotExist(layerName);

#endif
            if (behaviour.GetComponent<SortingGroup>() != null)
            {
                behaviour.GetComponent<SortingGroup>().name = layerName;
            }
             

        }
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
