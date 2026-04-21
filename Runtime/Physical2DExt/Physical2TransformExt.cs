using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using unvs.shares;

namespace unvs.ext.physical2d
{
    public static class CompositeCollider2DExt
    {
        //public static bool GetHit(this CompositeCollider2D compositeColl, out RaycastHit2D hit, Vector2 direction, float distance = 10f, string Layer = Constants.Layers.WORLD_GROUND, params string[] extra)
        //{
        //    var filter = new ContactFilter2D();
        //    filter.useLayerMask = true;
        //    filter.layerMask = LayerMask.GetMask(Layer);
        //    if (extra.Length > 0)
        //    {
        //        filter.layerMask |= LayerMask.GetMask(extra);
        //    }
        //    filter.useTriggers = false;

        //    RaycastHit2D[] hits = new RaycastHit2D[1];
        //    int count = compositeColl.Raycast(direction, filter, hits, distance);

        //    if (count > 0)
        //    {
        //        hit = hits[0];
        //        Bounds b = compositeColl.bounds;
        //        Debug.DrawLine(b.center, hit.point, Color.red, 5f);
        //        // Kiểm tra xem điểm va chạm có nằm "sau" mép ngoài cùng của Collider theo hướng bắn không
        //        // Nếu điểm va chạm nằm lọt vào bên trong bounds, ta coi đó là lỗi vật lý và bỏ qua
        //        if (direction == Vector2.up && hit.point.y <= b.max.y) return false;
        //        if (direction == Vector2.down && hit.point.y >= b.min.y) return false;
        //        if (direction == Vector2.right && hit.point.x <= b.max.x) return false;
        //        if (direction == Vector2.left && hit.point.x >= b.min.x) return false;

        //        // Vẽ để debug trực quan
                
        //        return true;
        //    }

        //    hit = new RaycastHit2D();
        //    return false;
        //}
    }
    public static class Physical2TransformExt
    {
        public static bool GetHit(this Collider2D compositeColl, out RaycastHit2D hit, Vector2 direction, float distance = float.PositiveInfinity, string Layer = Constants.Layers.WORLD_GROUND, params string[] extra)
        {
            bool oldSetting = Physics2D.queriesStartInColliders;
            // Force physics engine to ignore the starting collider
            //Physics2D.queriesStartInColliders = false;
            var filter = new ContactFilter2D();
            filter.useLayerMask = true;
            filter.layerMask = LayerMask.GetMask(Layer);
            if (extra.Length > 0)
            {
                filter.layerMask |= LayerMask.GetMask(extra);
            }
            filter.useTriggers = false;
            hit = default;
            RaycastHit2D[] hits = new RaycastHit2D[3];
            int count = compositeColl.Raycast(direction, filter, hits, distance);
            //Physics2D.queriesStartInColliders = oldSetting;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    // Explicitly ignore itself
                    if (hits[i].collider != null && hits[i].collider != compositeColl)
                    {
                        hit = hits[i];
                        if(compositeColl is BoxCollider2D)
                        {
                            return true;
                        }
                        break;
                    }
                }
                Bounds b = compositeColl.bounds;
                Debug.DrawLine(b.center, hit.point, Color.red, 5f);
                // Kiểm tra xem điểm va chạm có nằm "sau" mép ngoài cùng của Collider theo hướng bắn không
                // Nếu điểm va chạm nằm lọt vào bên trong bounds, ta coi đó là lỗi vật lý và bỏ qua
                if (direction == Vector2.up && hit.point.y <= b.max.y) return false;
                if (direction == Vector2.down && hit.point.y >= b.min.y) return false;
                if (direction == Vector2.right && hit.point.x <= b.max.x) return false;
                if (direction == Vector2.left && hit.point.x >= b.min.x) return false;

                // Vẽ để debug trực quan

