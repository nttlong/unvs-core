using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using unvs.shares;

namespace unvs.ext
{
    public static class EdgeCollider2DExtension
    {
        public static Egde2d[] ToEdge2dArray(this EdgeCollider2D coll)
        {
            if (coll == null || coll.pointCount < 2)
            {
                return new Egde2d[0];
            }

            Vector2[] points = coll.points;
            int edgeCount = points.Length - 1;
            Egde2d[] edges = new Egde2d[edgeCount];

            for (int i = 0; i < edgeCount; i++)
            {
                // Gán điểm đầu và điểm cuối cho từng cạnh
                edges[i] = new Egde2d
                {
                    Start = points[i],
                    End = points[i + 1]
                };
            }

            return edges;
        }
        /// <summary>
        /// chuyen nguoc lai
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static void SetPointsFromEdgeArray(this EdgeCollider2D coll, Egde2d[] edges)
        {
            if (coll == null || edges == null || edges.Length == 0) return;

            // Một EdgeCollider2D nối tiếp cần n điểm cho n-1 cạnh.
            // Chúng ta lấy tất cả điểm Start và điểm End cuối cùng.
            int pointCount = edges.Length + 1;
            Vector2[] newPoints = new Vector2[pointCount];

            for (int i = 0; i < edges.Length; i++)
            {
                newPoints[i] = edges[i].Start;
            }

            // Gán điểm cuối cùng của cạnh cuối cùng
            newPoints[pointCount - 1] = edges[edges.Length - 1].End;

            coll.points = newPoints;
        }
        public static WorldJoinInfo Calculate(this Egde2d[] egds, float x1, float x2, Transform transform)
        {
            WorldJoinInfo info = new WorldJoinInfo();
            info.Poly = egds;
            info.LeftGroundIndex = -1;
            info.RightGroundIndex = -1;

            if (egds == null || egds.Length == 0) return info;

            for (int i = 0; i < egds.Length; i++)
            {
                // CHUYỂN SANG WORLD SPACE TRƯỚC KHI TÍNH TOÁN
                Vector2 p1 = transform.TransformPoint(egds[i].Start);
                Vector2 p2 = transform.TransformPoint(egds[i].End);

                float minX = Mathf.Min(p1.x, p2.x);
                float maxX = Mathf.Max(p1.x, p2.x);

                // Kiểm tra x1 (Điểm nối bên trái)
                if (info.LeftGroundIndex == -1 && x1 >= minX && x1 <= maxX)
                {
                    info.LeftGroundIndex = i;
                    info.LeftPos = CalculateIntersection(p1, p2, x1);
                }

                // Kiểm tra x2 (Điểm nối bên phải)
                if (info.RightGroundIndex == -1 && x2 >= minX && x2 <= maxX)
                {
                    info.RightGroundIndex = i;
                    info.RightPos = CalculateIntersection(p1, p2, x2);
                }
            }

            return info;
        }

        /// <summary>
        /// Trả về index của cạnh bên trái nhất và bên phải nhất.
        /// Tuple: (int leftEdgeIndex, int rightEdgeIndex)
        /// </summary>
        public static (int left, int right) GetExtremeEdgeIndices(this EdgeCollider2D coll)
        {
            Vector2[] points = coll.points;
            int lastPointIndex = points.Length - 1;

            if (points.Length < 2) return (-1, -1);

            // Lấy tọa độ X của điểm đầu và điểm cuối (Local Space)
            float firstX = points[0].x;
            float lastX = points[lastPointIndex].x;

            // Cạnh đầu tiên có index là 0 (nối point 0 và 1)
            // Cạnh cuối cùng có index là lastPointIndex - 1 (nối point n-2 và n-1)
            int firstEdgeIdx = 0;
            int lastEdgeIdx = lastPointIndex - 1;

            // So sánh để xác định hướng vẽ
            if (firstX < lastX)
            {
                // Vẽ thuận: Trái là cạnh đầu, Phải là cạnh cuối
                return (firstEdgeIdx, lastEdgeIdx);
            }
            else
            {
                // Vẽ ngược: Trái là cạnh cuối, Phải là cạnh đầu
                return (lastEdgeIdx, firstEdgeIdx);
            }
        }
        // Scene B (đoạn nối tiếp) - Cắt cạnh PHẢI của bức tường
        public static Vector2 GetIntersectPointWithFirstEdge(this EdgeCollider2D floor, BoxCollider2D wall)
        {
            if (floor.pointCount < 2) return Vector2.zero;

            Vector2 p1 = floor.transform.TransformPoint(floor.points[0]);
            Vector2 p2 = floor.transform.TransformPoint(floor.points[1]);

            // Cắt cạnh PHẢI của bức tường
            float wallX = wall.bounds.max.x;

            return CalculateIntersection(p1, p2, wallX);
        }

