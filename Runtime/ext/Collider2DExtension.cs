
using System;
using UnityEngine;
namespace unvs.ext
{
    public static class Collider2DExtension
    {
        public static GameObject ScanObject(this Collider2D coll, float ExpandWidth, float ExpandHeight, params string[] Layers)
        {
            return coll.ScanObject(ExpandWidth, ExpandHeight, LayerMask.GetMask(Layers));
        }
        public static GameObject ScanObject(this Collider2D coll, float ExpandWidth, float ExpandHeight, int interactableLayer)
        {

            // 1. Lấy vị trí tâm của nhân vật (hoặc một điểm phía trước mặt nhân vật)
            Vector2 scanPoint = coll.bounds.center;

            var interactSize = coll.bounds.size + new Vector3(ExpandWidth, ExpandHeight, 0);
            // 2. Quét tất cả các Collider2D nằm trong vùng hình hộp và thuộc LayerMask
            Collider2D[] results = Physics2D.OverlapBoxAll(scanPoint, interactSize, 0f, interactableLayer);



            // 3. Xử lý các đối tượng tìm được
            if (results.Length > 0)
            {
                // Thường chúng ta sẽ lấy vật thể gần nhất
                GameObject closestGO = coll.bounds.center.GetClosestTarget(results);
                return closestGO;

            }
            else if (results.Length == 1)
            {
                return results[0].gameObject;
            }
            return null;
        }

        public static void Resize(this BoxCollider2D boxCollider2D, Transform transform)
        {
            // Vì không có SpriteRenderer, chúng ta coi kích thước chuẩn của Collider là 1x1 
            // (tương ứng với khung bao của Transform khi Scale = 1).

            // 1. Set Size về (1, 1) để nó khớp chính xác với Scale của Transform
            boxCollider2D.size = Vector2.one;

            // 2. Set Offset về (0, 0) để nó nằm ngay tâm của Transform
            boxCollider2D.offset = Vector2.zero;

            // Lưu ý: Nếu bạn muốn Collider bao phủ toàn bộ các Object con bên trong 
            // mà không dùng Renderer, bạn phải tính toán dựa trên vị trí của các con.
        }
    }
}
