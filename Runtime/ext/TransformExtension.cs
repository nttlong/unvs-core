using Cysharp.Threading.Tasks;
using PlasticGui.WorkspaceWindow.QueryViews.Branches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using unvs.interfaces;
using unvs.shares;
namespace unvs.ext
{
    
    public static class TransformExtension
    {
        public static void MoveContinuous(this Transform transform, Vector2 direction, float speed)
        {
            if (transform == null || direction == Vector2.zero) return;
           
            transform.position += new Vector3(direction.x>0?1:-1,0,0) * (speed * Time.deltaTime);
           
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

        public static async UniTask<MoveInfo2D> MoveToAsync(this Transform transform, float Speed, Vector2 target, Action<MoveInfo2D> OnMoving,Action<MoveInfo2D> OnFinish, CancellationToken ct,float distance=0)
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
                var ds = transform.MoveStep(target, Speed , out dir);

                ret.Direction = dir; // Cập nhật hướng cho nhân vật
                OnMoving?.Invoke(ret);

                while (ds > distance)
                {
                    // Kiểm tra xem Task có bị hủy (cancel) không (ví dụ khi đổi mục tiêu hoặc thoát game)
                    if (ct.IsCancellationRequested)
                    {
                        return ret;
                    }
                    // Chờ đến frame tiếp theo
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);

                    // Tiếp tục di chuyển và cập nhật ds
                    ds = transform.MoveStep(target, Speed, out dir);

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
        public struct Segment
        {
            public Vector2 Start;
            public Vector2 End;

            public float Length
            {
                get
                {
                    return Vector2.Distance(Start, End);
                }
            }
        }
        /// <summary>
        /// Lấy tọa độ thế giới của điểm gốc và điểm kết thúc dựa trên hướng Forward và Scale.
        /// </summary>
        public static Segment GetSegment(this Transform transform)
        {
            // Lấy vị trí 2D (tự động bỏ qua Z)
            Vector2 start = transform.position;

            // Lấy hướng "Right" của đối tượng trong không gian thế giới (đã bao gồm xoay)
            // transform.right là vector đơn vị hướng theo trục X cục bộ của object
            Vector2 direction = transform.right;

            // Tính điểm cuối dựa trên Scale X (độ dài của object)
            // Sử dụng lossyScale để tính cả scale của cha nếu có
            Vector2 end = start + direction * transform.lossyScale.x;

            return new Segment
            {
                Start = start,
                End = end
            };
        }
        public static Button AddButtonIfNotExist(this Transform parent, string name, string label)
        {
            // 1. Kiểm tra xem nút có tên này đã tồn tại chưa
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                // Nếu tìm thấy, lấy Component Button và cập nhật lại Label
                Button existingBtn = existing.GetComponent<Button>();
                if (existingBtn != null)
                {
                    // Cập nhật lại text nếu cần
                    var tmp = existingBtn.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null) tmp.text = label;

                    return existingBtn;
                }
            }

            // 2. Nếu chưa có, tiến hành tạo mới hoàn toàn
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false); // parent đã là Transform nên không cần .transform

            // Thiết lập Button Component
            Button btn = btnObj.GetComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;

            ColorBlock cb = ColorBlock.defaultColorBlock;
            cb.selectedColor = new Color(0.7f, 0.9f, 1f);
            btn.colors = cb;

            // 3. Tạo Text con (TextMeshPro)
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(btnObj.transform, false);

            TextMeshProUGUI tmpNew = textObj.GetComponent<TextMeshProUGUI>();
            tmpNew.text = label;
            tmpNew.fontSize = 24;
            tmpNew.color = Color.black;
            tmpNew.alignment = TextAlignmentOptions.Center;

            // 4. Căn chỉnh UI
            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(160, 45);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            return btn;
        }

    }
   
}