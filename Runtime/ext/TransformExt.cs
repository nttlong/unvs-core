
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using unvs.interfaces;

namespace unvs.ext
{
    public static class TransformExt
    {
        
        public static T GetComponentInChildrenByName<T>(this Transform obj, string Name, bool includeInactive = true) 
        {
            var components = obj.GetComponentsInChildren<T>(includeInactive);

            foreach (var comp in components)
            {
                if (comp is Component c && c.gameObject.name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                {
                    return comp;
                }
            }

            return default(T);
        }
        /// <summary>
        /// Ham nay se tim root bone.
        /// Root Bone duoc dinh nghia la bone ma parent cua no kg thuoc bones
        /// </summary>
        /// <param name="bones"></param>
        /// <returns></returns>
        public static Transform GetRoot(this IEnumerable<Transform> bones)
        {
            if (bones == null) return null;

            var boneSet = new HashSet<Transform>(bones);
            foreach (var bone in bones)
            {
                if (bone == null) continue;

                // Root Bone là bone mà parent của nó không thuộc bones (boneSet)
                if (bone.parent == null || !boneSet.Contains(bone.parent))
                {
                    return bone;
                }
            }

            return null;
        }
        /// <summary>
        /// Tim ta ca cac bone kg co children hoac neu co thi kg co bat cu phan tu nao 
        /// trong children thuoc bone
        /// </summary>
        /// <param name="bones"></param>
        /// <returns></returns>
        public static Transform[] GetAllLeafBones(this IEnumerable<Transform> bones)
        {
            if (bones == null) return new Transform[0];

            var boneSet = new HashSet<Transform>(bones);
            var result = new List<Transform>();

            foreach (var bone in bones)
            {
                if (bone == null) continue;

                bool isLeaf = true;
                // Duyệt qua tất cả các con trực tiếp của bone
                foreach (Transform child in bone)
                {
                    // Nếu bất kỳ con nào nằm trong danh sách bones ban đầu, bone này không phải là leaf
                    if (boneSet.Contains(child))
                    {
                        isLeaf = false;
                        break;
                    }
                }

                if (isLeaf)
                {
                    result.Add(bone);
                }
            }

            return result.ToArray();
        }

        public static T AddChildIfNotExist<T>(this Transform tr, string name) where T : Component
        {
            var ret = tr.GetComponentInChildrenByName<T>(name);
            if (ret != null) return ret;

            var go = new GameObject(name);
            go.transform.SetParent(tr);
            
            if (typeof(T) == typeof(Transform))
            {
                return (T)(object)go.transform;
            }

            return go.AddComponent<T>();
        }

        public static void AttachToParent(this Transform child, Transform parent)
        {
            child.SetParent(parent, false);
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;//physical.CurrentHoldingObject.gameObject.SetActive(true);
        }
        public static Transform GetRoot(this Transform tr)
        {
            if (tr.parent == null) return tr;
            return tr.parent.GetRoot();
        }
        public static Transform GetRootBone(this Transform tr)
        {
            var allBones = tr.GetRoot().GetComponentsInChildren<SpriteSkin>(true).SelectMany(p => p.boneTransforms);
            
            var travel = tr;
            var ret = tr;
            while (allBones.Contains(travel))
            {
                
                ret = travel;
                travel = travel.parent;
            }
            return ret;
        }
        public static IKManager2D CreateRootIKManager2DIfNotExist(this Transform tr)
        {
            var r = tr.GetRootBone();
            if (r == null) return null;
            return r.AddComponentIfNotExist<IKManager2D>();
        }
    }
}