        // Scene A (đoạn hiện tại) - Cắt cạnh TRÁI của bức tường
        public static Vector2 GetIntersectPointWithLastEdge(this EdgeCollider2D floor, BoxCollider2D wall)
        {
            int count = floor.pointCount;
            if (count < 2) return Vector2.zero;

            Vector2 p1 = floor.transform.TransformPoint(floor.points[count - 2]);
            Vector2 p2 = floor.transform.TransformPoint(floor.points[count - 1]);

            // Cắt cạnh TRÁI của bức tường
            float wallX = wall.bounds.min.x;

            return CalculateIntersection(p1, p2, wallX);
        }
        public static void TrimEdge(this EdgeCollider2D floor, int indexOfFirstEdge, Vector2 firstPoint, int indexOfLastEdge, Vector2 lastPoint)
        {
            if (floor == null || indexOfFirstEdge == -1 || indexOfLastEdge == -1) return;

            Vector2[] originalPoints = floor.points;
            System.Collections.Generic.List<Vector2> trimmedPoints = new System.Collections.Generic.List<Vector2>();

            // 1. Thêm điểm giao đầu tiên (First Point) làm điểm bắt đầu mới
            trimmedPoints.Add(firstPoint);

            // 2. Thêm các điểm nằm giữa First Edge và Last Edge
            // Cạnh i nối point i và i+1. Vậy các điểm nằm giữa sẽ bắt đầu từ indexOfFirstEdge + 1
            for (int i = indexOfFirstEdge + 1; i <= indexOfLastEdge; i++)
            {
                trimmedPoints.Add(originalPoints[i]);
            }

            // 3. Thêm điểm giao cuối cùng (Last Point) làm điểm kết thúc mới
            trimmedPoints.Add(lastPoint);

            // 4. Cập nhật lại Collider
            floor.points = trimmedPoints.ToArray();
        }
        public static Vector2 TrimLastEdgeAtLeftSideOftWall(this EdgeCollider2D floor, BoxCollider2D wall)
        {
            // 1. Lấy điểm giao (World Space) từ hàm nội suy bạn đã viết
            Vector2 intersectWorld = floor.GetIntersectPointWithLastEdge(wall);

            // Nếu không tìm thấy điểm giao, trả về Vector2.zero
            if (intersectWorld == Vector2.zero) return Vector2.zero;

            // 2. Chuyển điểm giao về Local Space của sàn để cập nhật Collider
            Vector2 intersectLocal = floor.transform.InverseTransformPoint(intersectWorld);

            // 3. Cập nhật mảng điểm của EdgeCollider2D
            Vector2[] points = floor.points;
            int count = points.Length;

            if (count >= 2)
            {
                // Thay thế điểm cuối cùng bằng điểm giao đã local hóa
                points[count - 1] = intersectLocal;
                floor.points = points; // Cập nhật vật lý
            }

            // 4. Trả về điểm giao ở World Space để dùng cho việc nối Scene tiếp theo
            return intersectWorld;
        }