                return true;
            }

            
            return false;
        }
        public static string GetHitLayer(this Collider2D coll, Vector2 direction, float distance = 1f, string LayerName = Constants.Layers.WORLD_GROUND, params string[] extraLayers)
        {
            var mask = LayerMask.GetMask(LayerName);
            if (extraLayers.Length > 0)
            {
                mask |= LayerMask.GetMask(extraLayers);
            }

            // Sử dụng Center của bounds để Raycast xuống
            RaycastHit2D hit = Physics2D.Raycast(coll.bounds.center, Vector2.down, distance, mask);

            if (hit.collider != null)
            {
                return LayerMask.LayerToName(hit.collider.gameObject.layer);
            }

            return string.Empty; // Không chạm gì cả
        }
        public static T RayCastDownHit<T>(this Collider2D coll, int layerMask, float distance = 10f)
        {
            RaycastHit2D[] hits = new RaycastHit2D[5];
            int hitCount = coll.Raycast(
                Vector2.down,           // hướng
                hits,                   // mảng chứa kết quả
                distance,                    // distance
                layerMask             // LayerMask
            );
            if (hitCount > 0)
            {
                RaycastHit2D hit = hits[0];   // lấy cái gần nhất
                return hit.collider.GetComponent<T>();
            }
            return default(T);
        }
        public static Collider2D RayCastDownHit(this Collider2D coll, int layerMask, float distance = 10f)
        {
            RaycastHit2D[] hits = new RaycastHit2D[5];
            int hitCount = coll.Raycast(
                Vector2.down,           // hướng
                hits,                   // mảng chứa kết quả
                distance,                    // distance
                layerMask             // LayerMask
            );
            if (hitCount > 0)
            {
                RaycastHit2D hit = hits[0];   // lấy cái gần nhất
                return hit.collider;
            }
            return null;
        }
        public static bool IsOnWorldGround(this Collider2D coll, float distance = 1f, string LayerName = Constants.Layers.WORLD_GROUND, params string[] extraLayers)
        {
            var mask = LayerMask.GetMask(LayerName);
            if (extraLayers.Length > 0)
            {
                mask |= LayerMask.GetMask(extraLayers);
            }
            var ret = coll.RayCastDownHit(mask);
            return ret != null;

        }
        public static Collider2D RayCastUp(Collider2D coll, float distance = 1f, string LayerName = Constants.Layers.WORLD_GROUND, params string[] extraLayers)
        {
            var mask = LayerMask.GetMask(LayerName);
            if (extraLayers.Length > 0)
            {
                mask |= LayerMask.GetMask(extraLayers);
            }
            RaycastHit2D[] hits = new RaycastHit2D[5];
            int hitCount = coll.Raycast(
                Vector2.up,           // hướng
                hits,                   // mảng chứa kết quả
                distance,                    // distance
                mask             // LayerMask
            );
            if (hitCount > 0)
            {
                RaycastHit2D hit = hits[0];   // lấy cái gần nhất
                return hit.collider;
            }
            return null;
        }
        public static Collider2D RayCastDown(Collider2D coll, float distance = 1f, string LayerName = Constants.Layers.WORLD_GROUND, params string[] extraLayers)
        {
            var mask = LayerMask.GetMask(LayerName);
            if (extraLayers.Length > 0)
            {
                mask |= LayerMask.GetMask(extraLayers);
            }
            RaycastHit2D[] hits = new RaycastHit2D[5];
            int hitCount = coll.Raycast(
                Vector2.down,           // hướng
                hits,                   // mảng chứa kết quả
                distance,                    // distance
                mask             // LayerMask
            );
            if (hitCount > 0)
            {
                RaycastHit2D hit = hits[0];   // lấy cái gần nhất
                return hit.collider;
            }
            return null;
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
        public static void MoveUpByHeight(this Transform transform, float Height)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Height, transform.position.z);
        }
        /// <summary>
        /// Calculate slop
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="slope"></param>
        /// <param name="duration"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
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


        public static void CalculateSlopDirection(this Transform tr, ref CalculateSlopeDirectionResull result, float direction, float groundCheckDistance = 2f, string LayerName = Constants.Layers.WORLD_GROUND)
        {

            var pos = tr.GetSegment().Center();

            pos.CalculateSlopeDirection(ref result, direction, groundCheckDistance, LayerName);

        }
    }
}
