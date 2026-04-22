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

using unvs.shares;
namespace unvs.ext
{

    public static class TransformExtension
    {
       

        public static void MoveContinuous(this Transform transform, Vector2 direction, float speed)
        {
            if (transform == null || direction == Vector2.zero) return;

            transform.position += new Vector3(direction.x > 0 ? 1 : -1, 0, 0) * (speed * Time.deltaTime);

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

        public static async UniTask<MoveInfo2D> MoveToAsync(
    this Transform transform,
    float speed,
    Vector2 target,
    Action<MoveInfo2D> onMoving,
    Action<MoveInfo2D> onFinish,
    CancellationToken token, // Chuyển sang dùng CancellationToken
    float distance = 0)
        {
            var ret = new MoveInfo2D();

            try
            {
                // 1. Kiểm tra ban đầu
                if (transform == null) return ret;
                token.ThrowIfCancellationRequested();

                // 2. Tính toán bước đầu tiên
                var ds = transform.MoveStep(target, speed, out var dir);
                ret.Direction = dir;
                onMoving?.Invoke(ret);

                // 3. Vòng lặp di chuyển
                while (ds > distance)
                {
                    // UniTask.Yield với token sẽ tự động hủy Task nếu token bị Cancel
                    // PlayerLoopTiming.Update giúp đồng bộ với hệ thống vật lý/render
                    await UniTask.Yield(PlayerLoopTiming.Update, token);

                    // CỰC KỲ QUAN TRỌNG: Kiểm tra transform có bị Destroy trong lúc chờ không
                    if (transform == null) return ret;

                    ds = transform.MoveStep(target, speed, out dir);
                    ret.Direction = dir;
                    onMoving?.Invoke(ret);
                }

                // 4. Hoàn thành
                onFinish?.Invoke(ret);
            }
            catch (OperationCanceledException)
            {
                // Log hoặc xử lý riêng khi bị Cancel nếu cần
                return ret;
            }

            return ret;
        }
        public static T CreateIfNoExist<T>(this Transform transform, string name)
        {
            var ret = transform.gameObject.GetComponentInChildrenByName<T>(name);
            if (ret != null) return ret;
            return transform.Create<T>(name);
        }
        public static T Create<T>(this Transform transform, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            if (typeof(T) == typeof(Transform))
            {
                return (T)((object)go.transform);
            }
            var ret = go.AddComponent(typeof(T));
            return (T)((object)ret);
        }

        public static SpriteSkin GetSpriteSkin(this Transform transform)
        {
            var anim = transform.gameObject.GetComponentInParent<Animator>();
            if (anim == null) return null;
            var srs = anim.ExtractAllSpriteSkins();
            var ret = srs.FirstOrDefault(p => p.boneTransforms.Any(p => p == transform.parent));
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
            public Quaternion rotation;

            public float Length
            {
                get
                {
                    return Vector2.Distance(Start, End);
                }
            }
            public Vector2[] CreateRect(Transform tr, float width = 1f)
            {

                var rect = CreateRect(width);
              
                var t = new List<Vector2>();
                foreach (var v in rect)
                {
                    var tt = (Vector2)(Quaternion.Inverse(tr.rotation) * (v - Start));
                    t.Add(tt);
                }
                return t.ToArray();
            }
            public Vector2[] CreateRect(float width = 1f)
            {
                // 1. Tính hướng của đoạn thẳng (Đường màu đỏ)
                Vector2 dir = (End - Start).normalized;

                // 2. Tính vector vuông góc để tạo độ rộng (Width)
                // Trong 2D, vuông góc của (x, y) là (-y, x)
                Vector2 perp = new Vector2(-dir.y, dir.x);

                // 3. Độ rộng chia đôi sang mỗi bên
                float halfW = width * 0.5f;
                Vector2 offset = perp * halfW;

                // 4. Tính toán 4 đỉnh dựa trên Start và End (Đúng như hình vẽ)
                // Điểm 1: Dưới Start
                // Điểm 2: Dưới End
                // Điểm 3: Trên End
                // Điểm 4: Trên Start
                return new Vector2[4]
                {
                    Start - offset, // Điểm góc dưới bên trái (Start)
                    End   - offset, // Điểm góc dưới bên phải (End)
                    End   + offset, // Điểm góc trên bên phải (End)
                    Start + offset  // Điểm góc trên bên trái (Start)
                            };
            }

            public Vector2 Center()
            {
                return (this.End + this.Start) / 2;
            }
        }
        public static Vector2[] Collider2dGeneratePoints(this Transform transform,float width=1f)
        {
            var seg = transform.GetSegment();
            return seg.CreateRect(transform, width);
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

            var ret= new Segment { Start = start, End = end };
            
            ret.rotation = transform.rotation;
            return ret;
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

        public static async UniTask MoveToTargetAsync(
                this Transform socketController,
                 Vector2 targetPos,
                 Action<float> OnMoving = null,
                 CancellationToken token=default,
                 float speed = 5f,
                 float stopDistance = 0.7f)
        {
            while (true)
            {
                if(token.IsCancellationRequested) return;
                token.ThrowIfCancellationRequested();

                Vector2 current = socketController.position;
                float dist = Math.Abs(current.x - targetPos.x);// Vector2.Distance(current, targetPos);
                OnMoving?.Invoke(dist);
                if (dist <= stopDistance)
                {
                    socketController.position = targetPos;
                    break;
                }

                Vector2 next = Vector2.MoveTowards(current, new Vector2(targetPos.x, current.y), speed * Time.deltaTime);
                socketController.position = next;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        public static void AttachItemToSocket(this Transform socketHand, Transform handle)
        {
            // 1. Gán cha cho item vào socket ở tay
            handle.SetParent(socketHand);

            // 2. Đưa vị trí về (0,0,0) so với socket
            handle.localPosition = Vector3.zero;

            // 3. Đưa hướng xoay về (0,0,0) hoặc khớp với hướng của socket
            handle.localRotation = Quaternion.identity;

            // 4. (Tùy chọn) Reset scale nếu item bị biến dạng do xương của nhân vật
            handle.localScale = Vector3.one;
            handle.position = new Vector3(handle.position.x, handle.position.y, socketHand.position.z);
        }
        public static void SetMeOnTag(this Transform tr, string tagName)
        {
            #if UNITY_EDITOR
                        TagHelper.AddTag(tagName);
#endif
            tr.gameObject.tag = tagName;
        }
    }
    public static class UnvsActorPhysicalSolverRuntimeExt
    {
        public static async UniTask<UnvsActorPhysicalSolverRuntime> MoveToAsync(UnvsActorPhysicalSolverRuntime socketController, Vector2 pos,Action<float> OnMoving, CancellationToken token)
        {
            //socketController.Disable();
            await socketController.target.MoveToTargetAsync(pos,OnMoving, token);
            return socketController;
        }
    }
}