        public static Vector2 TrimFirstEdgeAtRightSideOfWall(this EdgeCollider2D floor, float x)
        {

            // 1. Lấy đoạn thẳng ĐẦU TIÊN của sàn (World Space)
            Vector2[] points = floor.points;
            if (points.Length < 2) return Vector2.zero;

            Vector2 p1 = floor.transform.TransformPoint(points[0]);
            Vector2 p2 = floor.transform.TransformPoint(points[1]);

            // 2. Tìm điểm giao với cạnh PHẢI của bức tường
            float wallRightX = x;

            // Sử dụng lại hàm nội suy CalculateIntersection bạn đã viết
            Vector2 intersectWorld = CalculateIntersection(p1, p2, wallRightX);

            // 3. Chuyển điểm giao về Local Space của sàn mới
            Vector2 intersectLocal = floor.transform.InverseTransformPoint(intersectWorld);

            // 4. Thay thế điểm ĐẦU TIÊN và cập nhật Collider
            points[0] = intersectLocal;
            floor.points = points;

            // 5. Trả về điểm giao World Space để làm mốc nối
            return intersectWorld;
        }

        public static Vector2 TrimFirstEdgeAtRightSideOfWall(this EdgeCollider2D floor, BoxCollider2D wall)
        {

            // 1. Lấy đoạn thẳng ĐẦU TIÊN của sàn (World Space)
            Vector2[] points = floor.points;
            if (points.Length < 2) return Vector2.zero;

            Vector2 p1 = floor.transform.TransformPoint(points[0]);
            Vector2 p2 = floor.transform.TransformPoint(points[1]);

            // 2. Tìm điểm giao với cạnh PHẢI của bức tường
            float wallRightX = wall.bounds.max.x;

            // Sử dụng lại hàm nội suy CalculateIntersection bạn đã viết
            Vector2 intersectWorld = CalculateIntersection(p1, p2, wallRightX);

            // 3. Chuyển điểm giao về Local Space của sàn mới
            Vector2 intersectLocal = floor.transform.InverseTransformPoint(intersectWorld);

            // 4. Thay thế điểm ĐẦU TIÊN và cập nhật Collider
            points[0] = intersectLocal;
            floor.points = points;

            // 5. Trả về điểm giao World Space để làm mốc nối
            return intersectWorld;
        }

        public static (Vector2 min, Vector2 max) CalculateIntersection(this EdgeCollider2D coll, float leftX, float rightX)
        {

            // Giả sử hàm CalculateIntersection(Vector2, Vector2, float) đã được bạn viết ở trên
            var v1 = CalculateIntersection(coll.points[0], coll.points[1], leftX);

            int len = coll.points.Length;
            var v2 = CalculateIntersection(coll.points[len - 2], coll.points[len - 1], rightX);

            // Trả về tuple trực tiếp
            return (v1, v2);
        }

        //public static Vector2 CalculateIntersection(Vector2 p1, Vector2 p2, float targetX)
        //{
        //    // 1. Nếu cạnh p1-p2 là đường thẳng đứng (Song song với targetX)
        //    // Cấp 2 gọi là đường thẳng không có hệ số góc
        //    //if (Mathf.Approximately(p1.x, p2.x))
        //    //{
        //    //    // Trả về targetX và giữ nguyên Y của điểm đầu (hoặc trung điểm)
        //    //    return new Vector2(targetX, p1.y);
        //    //}

        //    //// 2. Phương trình đường thẳng đi qua 2 điểm:
        //    //// (y - y1) / (x - x1) = (y2 - y1) / (x2 - x1)
        //    //// => y = y1 + (targetX - x1) * (y2 - y1) / (x2 - x1)

        //    float tuSo = p2.y - p1.y;
        //    float mauSo = p2.x - p1.x;

        //    float targetY = p1.y + (targetX - p1.x) * (tuSo / mauSo);

        //    return new Vector2(targetX, targetY);
        //}
        public static Vector2 CalculateIntersection(Vector2 p1, Vector2 p2, float targetX)
        {
            if (Mathf.Approximately(p1.x, p2.x)) return new Vector2(targetX, p1.y);

            float t = (targetX - p1.x) / (p2.x - p1.x);

            // Tùy chọn: Nếu bạn muốn đảm bảo Y chỉ nằm trong khoảng của đoạn sàn cũ
            // t = Mathf.Clamp01(t); 

            float targetY = Mathf.Lerp(p1.y, p2.y, t); // Dùng Lerp cho ngắn gọn và an toàn

            return new Vector2(targetX, targetY);
        }
        //private static Vector2 CalculateIntersection(Vector2 p1, Vector2 p2, float targetX)
        //{
        //    if (Mathf.Approximately(p1.x, p2.x)) return new Vector2(targetX, p1.y);

