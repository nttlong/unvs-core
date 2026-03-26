using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D.Animation;
using unvs.shares;
namespace unvs.ext
{
    
    public static class TransformExtension
    {
        public static void MoveContinuous(this Transform transform, Vector2 direction, float speed)
        {
            if (transform == null || direction == Vector2.zero) return;

            // Chỉ di chuyển trên trục X (theo logic của bạn)
            // Nếu muốn di chuyển cả Y, hãy thay 0 bằng direction.y
            Vector3 velocity = new Vector3(direction.x, 0, 0) * speed * Time.deltaTime;

            transform.position += velocity;
        }
        public static Transform FlipX(this Transform transform)
        {
            var scale = transform.localScale;
            var newScale = new Vector2(-scale.x, scale.y);
            transform.localScale = newScale;
            return transform;
        }
        public static float MoveStepByDirection(this Transform transform, Vector2 destination, float walkSpeed)
        {
            if (transform == null) return 0f;

            // Chỉ lấy vị trí đích trên trục X, giữ nguyên Y và Z của transform hiện tại
            Vector3 targetPosition = new Vector3(destination.x, transform.position.y, transform.position.z);

            // Di chuyển mượt mà đến đích, tự động dừng khi chạm đích
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                walkSpeed * Time.deltaTime
            );

            // Trả về khoảng cách còn lại trên trục X
            return Mathf.Abs(targetPosition.x - transform.position.x);
        }
        public static float MoveStep(this Transform transform, Vector2 destination, float WalkSpeed, out float dir, float movDirection = 1)
        {

            if (transform.gameObject.IsDestroyed())
            {
                dir = 0;
                return 0;
            }
            Vector3 moveDir = (new Vector3(destination.x, 0, 0) - new Vector3(transform.position.x, 0, 0)).normalized;
            transform.position += moveDir * WalkSpeed * Time.deltaTime;
            dir = moveDir.normalized.x;
            var dx = destination.x - transform.position.x;
            if (dir > 0 && dx < 0)
            {
                return 0;
            }
            if (dir < 0 && dx > 0)
            {
                return 0;
            }
            return Math.Abs(dx);

        }
        public static async UniTask<MoveInfo2D> MoveToAsync(this Transform transform, float Speed, Vector2 target, Action<MoveInfo2D> OnMoving,Action<MoveInfo2D> OnFinish, CancellationToken ct)
        {

            var ret = new MoveInfo2D();
            if (ct == null)
            {
                return ret;
            }
            if (ct.IsCancellationRequested) return ret;
            ct.ThrowIfCancellationRequested();
            try
            {

                // Lấy hướng ban đầu
                var dir = ret.Direction;

                // Tính toán bước di chuyển đầu tiên
                var ds = transform.MoveStep(target, Speed, out dir);

                ret.Direction = dir; // Cập nhật hướng cho nhân vật
                OnMoving?.Invoke(ret);

                while (ds > 0)
                {
                    // Kiểm tra xem Task có bị hủy (cancel) không (ví dụ khi đổi mục tiêu hoặc thoát game)
                    if (ct.IsCancellationRequested)
                    {
                        return ret;
                    }
                    // Chờ đến frame tiếp theo
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);

                    // Tiếp tục di chuyển và cập nhật ds
                    ds = transform.MoveStep(target, Speed*Time.deltaTime, out dir);

                    ret.Direction = dir; // Cập nhật hướng liên tục để nhân vật quay đúng hướng
                    OnMoving?.Invoke(ret);
                }
                OnFinish?.Invoke(ret);
            }
            catch (OperationCanceledException)
            {
                // Khi bị hủy, trả về dữ liệu hướng tại thời điểm bị hủy
                return ret;
            }

            return ret;

            // Đảm bảo sau khi dừng lại, ds bằng 0 và nhân vật ở đúng đích
        }
        public static T CreateIfNoExist<T>(this Transform transform, string name)
        {
            var ret= transform.gameObject.GetComponentInChildrenByName<T>(name);
            if(ret != null) return ret;
            return transform.Create<T>(name);
        }
        public static T Create<T>(this Transform transform, string name)
        {
            var go=new GameObject(name);
            go.transform.SetParent(transform);
            if (typeof(T) == typeof(Transform)) {
                return (T)((object)go.transform);
            }
            var ret= go.AddComponent(typeof(T));
            return (T)((object)ret);
        }
    
        public static SpriteSkin GetSpriteSkin(this Transform transform)
        {
            var anim=transform.gameObject.GetComponentInParent<Animator>();
            if(anim==null) return null;
            var srs= anim.ExtractAllSpriteSkins();
            var ret=srs.FirstOrDefault(p => p.boneTransforms.Any(p => p == transform.parent));
            return ret;
        }

        public static T AddChildComponentIfNotExist<T>(this Transform obj, string name) where T : Component
        {
            var ret = obj.GetComponentInChildrenByName<T>(name);
            if (ret != null) return ret;
            var go = new GameObject(name);
            go.transform.SetParent(obj.transform);
            if (typeof(T) == typeof(Transform))
            {
                return go.transform as T;
            }
            else
            {
                return go.AddComponent<T>();
            }
        }
    }
   
}