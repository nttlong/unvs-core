using System;
using System.Collections;
using Unity.Burst;
using Unity.Cinemachine;
using UnityEngine;
using static unvs.ext.TransformExtension;

namespace unvs.ext
{
    public static class BoxCollider2dExtension
    {
        [BurstCompile]
        public static Vector2 GetIntersectPoint(this BoxCollider2D coll, EdgeCollider2D floor)
        {
            // 1. Lấy 4 cạnh của BoxCollider2D (World Space)
            Bounds b = coll.bounds;
            Vector2[] boxEdges = new Vector2[] {
            new Vector2(b.min.x, b.min.y), new Vector2(b.max.x, b.min.y), // Cạnh dưới
            new Vector2(b.max.x, b.min.y), new Vector2(b.max.x, b.max.y), // Cạnh phải
            new Vector2(b.max.x, b.max.y), new Vector2(b.min.x, b.max.y), // Cạnh trên
            new Vector2(b.min.x, b.max.y), new Vector2(b.min.x, b.min.y)  // Cạnh trái
        };

            // 2. Lấy danh sách điểm của EdgeCollider2D (World Space)
            Vector2[] floorPoints = new Vector2[floor.pointCount];
            for (int i = 0; i < floor.pointCount; i++)
            {
                floorPoints[i] = floor.transform.TransformPoint(floor.points[i]);
            }

            // 3. Duyệt tìm giao điểm đầu tiên
            for (int i = 0; i < boxEdges.Length; i += 2)
            {
                Vector2 p1 = boxEdges[i];
                Vector2 p2 = boxEdges[i + 1];

                for (int j = 0; j < floorPoints.Length - 1; j++)
                {
                    Vector2 p3 = floorPoints[j];
                    Vector2 p4 = floorPoints[j + 1];

                    if (LineIntersection(p1, p2, p3, p4, out Vector2 intersect))
                    {
                        return intersect; // Trả về điểm giao đầu tiên tìm thấy
                    }
                }
            }

            return Vector2.zero; // Không có giao điểm
        }

        /// <summary>
        /// Tìm EdgeCollider2D nằm ngay dưới chân của Collider hiện tại trong các Layer chỉ định.
        /// </summary>
        
        public static EdgeCollider2D FindBottomEdge(this Collider2D coll, params string[] layers)
        {
            // 1. Tạo LayerMask từ danh sách tên layer
            int layerMask = LayerMask.GetMask(layers);

            // 2. Xác định điểm bắt đầu (tâm dưới của Bounds) và hướng (xuống dưới)
            // Dùng bounds.center để đảm bảo tia bắt đầu từ giữa nhân vật
            Vector2 rayStart = new Vector2(coll.bounds.center.x, coll.bounds.min.y + 0.1f);
            Vector2 rayDirection = Vector2.down;

            // 3. Bắn một tia Raycast (khoảng cách 1 unit, có thể điều chỉnh tùy độ cao nhân vật)
            float maxDistance = 2.0f;
            RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, maxDistance, layerMask);

            // 4. Kiểm tra kết quả va chạm
            if (hit.collider != null)
            {
                // Kiểm tra xem đối tượng va chạm có phải là EdgeCollider2D không
                EdgeCollider2D edge = hit.collider as EdgeCollider2D;

                if (edge != null)
                {
                    return edge;
                }
            }

            return null; // Không tìm thấy EdgeCollider2D nào ở dưới
        }

        // Hàm phụ tính giao điểm 2 đoạn thẳng (Line Segment Intersection)
       