        //    // Nội suy để tìm Y tại vị trí X của cạnh tường
        //    float t = (targetX - p1.x) / (p2.x - p1.x);
        //    float targetY = p1.y + t * (p2.y - p1.y);

        //    return new Vector2(targetX, targetY);
        //}
        /// <summary>
        /// ground neu chua co tao 1 dung duy nhat nam ngang y=0, co roi thi bo qua
        /// </summary>
        /// <param name="ground">Là nền và wall bên trái, wall bên phải</param>
        /// <param name="worldBouding"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void ResizeByPolygonCollider2D(this EdgeCollider2D ground, PolygonCollider2D worldBouding)
        {
            if (ground == null || worldBouding == null) return;

            // Requirement: "ground neu chua co tao 1 dung duy nhat nam ngang y=0, co roi thi bo qua"
            if (ground.pointCount > 0) return;

            Bounds bounds = worldBouding.bounds;

            // Convert world bounds to local space of the ground collider
            Vector2 localMin = ground.transform.InverseTransformPoint(bounds.min);
            Vector2 localMax = ground.transform.InverseTransformPoint(bounds.max);

            float minX = localMin.x;
            float maxX = localMax.x;
            float floorY = 0f;

            // Initialize with two points forming a horizontal floor line
            Vector2[] points = new Vector2[2];
            points[0] = new Vector2(minX, floorY);
            points[1] = new Vector2(maxX, floorY);

            ground.points = points;
        }
        public static void TrimEdge(this EdgeCollider2D floor, int indexOfFirstEdge, float x1, int indexOfLastEdge, float x2)
        {
            if (floor == null || indexOfFirstEdge == -1 || indexOfLastEdge == -1) return;

            Vector2[] originalPoints = floor.points;

            // 1. Tìm điểm giao thực tế tại x1 và x2 (Toán cấp 2 - Extrapolation)
            // Lưu ý: Tính toán trên Local Space của Collider
            Vector2 firstIntersect = CalculateIntersection(originalPoints[indexOfFirstEdge], originalPoints[indexOfFirstEdge + 1], x1);
            Vector2 lastIntersect = CalculateIntersection(originalPoints[indexOfLastEdge], originalPoints[indexOfLastEdge + 1], x2);

            System.Collections.Generic.List<Vector2> trimmedPoints = new System.Collections.Generic.List<Vector2>();

            // 2. Chèn điểm giao đầu tiên
            trimmedPoints.Add(firstIntersect);

            // 3. Giữ lại các điểm gốc nằm giữa hai vị trí cắt (image_74e4c1.png)
            for (int i = indexOfFirstEdge + 1; i <= indexOfLastEdge; i++)
            {
                trimmedPoints.Add(originalPoints[i]);
            }

            // 4. Chèn điểm giao cuối cùng
            trimmedPoints.Add(lastIntersect);

            // 5. Cập nhật lại vào EdgeCollider2D
            floor.points = trimmedPoints.ToArray();


        }
        /// <summary>
        /// This function will cut last edge of ground with right wall
        /// 1- find inter point of last edge with left veritical line of right wall
        /// 2- if found cut ground at that point
        /// 3 - if not add horizontal line from last point of ground to right wall
        /// </summary>
        /// <param name="ground"></param>
        /// <param name="rightWall"></param>
        public static void ClipLastEdgeByX(this EdgeCollider2D ground, float x)
        {
            if (ground.pointCount < 1) return;

            float targetX = x;
            Vector2[] points = ground.points;
            int lastIdx = points.Length - 1;

            Vector2 pLastWorld = ground.transform.TransformPoint(points[lastIdx]);

            if (points.Length >= 2)
            {
                Vector2 pPrevWorld = ground.transform.TransformPoint(points[lastIdx - 1]);

                // 1- find inter point of last edge with right veritical line of right wall
                if ((pPrevWorld.x <= targetX && pLastWorld.x >= targetX) || (pPrevWorld.x >= targetX && pLastWorld.x <= targetX))
                {
                    // 2- if found cut ground at that point
                    Vector2 intersectWorld = CalculateIntersection(pPrevWorld, pLastWorld, targetX);
                    points[lastIdx] = ground.transform.InverseTransformPoint(intersectWorld);
                    ground.points = points;
                    return;
                }
            }

            // 3 - if not add horizontal line from last point of ground to right wall
            if (pLastWorld.x < targetX)
            {
                Vector2 newPointLocal = ground.transform.InverseTransformPoint(new Vector2(targetX, pLastWorld.y));
                Vector2[] newPointsList = new Vector2[points.Length + 1];
                Array.Copy(points, newPointsList, points.Length);
                newPointsList[points.Length] = newPointLocal;
                ground.points = newPointsList;
            }
            else if (pLastWorld.x > targetX)
            {
                // If it's already beyond targetX but didn't cross in the last edge
                points[lastIdx] = ground.transform.InverseTransformPoint(new Vector2(targetX, pLastWorld.y));
                ground.points = points;
            }
        }
        /// <summary>
        /// This function will clip ground by left-wall
        /// 1- find inter point between the first edge of ground and right edge of left-wall
        /// 2- if found clip first edge of ground by that point
        /// 3- else make new horizontal edge from right edge of left wall to first edge of left-wall
        /// </summary>
        /// <param name="ground"></param>
        /// <param name="leftWalll"></param>
        public static void ClipByFirstEdgeByX(this EdgeCollider2D ground, float x)
        {
            if (ground.pointCount < 1) return;

            // targetX is the right edge of the left wall
            float targetX = x;
            Vector2[] points = ground.points;

            Vector2 pFirstWorld = ground.transform.TransformPoint(points[0]);

            if (points.Length >= 2)
            {
                Vector2 pSecondWorld = ground.transform.TransformPoint(points[1]);

                // 1- find inter point between the first edge of ground and right edge of left-wall
                if ((pFirstWorld.x <= targetX && pSecondWorld.x >= targetX) || (pFirstWorld.x >= targetX && pSecondWorld.x <= targetX))
                {
                    // 2- if found clip first edge of ground by that point
                    Vector2 intersectWorld = CalculateIntersection(pFirstWorld, pSecondWorld, targetX);
                    points[0] = ground.transform.InverseTransformPoint(intersectWorld);
                    ground.points = points;
                    return;
                }
            }

            // 3- else make new horizontal edge from right edge of left wall to first edge of left-wall
            if (pFirstWorld.x > targetX)
            {
                Vector2 newPointLocal = ground.transform.InverseTransformPoint(new Vector2(targetX, pFirstWorld.y));
                Vector2[] newPointsList = new Vector2[points.Length + 1];
                newPointsList[0] = newPointLocal;
                Array.Copy(points, 0, newPointsList, 1, points.Length);
                ground.points = newPointsList;
            }
            else if (pFirstWorld.x < targetX)
            {
                // If it's already "inside" the wall area, snap the first point to the wall edge
                points[0] = ground.transform.InverseTransformPoint(new Vector2(targetX, pFirstWorld.y));
                ground.points = points;
            }
        }
        /// <summary>
        /// This function will clip ground with right ground if intersection
        /// 1- find inter point between last edge of ground with first edge of right
        /// 2- if found trim last edge of ground at that point and also trim first edge of right
        /// 3- if not found append new edge to ground. 
        /// The second point of new edge has coordination is firt point of left wall
        /// </summary>
        /// <param name="ground"></param>
        /// <param name="right"></param>
        public static void TrimRightAndJoin(this EdgeCollider2D ground, EdgeCollider2D right)
        {
            if (ground.pointCount < 2 || right.pointCount < 2) return;

            Vector2[] gPoints = ground.points;
            Vector2[] rPoints = right.points;

            // Last edge of ground (world space)
            Vector2 g1 = ground.transform.TransformPoint(gPoints[gPoints.Length - 2]);
            Vector2 g2 = ground.transform.TransformPoint(gPoints[gPoints.Length - 1]);

            // First edge of right (world space)
            Vector2 r1 = right.transform.TransformPoint(rPoints[0]);
            Vector2 r2 = right.transform.TransformPoint(rPoints[1]);

            // Segment-Segment Intersection
            // P = g1, R = g2 - g1
            // Q = r1, S = r2 - r1
            Vector2 R = g2 - g1;
            Vector2 S = r2 - r1;
            float denominator = R.x * S.y - R.y * S.x;

            if (Mathf.Abs(denominator) > 0.0001f)
            {
                float t = ((r1.x - g1.x) * S.y - (r1.y - g1.y) * S.x) / denominator;
                float u = ((r1.x - g1.x) * R.y - (r1.y - g1.y) * R.x) / denominator;

                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    // 2- if found trim last edge of ground at that point and also trim first edge of right
                    Vector2 intersectWorld = g1 + t * R;

                    gPoints[gPoints.Length - 1] = ground.transform.InverseTransformPoint(intersectWorld);
                    ground.points = gPoints;

                    rPoints[0] = right.transform.InverseTransformPoint(intersectWorld);
                    right.points = rPoints;
                    return;
                }
            }

