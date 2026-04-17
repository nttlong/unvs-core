
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using unvs.interfaces;
using DG.Tweening;
using unvs.shares;
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

        public static void MoveUpByHeight(this Transform transform,float Height)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Height, transform.position.z);
        }
        public static async UniTask MoveUpSlopeAsync(this Transform transform, Vector2 slope, float duration, CancellationToken tk)
        {
            // slope.x: Tốc độ tiến tới (Speed)
            // slope.y: Độ cao cần leo lên (Height)

            // 1. Tính toán điểm đến dựa trên vị trí hiện tại
            Vector3 currentPos = transform.position;
            Vector3 targetPos = new Vector3(
                currentPos.x + (slope.x * duration), // Quãng đường ngang = Vận tốc * Thời gian
                currentPos.y + slope.y,              // Lên đúng độ cao y
                currentPos.z
            );

            // 2. Tính toán quãng đường thực tế (Đường chéo của dốc)
            // d = sqrt(x^2 + y^2)
            float distance = Vector2.Distance(new Vector2(currentPos.x, currentPos.y),
                                              new Vector2(targetPos.x, targetPos.y));

            // 3. Thực hiện di chuyển
            // Dùng Linear nếu là dốc thẳng, hoặc OutQuad nếu muốn nhân vật hơi khựng lại khi lên đỉnh dốc
            await transform.DOMove(targetPos, duration)
                           .SetEase(Ease.Linear)
                           .WithCancellation(tk);
        }
        public static async UniTask MoveUpByHeightAsync(this Transform transform, float height, float duration, CancellationToken tk)

        {

            // Nhảy lên với hiệu ứng mượt mà (Ease.OutQuad: chậm dần khi lên đỉnh)

            await transform.DOMoveY(transform.position.y + height, duration)

                           .SetEase(Ease.OutQuad)

                           .WithCancellation(tk);

        }
        public static async UniTask MoveJumpAsync(this Transform transform, Vector2 slope, float duration, CancellationToken tk)
        {
            float speed = slope.x;
            float height = slope.y;

            // Tính toán điểm đến (Destination) dựa trên hướng và tốc độ
            // Giả sử nhân vật đang nhảy về phía trước theo trục X
            Vector3 targetPosition = new Vector3(
                transform.position.x + (speed * duration),
                transform.position.y, // DOJump sẽ tự xử lý độ cao nhảy (height)
                transform.position.z
            );

            // Sử dụng DOJump để tạo quỹ đạo hình Parabol
            await transform.DOJump(
                                targetPosition,
                                jumpPower: height,
                                numJumps: 1,
                                duration: duration
                            )
                            .SetEase(Ease.Linear) // Trục X đi đều, trục Y tự động Ease theo Jump
                            .WithCancellation(tk);
        }
        public static void CalculateSlopDirection(this Transform tr,ref CalculateSlopeDirectionResull result, float direction, float groundCheckDistance = 2f, string LayerName = Constants.Layers.WORLD_GROUND)
        {

            var pos = tr.GetSegment().Center();
           
             pos.CalculateSlopeDirection(ref result, direction, groundCheckDistance, LayerName);
          
        }
    }
}