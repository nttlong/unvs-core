#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

using UnityEngine;
using unvs.interfaces;
using unvs.shares;
using System.Linq;
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
        public static T ScanObject<T>(this Collider2D coll, float ExpandWidth, float ExpandHeight, int interactableLayer)
        {

            // 1. Lấy vị trí tâm của nhân vật (hoặc một điểm phía trước mặt nhân vật)
            Vector2 scanPoint = coll.bounds.center;

            var interactSize = coll.bounds.size + new Vector3(ExpandWidth, ExpandHeight, 0);
            // 2. Quét tất cả các Collider2D nằm trong vùng hình hộp và thuộc LayerMask
            Collider2D[] results = Physics2D.OverlapBoxAll(scanPoint, interactSize, 0f, interactableLayer);
            var colls= results.Where(p=>p.GetComponent<T>()!=null).ToArray();


            // 3. Xử lý các đối tượng tìm được
            if (colls.Length > 0)
            {
                // Thường chúng ta sẽ lấy vật thể gần nhất
                GameObject closestGO = coll.bounds.center.GetClosestTarget(colls);
                return closestGO.GetComponent<T>();

            }
            else if (results.Length == 1)
            {
                return results[0].GetComponent<T>();
            }
            return default(T);
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

       
#if UNITY_EDITOR
        public static void GizmosDrawCamView(Vector3 Center, float orthographicSize, OffsetFollow cameraOffsetFolow, Color color, float thickness)
        {
            // 1. Tính toán kích thước Camera trong World Space
            Vector2 camSize = Commons.GetCameraWorldSizeEditorMode(orthographicSize);
            Debug.Log($"GizmosDrawCamView={camSize}");
            // 2. Tính toán vị trí tâm (Center) dựa trên Follow Offset
            // Lưu ý: Offset này thường dựa trên vị trí của nhân vật/object
            Vector3 offset = cameraOffsetFolow.CalculateOffset(orthographicSize);
            Vector3 center = Center + offset;

            // 3. Thiết lập màu sắc
            Gizmos.color = color;
            Handles.color = color;

            // 4. Vẽ hình chữ nhật
            // Nếu thickness <= 1, dùng DrawWireCube cho nhẹ
            if (thickness <= 1f)
            {
                Gizmos.DrawWireCube(center, new Vector3(camSize.x, camSize.y, 0.1f));
            }
            else
            {
                // Vẽ 4 cạnh bằng Handles để có độ dày (Chỉ hoạt động trong Editor)
                Vector3 topLeft = center + new Vector3(-camSize.x / 2, camSize.y / 2, 0);
                Vector3 topRight = center + new Vector3(camSize.x / 2, camSize.y / 2, 0);
                Vector3 bottomLeft = center + new Vector3(-camSize.x / 2, -camSize.y / 2, 0);
                Vector3 bottomRight = center + new Vector3(camSize.x / 2, -camSize.y / 2, 0);

                Handles.DrawBezier(topLeft, topRight, topLeft, topRight, color, null, thickness);
                Handles.DrawBezier(topRight, bottomRight, topRight, bottomRight, color, null, thickness);
                Handles.DrawBezier(bottomRight, bottomLeft, bottomRight, bottomLeft, color, null, thickness);
                Handles.DrawBezier(bottomLeft, topLeft, bottomLeft, topLeft, color, null, thickness);
            }
        }
        
#endif
    }

}