            // 3- if not found append new edge to ground. 
            // The second point of new edge has coordination is firt point of right ground
            Vector2 targetWorld = right.transform.TransformPoint(rPoints[0]);
            Vector2 newPointLocal = ground.transform.InverseTransformPoint(targetWorld);

            Vector2[] newGPoints = new Vector2[gPoints.Length + 1];
            Array.Copy(gPoints, newGPoints, gPoints.Length);
            newGPoints[gPoints.Length] = newPointLocal;
            ground.points = newGPoints;
        }
        /// <summary>
        /// Hàm này ngược lại với TrimRightAndJoin: 
        /// 1- Tìm điểm giao giữa cạnh đầu tiên của floor với cạnh cuối cùng của left.
        /// 2- Nếu thấy: Cắt điểm đầu của floor và điểm cuối của left tại điểm giao đó.
        /// 3- Nếu không thấy: Thêm một điểm mới vào ĐẦU của floor, nối tới điểm cuối của left.
        /// </summary>
        public static void TrimLeftAndJoin(this EdgeCollider2D floor, EdgeCollider2D left)
        {
            if (floor.pointCount < 2 || left.pointCount < 2) return;

            Vector2[] fPoints = floor.points;
            Vector2[] lPoints = left.points;

            // Cạnh đầu tiên của floor (world space)
            Vector2 f1 = floor.transform.TransformPoint(fPoints[0]);
            Vector2 f2 = floor.transform.TransformPoint(fPoints[1]);

            // Cạnh cuối cùng của left (world space)
            Vector2 l1 = left.transform.TransformPoint(lPoints[lPoints.Length - 2]);
            Vector2 l2 = left.transform.TransformPoint(lPoints[lPoints.Length - 1]);

            // Thuật toán giao điểm Segment-Segment
            Vector2 Rf = f2 - f1; // Vector cạnh floor
            Vector2 Rl = l2 - l1; // Vector cạnh left

            float denominator = Rl.x * Rf.y - Rl.y * Rf.x;

            if (Mathf.Abs(denominator) > 0.0001f)
            {
                // t là tỉ lệ trên cạnh left, u là tỉ lệ trên cạnh floor
                float t = ((f1.x - l1.x) * Rf.y - (f1.y - l1.y) * Rf.x) / denominator;
                float u = ((f1.x - l1.x) * Rl.y - (f1.y - l1.y) * Rl.x) / denominator;

                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    // Tìm thấy giao điểm
                    Vector2 intersectWorld = l1 + t * Rl;

                    // Cắt điểm cuối của left
                    lPoints[lPoints.Length - 1] = left.transform.InverseTransformPoint(intersectWorld);
                    left.points = lPoints;

                    // Cắt điểm đầu của floor
                    fPoints[0] = floor.transform.InverseTransformPoint(intersectWorld);
                    floor.points = fPoints;
                    return;
                }
            }