        private static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersect)
        {
            intersect = Vector2.zero;
            float d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);
            if (d == 0) return false; // Song song

            float u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
            float v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

            if (u < 0 || u > 1 || v < 0 || v > 1) return false;

            intersect.x = p1.x + u * (p2.x - p1.x);
            intersect.y = p1.y + u * (p2.y - p1.y);
            return true;
        }

        /// <summary>
        /// Thiết lập kích thước cho BoxCollider2D một cách nhanh chóng.
        /// </summary>
        /// <param name="coll">Đối tượng Collider cần chỉnh</param>
        /// <param name="width">Chiều rộng (x)</param>
        /// <param name="height">Chiều cao (y)</param>
        public static void SetSize(this BoxCollider2D coll, float width, float height)
        {
            if (coll == null)
            {
                Debug.LogError("SetSize: BoxCollider2D is null!");
                return;
            }

            // Gán trực tiếp Vector2 vào size
            coll.size = new Vector2(width, height);
        }
        public static void SetPositionAndSize(this BoxCollider2D coll, Vector2 pos, float width, float height)
        {
            if (coll == null)
            {
                Debug.LogError("SetPositionAndSize: BoxCollider2D is null!");
                return;
            }

            // 1. Thiết lập kích thước
            coll.size = new Vector2(width, height);

            // 2. Thiết lập vị trí (Offset)
            // Nếu 'pos' là vị trí tương đối so với Transform:
            coll.offset = pos;

            // Nếu 'pos' bạn truyền vào là World Position, bạn phải chuyển nó về Local:
            // coll.offset = coll.transform.InverseTransformPoint(pos);
        }
        public static void SetSizeWorld(this SpriteRenderer renderer, float targetWidth, float targetHeight)
        {
            if (renderer == null || renderer.sprite == null) return;

            // Kích thước thực tế của Sprite trong không gian World (chưa tính Scale)
            float worldWidth = renderer.sprite.bounds.size.x;
            float worldHeight = renderer.sprite.bounds.size.y;

            // Tính toán tỷ lệ Scale cần thiết
            float newScaleX = targetWidth / worldWidth;
            float newScaleY = targetHeight / worldHeight;

            renderer.transform.localScale = new Vector3(newScaleX, newScaleY, 1f);
        }
        public static void SyncColliderSize(this BoxCollider2D coll, SpriteRenderer renderer)
        {
            if (coll == null || renderer == null || renderer.sprite == null) return;

            // Lấy kích thước của Sprite (đã tính đến Pixels Per Unit - PPU)
            // renderer.sprite.bounds.size trả về kích thước gốc của sprite trong World Space
            Vector2 spriteSize = renderer.sprite.bounds.size;

            // Gán size cho Collider (Lưu ý: size của Collider là Local)
            // Ta phải chia cho scale của chính nó để bù trừ nếu transform có scale != 1
            Vector3 localScale = coll.transform.localScale;
            coll.size = new Vector2(spriteSize.x / localScale.x, spriteSize.y / localScale.y);

            // Nếu bạn muốn tâm của Collider khớp với tâm của Sprite
            coll.offset = renderer.sprite.bounds.center - coll.transform.position;
        }
        /// <summary>
        /// Ép SpriteRenderer phải co giãn vừa khít với kích thước của BoxCollider2D.
        /// Chỉ nên dùng trong Editor Mode.
        /// </summary>
        public static void SyncSpriteToCollider(this BoxCollider2D coll, SpriteRenderer renderer)
        {
            if (coll == null || renderer == null || renderer.sprite == null) return;

            // 1. Lấy kích thước thực tế của Sprite gốc (World Units - chưa tính scale)
            // Sprite.bounds.size cho biết tấm ảnh này to bao nhiêu unit nếu scale = 1
            Vector2 spriteOriginalSize = renderer.sprite.bounds.size;

            // 2. Lấy kích thước mong muốn từ BoxCollider2D (Local Size)
            Vector2 targetSize = coll.size;

            // 3. Tính toán LocalScale mới cho Transform
            // Tỷ lệ = Kích thước Collider / Kích thước gốc của Sprite
            float newScaleX = targetSize.x / spriteOriginalSize.x;
            float newScaleY = targetSize.y / spriteOriginalSize.y;

            renderer.transform.localScale = new Vector3(newScaleX, newScaleY, 1f);

            // 4. Đồng bộ vị trí (Offset) nếu cần
            // Nếu Pivot của Sprite không nằm ở giữa, bạn cần chỉnh thêm localPosition
            renderer.transform.localPosition = coll.offset;

            Debug.Log($"[Editor] Resized Sprite to match Collider: {targetSize}");
        }
        public static void FitSpriteToBoxCollider(this SpriteRenderer renderer, BoxCollider2D coll)
        {
            if (renderer == null || coll == null || renderer.sprite == null) return;

            // 1. Lấy kích thước thực tế của cái ảnh (tính bằng Unit trong World)
            // Ví dụ: Ảnh 100px, PPU 100 => Size = 1 Unit.
            Vector2 spriteUnitSize = renderer.sprite.rect.size / renderer.sprite.pixelsPerUnit;

            // 2. Lấy kích thước bạn đã kéo trong BoxCollider2D (Local Size)
            Vector2 targetSize = coll.size;

            // 3. Tính toán tỷ lệ Scale cần thiết để "ép" ảnh giãn ra
            float newScaleX = targetSize.x / spriteUnitSize.x;
            float newScaleY = targetSize.y / spriteUnitSize.y;

            // 4. Áp vào Scale của Transform
            renderer.transform.localScale = new Vector3(newScaleX, newScaleY, 1f);

            // 5. Căn chỉnh tâm (Offset)
            renderer.transform.localPosition = coll.offset;
        }
        public static void ForceFitSpriteToCollider(this SpriteRenderer renderer, BoxCollider2D coll)
        {
            if (renderer == null || coll == null || renderer.sprite == null) return;

            // 1. Tính toán kích thước chuẩn của Sprite Asset (đơn vị World Unit)
            // sprite.bounds.size trả về kích thước dựa trên PPU và Rect gốc
            Vector2 spriteOriginalSize = renderer.sprite.bounds.size;

            // 2. Tính toán tỷ lệ Scale cần thiết dựa trên Size của Collider
            // Chúng ta dùng coll.size (Local) vì ta đang chỉnh Scale của chính nó
            float newScaleX = coll.size.x / spriteOriginalSize.x;
            float newScaleY = coll.size.y / spriteOriginalSize.y;

            renderer.transform.localScale = new Vector3(newScaleX, newScaleY, 1f);

            // 3. Xử lý phần lệch tâm (Quan trọng nhất)
            // BoxCollider2D có thuộc tính offset, nếu offset khác (0,0) nó sẽ bị lệch như hình của bạn
            // Ta dịch chuyển Local Position của Transform để bù trừ vào cái Offset đó
            renderer.transform.localPosition = (Vector3)coll.offset;

            // Nếu Sprite và Collider nằm trên 2 Object khác nhau (cha-con), 
            // bạn cần đảm bảo tọa độ được tính toán trong cùng một Space.
        }
        public static void FitSpriteToColliderCorrect(this SpriteRenderer renderer, BoxCollider2D coll)
        {
            if (renderer == null || coll == null || renderer.sprite == null) return;

            // 1. Lấy size thực tế của Sprite gốc (World Units)
            Vector2 spriteUnitSize = renderer.sprite.rect.size / renderer.sprite.pixelsPerUnit;

            // 2. Tính Scale dựa trên Size của Collider
            // Chỉ chỉnh Scale, KHÔNG chỉnh Position của chính nó nếu chung 1 Object
            float newScaleX = coll.size.x / spriteUnitSize.x;
            float newScaleY = coll.size.y / spriteUnitSize.y;

            renderer.transform.localScale = new Vector3(newScaleX, newScaleY, 1f);

            // 3. Xử lý Offset: Chỉ chỉnh Offset của Collider về 0 để khớp với Tâm Sprite
            // Thay vì bắt Sprite chạy theo Collider, hãy bắt Collider nằm giữa Sprite
            coll.offset = Vector2.zero;

            Debug.Log("Success: Sprite and Collider are now perfectly aligned at Center.");
        }
        public static void FitSpriteToBoxCollider2D_WorldCorrect(this SpriteRenderer renderer, BoxCollider2D coll)
        {
            if (renderer == null || coll == null || renderer.sprite == null) return;

            Vector2 spriteLocalSize = renderer.sprite.bounds.size;

            // Sprite size phải hợp lệ
            if (spriteLocalSize.x <= 0.00001f || spriteLocalSize.y <= 0.00001f)
            {
                Debug.LogError($"Sprite bounds size invalid: {spriteLocalSize}");
                return;
            }

            Vector2 colliderLocalSize = coll.size;

            // Collider size phải hợp lệ
            if (colliderLocalSize.x <= 0.00001f || colliderLocalSize.y <= 0.00001f)
            {
                Debug.LogError($"Collider size invalid: {colliderLocalSize}");
                return;
            }

            Vector3 spriteLossy = renderer.transform.lossyScale;
            Vector3 colliderLossy = coll.transform.lossyScale;

            // Nếu scale cha bị 0 -> không thể fit
            if (Mathf.Abs(spriteLossy.x) <= 0.00001f || Mathf.Abs(spriteLossy.y) <= 0.00001f)
            {
                Debug.LogError($"Renderer lossyScale invalid: {spriteLossy}");
                return;
            }

            if (Mathf.Abs(colliderLossy.x) <= 0.00001f || Mathf.Abs(colliderLossy.y) <= 0.00001f)
            {
                Debug.LogError($"Collider lossyScale invalid: {colliderLossy}");
                return;
            }

            Vector2 colliderWorldSize = new Vector2(
                colliderLocalSize.x * Mathf.Abs(colliderLossy.x),
                colliderLocalSize.y * Mathf.Abs(colliderLossy.y)
            );

            Vector2 spriteWorldSizeCurrent = new Vector2(
                spriteLocalSize.x * Mathf.Abs(spriteLossy.x),
                spriteLocalSize.y * Mathf.Abs(spriteLossy.y)
            );

            if (spriteWorldSizeCurrent.x <= 0.00001f || spriteWorldSizeCurrent.y <= 0.00001f)
            {
                Debug.LogError($"spriteWorldSizeCurrent invalid: {spriteWorldSizeCurrent}");
                return;
            }

            float ratioX = colliderWorldSize.x / spriteWorldSizeCurrent.x;
            float ratioY = colliderWorldSize.y / spriteWorldSizeCurrent.y;

            // clamp tránh scale điên
            ratioX = Mathf.Clamp(ratioX, 0.0001f, 10000f);
            ratioY = Mathf.Clamp(ratioY, 0.0001f, 10000f);

            Vector3 localScale = renderer.transform.localScale;
            localScale.x *= ratioX;
            localScale.y *= ratioY;
            renderer.transform.localScale = localScale;

            // Align offset theo pivot thật
            Vector2 spriteLocalCenter = renderer.sprite.bounds.center;

            if (renderer.transform == coll.transform)
            {
                coll.offset = spriteLocalCenter;
            }
            else
            {
                Vector3 spriteCenterWorld = renderer.transform.TransformPoint(spriteLocalCenter);
                Vector3 spriteCenterLocalToCollider = coll.transform.InverseTransformPoint(spriteCenterWorld);
                coll.offset = (Vector2)spriteCenterLocalToCollider;
            }
        }
        public static void UpdateSizeByLensSettings(this BoxCollider2D box, LensSettings lens)
        {
            //if (cam == null || box == null) return;
            //if (!cam.orthographic)
            //{
            //    Debug.LogError("Camera must be Orthographic");
            //    return;
            //}

            float height = lens.OrthographicSize * 2f;
            float width = height * lens.Aspect;
            float depth = lens.FarClipPlane - lens.NearClipPlane;

            box.size = new Vector3(width, height, depth);

            // Đưa collider nằm đúng vùng nhìn của camera (center theo camera)
            box.offset = new Vector3(0f, 0f, lens.NearClipPlane + depth * 0.5f);
        }
      
        public static void ResizeByTransform(this BoxCollider2D coll)
        {
            if (coll == null) return;

            // Lấy Segment từ Transform (giả sử bạn đã có extension GetSegment())
            Segment segment = coll.transform.GetSegment();

            // Tạo 4 điểm của rectangle (width bạn có thể truyền vào sau)
            Vector2[] points = segment.CreateRect(width: 1f); // thay 1f bằng độ dày mong muốn

            // Tính toán để BoxCollider2D khớp chính xác
            Vector2 center = (segment.Start + segment.End) * 0.5f;
            Vector2 direction = (segment.End - segment.Start).normalized;
            float length = segment.Length;

            // Tính size của BoxCollider2D
            Vector2 size = new Vector2(length, 1f); // width sẽ được set sau

            // Chuyển points về local space để tính chính xác
            //Vector2 localCenter = coll.transform.InverseTransformPoint(center);

            // Tính size theo local space
            float localLength = Vector2.Distance(
                coll.transform.InverseTransformPoint(segment.Start),
                coll.transform.InverseTransformPoint(segment.End));

            // Resize collider
            coll.offset =Vector2.zero;
            coll.size = new Vector2(localLength, 1f); // tạm thời

            // Nếu bạn muốn set width động:
            // coll.size = new Vector2(localLength, desiredWidth);
        }
        public static void FixBySpriteSize(this BoxCollider2D coll, SpriteRenderer sp)
        {
            float width = sp.sprite.rect.width / sp.sprite.pixelsPerUnit;
            float height = sp.sprite.rect.height / sp.sprite.pixelsPerUnit;
            coll.size = new Vector2(width, height);// sp.transform.localScale;
        }
        public static void FixBySpriteSize(this BoxCollider2D coll , Sprite sp)
        {
            float width = sp.rect.width / sp.pixelsPerUnit;
            float height = sp.rect.height / sp.pixelsPerUnit;
            coll.size = new Vector2(width, height);// sp.transform.localScale;
        }
    }
}