using Cysharp.Threading.Tasks;

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
        public static async UniTask<MoveInfo2D> MoveToAsync(this Transform transform, float Speed, Vector2 target, Action<MoveInfo2D> OnMoving, Action<MoveInfo2D> OnFinish, CancellationToken token, float distance = 0)
        {
            var ret = new MoveInfo2D();

            // Luôn lấy hướng hiện tại từ transform trước khi bắt đầu
            float currentDir = 0; ;

            try
            {
                // Tính toán bước đầu tiên
                float ds = transform.MoveStep(target, Speed, out currentDir);
                ret.Direction = currentDir;
                OnMoving?.Invoke(ret);

                while (ds > distance)
                {
                    // Quan trọng: UniTask.Yield với token sẽ tự ném OperationCanceledException
                    await UniTask.Yield(PlayerLoopTiming.Update, token);

                    // Cập nhật vị trí và hướng
                    ds = transform.MoveStep(target, Speed, out currentDir);

                    ret.Direction = currentDir;
                    OnMoving?.Invoke(ret);
                }

                OnFinish?.Invoke(ret);
            }
            catch (OperationCanceledException)
            {
                // Khi bị Cancel, không trả về ret rỗng, mà có thể giữ nguyên hướng cuối
                Debug.Log("MoveToAsync was cancelled - Switching target");
            }

            return ret;
        }
        public static async UniTask<MoveInfo2D> MoveToAsync(this Transform transform, float Speed, Vector2 target, Action<MoveInfo2D> OnMoving,Action<MoveInfo2D> OnFinish, CancellationTokenSource cts,float distance=0)
        {
           
            var ret = new MoveInfo2D();
            if (cts == null)
            {
                return ret;
            }
            if (cts.IsCancellationRequested) return ret;
            cts.Token.ThrowIfCancellationRequested();
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
                    if (cts.Token.IsCancellationRequested)
                    {
                        return ret;
                    }
                    // Chờ đến frame tiếp theo
                    await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);

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

#if UNITY_EDITOR
        public static void EditRemoveChildComponentIfExists<T>(this Transform obj, string name) where T : Component
        {
            // Tìm kiếm con trực tiếp theo tên để tránh xóa nhầm "cháu chắt"
            Transform child = obj.Find(name);

            if (child != null)
            {
                // Kiểm tra xem Object đó có chứa Component loại T không
                if (child.GetComponent<T>() != null)
                {
                    // Trong Editor, dùng DestroyImmediate là chuẩn
                    // Dùng child.gameObject trực tiếp vì child != null đã đảm bảo nó tồn tại
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                }
            }
        }
#endif

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
            public Vector2[] CreateRect(float width = 1f, bool centered = true)
            {
                Vector2 dir = (End - Start).normalized;
                Vector2 perp = new Vector2(-dir.y, dir.x);

                float halfW = width * 0.5f;
                Vector2 offset = perp * halfW;

                Vector2 start = centered ? Start : Start - dir * (width * 0.5f); // nếu không centered thì dịch

                return new Vector2[4]
                {
                    start - offset,
                    End   - offset,
                    End   + offset,
                    start + offset
                };
               
            }

            public Vector2 Center()
            {
                return (this.End - this.Start)/2;
            }
        }
        public static Segment GetSegment(this Transform transform)
        {
            // Điểm Start luôn là vị trí của Transform (Khớp nối/Joint)
            Vector2 start = transform.position;

            // Trong Unity 2D Animation, xương thường dài theo trục Up (Y) hoặc Right (X)
            // Để chính xác nhất với hình ảnh bạn gửi, ta sẽ dùng Vector hướng từ vị trí hiện tại
            // đến vị trí của con đầu tiên (nếu có), đó chính là chiều dài thực của xương.
            Vector2 end;

            if (transform.childCount > 0)
            {
                // Nếu có con, điểm kết thúc chính là vị trí của xương con
                end = transform.GetChild(0).position;
            }
            else
            {
                // Nếu không có con (xương cuối cùng), ta mặc định theo hướng 'Right' 
                // nhưng nhân với Scale để bù đắp
                end = start + (Vector2)transform.right * transform.lossyScale.x;
            }

            return new Segment { Start = start, End = end };
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