            // Trường hợp KHÔNG giao nhau: Chèn thêm điểm vào ĐẦU mảng points của floor
            Vector2 targetWorld = left.transform.TransformPoint(lPoints[lPoints.Length - 1]);
            Vector2 newPointLocal = floor.transform.InverseTransformPoint(targetWorld);

            Vector2[] newFPoints = new Vector2[fPoints.Length + 1];

            // Điểm mới nằm ở vị trí 0
            newFPoints[0] = newPointLocal;
            // Copy các điểm cũ lùi lại phía sau 1 index
            System.Array.Copy(fPoints, 0, newFPoints, 1, fPoints.Length);

            floor.points = newFPoints;
        }
        public static Vector2 GetIntersetPoint(this EdgeCollider2D floor, float x)
        {
            if (floor == null) return Vector2.zero;

            Vector2[] points = floor.points;

            // 1. Chuyển x từ World Space sang Local Space của floor để tính toán
            Vector3 localPos = floor.transform.InverseTransformPoint(new Vector3(x, 0, 0));
            float localX = localPos.x;

            // 2. Duyệt qua từng cặp điểm để tìm đoạn thẳng chứa x
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[i + 1];

                // Kiểm tra xem localX có nằm trong khoảng giữa 2 điểm p1 và p2 không
                if ((p1.x <= localX && localX <= p2.x) || (p2.x <= localX && localX <= p1.x))
                {
                    // Tránh chia cho 0 nếu đoạn thẳng dựng đứng (x1 == x2)
                    if (Mathf.Approximately(p1.x, p2.x))
                    {
                        // Ưu tiên trả về điểm cao hơn nếu là tường đứng
                        return floor.transform.TransformPoint(p1.y > p2.y ? p1 : p2);
                    }

                    // 3. Nội suy tuyến tính để tìm y tại x (Linear Interpolation)
                    // Công thức: y = y1 + (x - x1) * (y2 - y1) / (x2 - x1)
                    float t = (localX - p1.x) / (p2.x - p1.x);
                    float localY = p1.y + t * (p2.y - p1.y);

                    // 4. Trả về kết quả đã chuyển ngược lại World Space
                    return floor.transform.TransformPoint(new Vector2(localX, localY));
                }
            }

            // Nếu x nằm ngoài phạm vi của Collider, mặc định lấy y của điểm gần nhất
            return Vector2.zero;
        }
        public static Vector2 CalculateSpawnPoint(this EdgeCollider2D floor, Collider2D coll)
        {
            if (floor == null || coll == null) return Vector2.zero;

            // 1. Lấy tọa độ X tại tâm của đối tượng (trong không gian World)
            float targetX = coll.bounds.center.x;

            // 2. Chuyển đổi targetX về không gian Local của EdgeCollider2D
            // Điều này quan trọng nếu sàn nhà bị di chuyển hoặc xoay
            Vector2 localTarget = floor.transform.InverseTransformPoint(new Vector2(targetX, 0));
            float x = localTarget.x;

            // 3. Lấy danh sách các điểm của EdgeCollider2D
            Vector2[] points = floor.points;

            // 4. Duyệt qua từng cặp điểm để tìm đoạn thẳng chứa x
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[i + 1];

                // Kiểm tra xem x có nằm giữa p1.x và p2.x không
                if ((x >= p1.x && x <= p2.x) || (x >= p2.x && x <= p1.x))
                {
                    // Tính toán y bằng nội suy tuyến tính (Linear Interpolation)
                    // Công thức: y = y1 + (x - x1) * (y2 - y1) / (x2 - x1)
                    float t = (x - p1.x) / (p2.x - p1.x);
                    float y = p1.y + t * (p2.y - p1.y);

                    // 5. Chuyển điểm giao diện ngược lại không gian World
                    Vector2 localSpawnPoint = new Vector2(x, y);
                    return floor.transform.TransformPoint(localSpawnPoint);
                }
            }

            // Nếu không tìm thấy (x nằm ngoài phạm vi của sàn), trả về vị trí gốc của sàn
            return floor.transform.position;
        }
    }
}