using System.Collections;
using System.Collections;
using UnityEngine;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;
using unvs.interfaces;
namespace unvs.gameworld.cinema
{


    public class CamObject : MonoBehaviour, ICam
    {
        private BoxCollider2D coll;
        public static CamObject Instance;
        public ICamCenterCamTracking tracker;
        private Rigidbody2D body;

        public BoxCollider2D Coll => coll;

        public ICamCenterCamTracking Tracker => tracker;

        public Rigidbody2D Body => body;

        public void BoundByOrthoGraphicSize(PolygonCollider2D poly)
        {
            if (poly == null) return;
            if (coll == null) coll = Body.GetComponent<BoxCollider2D>();

            // 1. Lấy kích thước hiện tại của cả hai (World Space để so sánh chuẩn)
            Bounds polyBounds = poly.bounds;
            Bounds camBounds = coll.bounds;

            // 2. Kiểm tra nếu Polygon vượt quá giới hạn của Camera Box
            bool isOverWidth = polyBounds.size.x > camBounds.size.x;
            bool isOverHeight = polyBounds.size.y > camBounds.size.y;

            if (isOverWidth || isOverHeight)
            {
                // 3. Tính toán tỉ lệ scale cần thiết để "co" lại
                // Chúng ta lấy tỉ lệ nhỏ nhất để đảm bảo cả 2 chiều đều chui lọt vào cam
                float scaleX = isOverWidth ? (camBounds.size.x / polyBounds.size.x) : 1f;
                float scaleY = isOverHeight ? (camBounds.size.y / polyBounds.size.y) : 1f;

                // Chọn tỉ lệ scale chung (uniform scale) để không làm méo hình dáng poly
                float finalScale = Mathf.Min(scaleX, scaleY);

                // 4. Áp dụng scale cho các điểm của Polygon
                ScalePolygonPoints(poly, finalScale);
            }
        }

        private void ScalePolygonPoints(PolygonCollider2D poly, float scale)
        {
            Vector2 center = poly.bounds.center;
            Vector2 localCenter = poly.transform.InverseTransformPoint(center);

            for (int i = 0; i < poly.pathCount; i++)
            {
                Vector2[] points = poly.GetPath(i);
                for (int j = 0; j < points.Length; j++)
                {
                    // Scale quanh tâm của chính nó
                    points[j] = localCenter + (points[j] - localCenter) * scale;
                }
                poly.SetPath(i, points);
            }
        }


        public void UpdateSizeByOrthoGraphicSize(float size, bool skipSize = false)
        {
            if (coll == null) coll = Body.GetComponent<BoxCollider2D>();

            // Lấy Camera gắn trên cùng GameObject hoặc Camera chính
            Camera cam = GetComponent<Camera>();
            if (cam == null) cam = Camera.main;

            if (cam == null) return;

            // Chiều cao (Vertical) = 2 * orthographicSize
            float height = 2f * size;

            // Chiều rộng (Horizontal) = Chiều cao * Aspect Ratio (Width/Height)
            float width = height * cam.aspect;
            if (!skipSize)
            {
                // Cập nhật kích thước cho BoxCollider2D
                coll.size = new Vector2(width, height);
            }

            //// Đảm bảo offset bằng 0 để Collider nằm chính tâm Camera
            coll.offset = Vector2.zero;
            AlingCeter(coll);
        }

        private void AlingCeter(BoxCollider2D master, float watcherSize = 0.1f)
        {
            if (Application.isPlaying)
            {
                BoxCollider2D coll = this.Tracker.Collider;
                if (coll == null || master == null) return;

                // 1. Lấy vị trí tâm của Master trong không gian World
                Vector3 masterWorldCenter = master.bounds.center;

                // 2. Chuyển vị trí đó về không gian Local của Tracker GameObject
                // Việc này đảm bảo nếu Tracker nằm trên một Object có tọa độ/xoay khác, nó vẫn khớp.
                Vector2 localCenter = coll.transform.InverseTransformPoint(masterWorldCenter);

                // 3. Gán vào Offset
                // Bây giờ tâm của Collider2D sẽ trùng khít với tâm của Master
                coll.offset = localCenter;

                // 4. Đặt kích thước siêu nhỏ cho "mắt thần" (Tracker)
                coll.size = new Vector2(watcherSize, watcherSize);
            }
        }


        private void Awake()
        {
            Instance = this;
            



            //if (Application.isPlaying)
            //{
            //    var go = new GameObject("cam-tracker");
            //    tracker = go.AddComponent<CamTrackingObject>();
            //    go.transform.SetParent(body.transform, false);

            //}
        }

        //private void SetupBody()
        //{
        //    if (body != null) return;
        //    var go = new GameObject("Body");
        //    var b = go.AddComponent<Rigidbody2D>();
        //    b.gravityScale = 0;
        //    b.useFullKinematicContacts = true;
        //    b.sleepMode = RigidbodySleepMode2D.NeverSleep;
        //    b.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        //    go.transform.SetParent(transform, false);
        //    body = b;
        //    b.simulated = true;

        //    coll = go.AddComponent<BoxCollider2D>();
        //    coll.isTrigger = true;
        //}
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Instance == null) return;
            this.coll.GizmosDraw(Color.red, 20);
           
        }




#endif
    }
}