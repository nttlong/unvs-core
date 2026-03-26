using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;
using unvs.shares;

namespace unvs.ext
{
    public static class PolygonCollider2Extension
    {
        /// <summary>
        /// This function will alig left wall and right wall where limit actor moving
        /// </summary>
        /// <param name="polygonCollider2D"></param>
        /// <param name="leftWall"></param>
        /// <param name="rightWall"></param>
        public static void AlignWall(this PolygonCollider2D polygonCollider2D, BoxCollider2D left, BoxCollider2D right, bool inner = false)
        {

            if (polygonCollider2D == null)
            {
                // SLog.Info($"AlignWall(polygonCollider2D={polygonCollider2D}");
                return;
            }


            Bounds bounds = polygonCollider2D.bounds;
            float centerY = bounds.center.y;
            float height = bounds.size.y;

            float width = left.size.x;
            float offsetX = left.offset.x;

            // Update height
            left.size = new Vector2(width, height);

            // Align RIGHT edge of left wall to bounds.min.x
            // worldRightEdge = transform.position.x + offsetX + (width / 2)
            var dx = inner ? -(width / 2f) : (width / 2f);
            float targetX = bounds.min.x - offsetX - dx;
            left.transform.position = new Vector3(targetX, centerY, left.transform.position.z);

            width = right.size.x;
            offsetX = right.offset.x;

            // Update height
            right.size = new Vector2(width, height);

            // Align LEFT edge of right wall to bounds.max.x
            // worldLeftEdge = transform.position.x + offsetX - (width / 2)
            targetX = bounds.max.x - offsetX + (inner ? -(width / 2f) : (width / 2f));
            right.transform.position = new Vector3(targetX, centerY, right.transform.position.z);
        }

        public static void FitToGrid(this PolygonCollider2D polygonCollider, int width = 7, int height = 4, float yOffsetRatio = 0.25f)
        {
            // 1. Tính toán kích thước thực tế dựa trên Grid
            float actualWidth = width * 10f;
            float actualHeight = height * 10f;

            float halfWidth = actualWidth / 2f;
            float halfHeight = actualHeight / 2f;

            // 2. Tính toán khoảng cách cần dời
            // Nếu bạn muốn tâm nằm ở 1/4 từ dưới lên, ta cần đẩy các điểm LÊN TRÊN một khoảng.
            // Khoảng cách từ tâm (giữa hình) đến đáy là halfHeight (0.5 chiều cao).
            // Bạn muốn tâm cách đáy 0.25 chiều cao -> Ta cần dịch hình lên 0.25 chiều cao nữa.
            float yOffset = actualHeight * yOffsetRatio;

            // 3. Định nghĩa 4 điểm và CỘNG yOffset để đẩy toàn bộ Collider lên trên so với tâm Pivot
            Vector2[] points = new Vector2[4]
            {
        new Vector2(-halfWidth,  halfHeight + yOffset), // Trên trái
        new Vector2( halfWidth,  halfHeight + yOffset), // Trên phải
        new Vector2( halfWidth, -halfHeight + yOffset), // Dưới phải
        new Vector2(-halfWidth, -halfHeight + yOffset)  // Dưới trái
            };

            // 4. Gán lại vào Collider
            polygonCollider.points = points;

            Debug.Log($"FitToGrid: Size {width}x{height} đã dời hình lên để tâm nằm ở 1/4 phía dưới.");
        }
        /// <summary>
        /// find the edge in world-boud whic is the most vertical and value X is biggest
        /// Trick abs(x2-x1) is smallest that mean is the most vertical
        /// 
        /// Align (90 degree) that edge to the right edge of right-wall +deltaX
        /// Exmaple: the X of right edge of right-wall is 100, and deltaX is 100 
        /// the first point and the second point of that edage are X+deltaX=200
        /// Note: the right-wall is ready vertical (90 degree )
        /// </summary>
        /// <param name="worldBound"></param>
        /// <param name="right"></param>
        /// <param name="deltaX"></param>
        public static void AlignEdgeToRightWall(this PolygonCollider2D worldBound, BoxCollider2D right, float deltaX = 20)
        {
            if (worldBound == null || right == null) return;

            float targetX = right.bounds.max.x + deltaX;
            Vector2[] points = worldBound.points;
            int bestEdgeStart = -1;
            float bestScore = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                int next = (i + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[i] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                float avgX = (p1.x + p2.x) / 2f;
                float absDX = Mathf.Abs(p2.x - p1.x);

                // Heuristic Score: favors higher X coordinates and strictly vertical edges (small absDX)
                // Weighting absDX heavily (100x) to prioritize verticality over X-position
                float score = avgX - (absDX * 100f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestEdgeStart = i;
                }
            }

            if (bestEdgeStart != -1) // no, we are sure that we found the most vertical edge and biggest X
            {
                int next = (bestEdgeStart + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[bestEdgeStart] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                // Both points of the selected edge are aligned to targetX
                points[bestEdgeStart] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p1.y)) - worldBound.offset;
                points[next] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p2.y)) - worldBound.offset;
                worldBound.points = points;
            }
        }
        public static void AlignEdgeToRightWall(this PolygonCollider2D worldBound, Transform pos, float deltaX = 20)
        {
            if (worldBound == null || pos == null) return;

            float targetX = pos.transform.position.x + deltaX;
            Vector2[] points = worldBound.points;
            int bestEdgeStart = -1;
            float bestScore = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                int next = (i + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[i] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                float avgX = (p1.x + p2.x) / 2f;
                float absDX = Mathf.Abs(p2.x - p1.x);

                // Heuristic Score: favors higher X coordinates and strictly vertical edges (small absDX)
                // Weighting absDX heavily (100x) to prioritize verticality over X-position
                float score = avgX - (absDX * 100f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestEdgeStart = i;
                }
            }

            if (bestEdgeStart != -1) // no, we are sure that we found the most vertical edge and biggest X
            {
                int next = (bestEdgeStart + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[bestEdgeStart] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                // Both points of the selected edge are aligned to targetX
                points[bestEdgeStart] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p1.y)) - worldBound.offset;
                points[next] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p2.y)) - worldBound.offset;
                worldBound.points = points;
            }
        }
        /// <summary>
        /// find the edge in world-boud whic is the most vertical and value X is smallest
        /// Trick abs(x2-x1) is smallest that mean is the most vertical
        /// 
        /// Align (90 degree) that edge to the left edge of left-wall -deltaX
        /// Example: the X of left edge of left-wall is 100, and deltaX is 100 
        /// the first point and the second point of that edage are X-deltaX=0
        /// Note: the left-wall is ready vertical (90 degree )
        /// </summary>
        /// <param name="worldBound"></param>
        /// <param name="left"></param>
        /// <param name="deltaX"></param>
        public static void AlignEdgeToLeftWall(this PolygonCollider2D worldBound, BoxCollider2D left, float deltaX = 20)
        {
            if (worldBound == null || left == null) return;

            float targetX = left.bounds.min.x - deltaX;
            Vector2[] points = worldBound.points;
            int bestEdgeStart = -1;
            float bestScore = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                int next = (i + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[i] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                float avgX = (p1.x + p2.x) / 2f;
                float absDX = Mathf.Abs(p2.x - p1.x);

                // Heuristic Score: favors smaller X coordinates and strictly vertical edges (small absDX)
                // Score = -avgX (to favor smaller X) - (absDX * 100f)
                float score = -avgX - (absDX * 100f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestEdgeStart = i;
                }
            }

            if (bestEdgeStart != -1)
            {
                int next = (bestEdgeStart + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[bestEdgeStart] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                // Both points of the selected edge are aligned to targetX
                points[bestEdgeStart] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p1.y)) - worldBound.offset;
                points[next] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p2.y)) - worldBound.offset;
                worldBound.points = points;
            }
        }
        public static void AlignEdgeToLeftWall(this PolygonCollider2D worldBound, Transform left, float deltaX = 20)
        {
            if (worldBound == null || left == null) return;

            float targetX = left.transform.position.x - deltaX;
            Vector2[] points = worldBound.points;
            int bestEdgeStart = -1;
            float bestScore = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                int next = (i + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[i] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                float avgX = (p1.x + p2.x) / 2f;
                float absDX = Mathf.Abs(p2.x - p1.x);

                // Heuristic Score: favors smaller X coordinates and strictly vertical edges (small absDX)
                // Score = -avgX (to favor smaller X) - (absDX * 100f)
                float score = -avgX - (absDX * 100f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestEdgeStart = i;
                }
            }

            if (bestEdgeStart != -1)
            {
                int next = (bestEdgeStart + 1) % points.Length;
                Vector2 p1 = worldBound.transform.TransformPoint(points[bestEdgeStart] + worldBound.offset);
                Vector2 p2 = worldBound.transform.TransformPoint(points[next] + worldBound.offset);

                // Both points of the selected edge are aligned to targetX
                points[bestEdgeStart] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p1.y)) - worldBound.offset;
                points[next] = (Vector2)worldBound.transform.InverseTransformPoint(new Vector2(targetX, p2.y)) - worldBound.offset;
                worldBound.points = points;
            }
        }

        public static PolygonCollider2D CreateRectCollider2D(this PolygonCollider2D source, string name, float margin = 0.1f)
        {
           
            // 1. Tạo GameObject con
            GameObject trackerGo = new GameObject(name);
            trackerGo.transform.SetParent(source.transform, false);

            // Đảm bảo transform của tracker trùng khít với cha để tọa độ local khớp nhau
            trackerGo.transform.localPosition = Vector3.zero;
            trackerGo.transform.localRotation = Quaternion.identity;
            trackerGo.transform.localScale = Vector3.one;

            PolygonCollider2D newColl = trackerGo.AddComponent<PolygonCollider2D>();
            newColl.isTrigger = true;

            // 2. Lấy Bounds của source
            // QUAN TRỌNG: Phải lấy bounds trong Local Space của source
            Bounds b = source.bounds;

            // Chuyển đổi cực trị của Bounds từ World về Local của chính nó
            Vector2 minLocal = source.transform.InverseTransformPoint(b.min);
            Vector2 maxLocal = source.transform.InverseTransformPoint(b.max);

            // 3. Tính toán 4 góc với Margin
            float left = minLocal.x + margin;
            float right = maxLocal.x - margin;
            float bottom = minLocal.y + margin;
            float top = maxLocal.y - margin;

            // 4. Thiết lập 4 điểm (Clockwise)
            Vector2[] rectPoints = new Vector2[]
            {
        new Vector2(left, top),
        new Vector2(right, top),
        new Vector2(right, bottom),
        new Vector2(left, bottom)
            };

            newColl.pathCount = 1;
            newColl.SetPath(0, rectPoints);

            return newColl;
        }
        public static PolygonCollider2D Clone(this PolygonCollider2D source, string name,bool defaultActive=true )
        {
            // 1. Tạo GameObject mới làm con của Source để thừa hưởng Transform (vị trí, góc quay, scale)
            GameObject trackerGo = new GameObject(name);
            trackerGo.SetActive(defaultActive);
            trackerGo.transform.SetParent(source.transform, false);

            // 2. Thêm PolygonCollider2D mới
            PolygonCollider2D newColl = trackerGo.AddComponent<PolygonCollider2D>();

            // 3. Sao chép dữ liệu hình dạng (Paths)
            newColl.pathCount = source.pathCount;
            for (int i = 0; i < source.pathCount; i++)
            {
                newColl.SetPath(i, source.GetPath(i));
            }

            // 4. Thiết lập để chỉ dùng để nhận diện (Trigger)
            newColl.isTrigger = true;
            newColl.offset = source.offset;

            // 5. Quan trọng: Đảm bảo Composite Operation là None để nó KHÔNG bị gộp vào Global Bound
            newColl.compositeOperation = Collider2D.CompositeOperation.None;

            return newColl;
        }
        public static PolygonCollider2D CloneWithMargin(this PolygonCollider2D source, float margin = 0.1f, string name = "SceneTracker")
        {
            // 1. Khởi tạo GameObject và Component
            GameObject trackerGo = new GameObject(name);
            trackerGo.transform.SetParent(source.transform, false);

            PolygonCollider2D newColl = trackerGo.AddComponent<PolygonCollider2D>();
            newColl.isTrigger = true;
            newColl.offset = source.offset;

            // 2. Duyệt qua các Path và Points
            newColl.pathCount = source.pathCount;
            for (int i = 0; i < source.pathCount; i++)
            {
                Vector2[] pathPoints = source.GetPath(i);

                for (int j = 0; j < pathPoints.Length; j++)
                {
                    // Thuật toán thụt lề: 
                    // Nếu x dương -> trừ margin. Nếu x âm -> cộng margin.
                    // Tương tự với y. Điều này ép các điểm co lại vào tâm.

                    float moveX = (pathPoints[j].x > 0) ? -margin : (pathPoints[j].x < 0 ? margin : 0);
                    float moveY = (pathPoints[j].y > 0) ? -margin : (pathPoints[j].y < 0 ? margin : 0);

                    pathPoints[j].x += moveX;
                    pathPoints[j].y += moveY;
                }

                newColl.SetPath(i, pathPoints);
            }

            return newColl;
        }
        public static PolygonCollider2D CloneAndScale(this PolygonCollider2D source, float scaleX = 0.95f, float scaleY = 1.0f, string name = "SceneTracker")
        {
            // 1. Sử dụng lại logic Clone cơ bản mà bạn đã có
            GameObject trackerGo = new GameObject(name);
            trackerGo.transform.SetParent(source.transform, false);

            PolygonCollider2D newColl = trackerGo.AddComponent<PolygonCollider2D>();
            newColl.isTrigger = true;
            newColl.offset = source.offset;
            newColl.compositeOperation = Collider2D.CompositeOperation.None;

            // 2. Sao chép và thu nhỏ dữ liệu hình dạng (Paths)
            newColl.pathCount = source.pathCount;
            for (int i = 0; i < source.pathCount; i++)
            {
                Vector2[] pathPoints = source.GetPath(i);

                // Tính toán thu nhỏ từng điểm trong Path
                for (int j = 0; j < pathPoints.Length; j++)
                {
                    // Thu nhỏ tọa độ điểm so với tâm của Collider (local space)
                    pathPoints[j].x *= scaleX;
                    pathPoints[j].y *= scaleY;
                }

                newColl.SetPath(i, pathPoints);
            }

            return newColl;
        }
        public static void MeasureDistanceLeftRight(
            this BoxCollider2D centerCollider,
            BoxCollider2D left,
            BoxCollider2D right,
            out float leftDistance,
            out float rightDistance)
        {
            leftDistance = float.MaxValue;
            rightDistance = float.MaxValue;

            if (centerCollider == null) return;

            // Lấy tâm thế giới của Box trung tâm
            Vector2 centerPoint = centerCollider.bounds.center;

            // Tính khoảng cách tới tâm Box bên trái
            if (left != null)
            {
                leftDistance = Vector2.Distance(centerPoint, left.bounds.center);
            }

            // Tính khoảng cách tới tâm Box bên phải
            if (right != null)
            {
                rightDistance = Vector2.Distance(centerPoint, right.bounds.center);
            }
        }
        public static void MeasureDistanceLeftRight(
            this BoxCollider2D centerCollider,
            PolygonCollider2D left,
            PolygonCollider2D right,
            out float leftDistance,
            out float rightDistance)
        {
            // Khởi tạo giá trị mặc định nếu collider bị null
            leftDistance = float.MaxValue;
            rightDistance = float.MaxValue;

            if (centerCollider == null) return;

            // Lấy tâm của BoxCollider trung tâm
            Vector2 centerPoint = centerCollider.bounds.center;

            // Tính khoảng cách tới tâm của Collider bên trái
            if (left != null)
            {
                leftDistance = Vector2.Distance(centerPoint, left.bounds.center);
            }

            // Tính khoảng cách tới tâm của Collider bên phải
            if (right != null)
            {
                rightDistance = Vector2.Distance(centerPoint, right.bounds.center);
            }
        }
        public static PolygonCollider2D CloneAndScaleMaintainPivot(this PolygonCollider2D source, float scaleX, float scaleY, string name = "SceneTracker")
        {
            GameObject trackerGo = new GameObject(name);
            trackerGo.transform.SetParent(source.transform, false);

            PolygonCollider2D newColl = trackerGo.AddComponent<PolygonCollider2D>();
            newColl.isTrigger = true;
            newColl.offset = source.offset;

            // Bước A: Tính toán Tâm (Center) của Polygon dựa trên Bounds Local
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            for (int i = 0; i < source.pathCount; i++)
            {
                foreach (var p in source.GetPath(i))
                {
                    if (p.x < minX) minX = p.x;
                    if (p.x > maxX) maxX = p.x;
                    if (p.y < minY) minY = p.y;
                    if (p.y > maxY) maxY = p.y;
                }
            }
            Vector2 center = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);

            // Bước B: Sao chép và Scale quanh tâm
            newColl.pathCount = source.pathCount;
            for (int i = 0; i < source.pathCount; i++)
            {
                Vector2[] pathPoints = source.GetPath(i);
                for (int j = 0; j < pathPoints.Length; j++)
                {
                    // 1. Dời điểm về gốc tọa độ (so với tâm)
                    Vector2 relativePoint = pathPoints[j] - center;

                    // 2. Nhân tỉ lệ
                    relativePoint.x *= scaleX;
                    relativePoint.y *= scaleY;

                    // 3. Đưa điểm trở lại vị trí tương ứng với tâm cũ
                    pathPoints[j] = relativePoint + center;
                }
                newColl.SetPath(i, pathPoints);
            }

            return newColl;
        }
        public static PolygonCollider2D CloneWithMarginByScale(this PolygonCollider2D source, float margin = 0.1f, string name = "SceneTracker")
        {
            // 1. Tính toán Kích thước thực tế (Width/Height)
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            for (int i = 0; i < source.pathCount; i++)
            {
                foreach (var p in source.GetPath(i))
                {
                    if (p.x < minX) minX = p.x;
                    if (p.x > maxX) maxX = p.x;
                    if (p.y < minY) minY = p.y;
                    if (p.y > maxY) maxY = p.y;
                }
            }

            float width = maxX - minX;
            float height = maxY - minY;

            if (width <= 0 || height <= 0) return null;

            // 2. Tính toán tỉ lệ Scale dựa trên Margin mong muốn
            // Công thức: (Cạnh mới) / (Cạnh cũ)
            float scaleX = (width - (margin * 2)) / width;
            float scaleY = (height - (margin * 2)) / height;

            // Đảm bảo không bị scale âm khi margin quá lớn so với vật thể
            scaleX = Mathf.Max(0.01f, scaleX);
            scaleY = Mathf.Max(0.01f, scaleY);

            // 3. Thực hiện Clone và Scale tại tâm
            return source.CloneAndScaleMaintainPivot(scaleX, scaleY, name);
        }

        /// <summary>
        /// Tìm index của cạnh gần như thẳng đứng nhất xuất phát từ điểm có X lớn nhất.
        /// </summary>
        public static int GetMostVerticalEdgeAtMaxX(this PolygonCollider2D polygon)
        {
            Vector2[] points = polygon.points;
            if (points.Length < 3) return -1;

            // 1. Tìm index của điểm có tọa độ X lớn nhất
            int maxXIndex = 0;
            float maxXValue = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x > maxXValue)
                {
                    maxXValue = points[i].x;
                    maxXIndex = i;
                }
            }

            // 2. Xác định 2 điểm lân cận để tạo thành 2 cạnh
            int prevIndex = (maxXIndex - 1 + points.Length) % points.Length;
            int nextIndex = (maxXIndex + 1) % points.Length;

            Vector2 pMax = points[maxXIndex];
            Vector2 pPrev = points[prevIndex];
            Vector2 pNext = points[nextIndex];

            // 3. Tính độ dốc (hoặc góc) của 2 cạnh
            // Cạnh 1: pPrev -> pMax (Index của cạnh này thường là prevIndex)
            // Cạnh 2: pMax -> pNext (Index của cạnh này thường là maxXIndex)

            float diffPrev = Mathf.Abs(pMax.x - pPrev.x);
            float diffNext = Mathf.Abs(pMax.x - pNext.x);

            // Cạnh nào có chênh lệch X càng nhỏ so với chênh lệch Y thì càng "thẳng đứng"
            // Hoặc đơn giản hơn: Cạnh nào có Delta X nhỏ hơn thì đứng hơn
            if (diffPrev < diffNext)
            {
                return prevIndex;
            }
            else
            {
                return maxXIndex;
            }
        }
        /// <summary>
        /// Tìm index của cạnh thẳng đứng nhất xuất phát từ điểm có X nhỏ nhất (phía bên trái).
        /// </summary>
        public static int GetMostVerticalEdgeAtMinX(this PolygonCollider2D polygon)
        {
            Vector2[] points = polygon.points;
            if (points.Length < 3) return -1;

            // 1. Tìm index của điểm có tọa độ X nhỏ nhất
            int minXIndex = 0;
            float minXValue = float.MaxValue;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x < minXValue)
                {
                    minXValue = points[i].x;
                    minXIndex = i;
                }
            }

            // 2. Xác định 2 điểm lân cận
            int prevIndex = (minXIndex - 1 + points.Length) % points.Length;
            int nextIndex = (minXIndex + 1) % points.Length;

            Vector2 pMin = points[minXIndex];
            Vector2 pPrev = points[prevIndex];
            Vector2 pNext = points[nextIndex];

            // 3. So sánh độ dốc dựa trên Delta X
            // Cạnh 1 (nối với điểm trước): Delta X1 = |x_min - x_prev|
            // Cạnh 2 (nối với điểm sau): Delta X2 = |x_min - x_next|

            float deltaXPrev = Mathf.Abs(pMin.x - pPrev.x);
            float deltaXNext = Mathf.Abs(pMin.x - pNext.x);

            // Cạnh nào có Delta X nhỏ hơn thì cạnh đó tiến gần đến đường thẳng đứng (vertical) hơn
            if (deltaXPrev < deltaXNext)
            {
                return prevIndex;
            }
            else
            {
                return minXIndex;
            }
        }
        /// <summary>
        /// Nắn cạnh đứng nhất bên phải của Polygon về đường thẳng dọc x = targetX
        /// </summary>
        public static void SnapRightEdgeToX(this PolygonCollider2D polygon, float targetX)
        {
            Vector2[] points = polygon.points;
            if (points.Length < 3) return;

            // 1. Chuyển targetX từ World Space về Local Space của Polygon và trừ đi offset
            float localTargetX = polygon.transform.InverseTransformPoint(new Vector3(targetX, 0, 0)).x - polygon.offset.x;

            // 2. Tìm điểm có X lớn nhất (Local)
            int maxXIndex = 0;
            float maxXValue = float.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x > maxXValue)
                {
                    maxXValue = points[i].x;
                    maxXIndex = i;
                }
            }

            // 3. Xác định cạnh đứng nhất tại điểm đó
            int prev = (maxXIndex - 1 + points.Length) % points.Length;
            int next = (maxXIndex + 1) % points.Length;

            int edgePartnerIndex = (Mathf.Abs(points[maxXIndex].x - points[prev].x) <
                                   Mathf.Abs(points[maxXIndex].x - points[next].x)) ? prev : next;

            // 4. Sửa tọa độ X của cả 2 điểm tạo nên cạnh đó
            points[maxXIndex].x = localTargetX;
            points[edgePartnerIndex].x = localTargetX;

            // 5. Cập nhật lại Collider
            polygon.points = points;
        }
        /// <summary>
        /// Nắn cạnh đứng nhất bên trái của Polygon về đường thẳng dọc x = targetX
        /// </summary>
        public static void SnapLeftEdgeToX(this PolygonCollider2D polygon, float targetX)
        {
            Vector2[] points = polygon.points;
            if (points.Length < 3) return;

            // 1. Chuyển targetX từ World Space về Local Space của Polygon và trừ đi offset
            float localTargetX = polygon.transform.InverseTransformPoint(new Vector3(targetX, 0, 0)).x - polygon.offset.x;

            // Tìm điểm có X nhỏ nhất
            int minXIndex = 0;
            float minXValue = float.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].x < minXValue)
                {
                    minXValue = points[i].x;
                    minXIndex = i;
                }
            }

            int prev = (minXIndex - 1 + points.Length) % points.Length;
            int next = (minXIndex + 1) % points.Length;

            int edgePartnerIndex = (Mathf.Abs(points[minXIndex].x - points[prev].x) <
                                   Mathf.Abs(points[minXIndex].x - points[next].x)) ? prev : next;

            // Nắn 2 điểm về đường dọc
            points[minXIndex].x = localTargetX;
            points[edgePartnerIndex].x = localTargetX;

            polygon.points = points;
        }

        /// <summary>
        /// Tìm tọa độ giao điểm Y trên một cạnh của Polygon khi biết tọa độ X.
        /// </summary>
        /// <param name="polygon">Collider cần kiểm tra.</param>
        /// <param name="edgeIndex">Index của điểm bắt đầu cạnh (0 đến points.Length-1).</param>
        /// <param name="targetX">Tọa độ X thế giới (World Space) của đường thẳng đứng.</param>
        /// <returns>Tọa độ Y giao điểm trong World Space. Trả về float.NaN nếu không có giao điểm hoặc cạnh thẳng đứng.</returns>
        public static float GetYIntersectionOnEdge(this PolygonCollider2D polygon, int edgeIndex, float targetX)
        {
            Vector2[] points = polygon.points;
            int pointCount = points.Length;

            if (pointCount < 3 || edgeIndex < 0 || edgeIndex >= pointCount) return float.NaN;

            // 1. Lấy 2 điểm đầu mút của cạnh và chuyển sang World Space
            int nextIndex = (edgeIndex + 1) % pointCount;
            Vector3 p1 = polygon.transform.TransformPoint(points[edgeIndex] + polygon.offset);
            Vector3 p2 = polygon.transform.TransformPoint(points[nextIndex] + polygon.offset);

            // 2. Xác định phạm vi X của cạnh
            float minX = Mathf.Min(p1.x, p2.x);
            float maxX = Mathf.Max(p1.x, p2.x);

            // 3. CƠ CHẾ CHỐNG NaN: Ép targetX vào trong phạm vi của cạnh
            // Nếu targetX lệch ngoài một chút (do bounds của Wall), ta lấy điểm đầu mút gần nhất
            float clampedX = Mathf.Clamp(targetX, minX, maxX);

            // 4. Trường hợp đặc biệt: Cạnh thẳng đứng (p1.x == p2.x)
            if (Mathf.Approximately(p1.x, p2.x))
            {
                // Trả về Y trung bình hoặc Y của điểm gần Player nhất
                return (p1.y + p2.y) / 2f;
            }

            // 5. Tính toán Y bằng nội suy tuyến tính (Linear Interpolation)
            // Công thức: y = y1 + (clampedX - x1) * (y2 - y1) / (x2 - x1)
            float t = (clampedX - p1.x) / (p2.x - p1.x);
            float intersectY = p1.y + t * (p2.y - p1.y);

            return intersectY;
        }
        /// <summary>
        /// Procedure Breakdown
        ///Locate A as the polygon vertex where x is minimized.
        ///Compute slopes: slope_AB = (y_B - y_A) / (x_B - x_A); slope_AC = (y_C - y_A) / (x_C - x_A).
        ///Choose the edge(AB or AC) with max |slope| value, as higher magnitude means closer to vertical(infinite slope).
        ///The indices of the two points (A and the endpoint of the most vertical edge, say B) 
        ///define the left supporting edge—the edge from the leftmost point 
        ///that most directly "supports" the polygon against horizontal 
        ///shifts from the left. This identifies the leftmost vertical profile for collision detection or bounding
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="start">index of first point</param>
        /// <param name="end">index of last point</param>
        public static void LeftVerticalFacet(this PolygonCollider2D poly, out int start, out int end)
        {
            Vector2[] points = poly.points;
            if (points == null || points.Length < 2) { start = end = -1; return; }

            int a = 0;
            float minX = points[0].x;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].x < minX) { minX = points[i].x; a = i; }
            }

            int b = (a + 1) % points.Length;
            int c = (a - 1 + points.Length) % points.Length;

            Vector2 ab = points[b] - points[a];
            Vector2 ac = points[c] - points[a];

            if (Mathf.Abs(ab.y * ac.x) >= Mathf.Abs(ac.y * ab.x)) { start = a; end = b; }
            else { start = a; end = c; }
        }
        public static void RightVerticalFacet(this PolygonCollider2D poly, out int start, out int end)
        {
            Vector2[] points = poly.points;
            if (points == null || points.Length < 2) { start = end = -1; return; }

            int a = 0;
            float maxX = points[0].x;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].x > maxX) { maxX = points[i].x; a = i; }
            }

            int b = (a + 1) % points.Length;
            int c = (a - 1 + points.Length) % points.Length;

            Vector2 ab = points[b] - points[a];
            Vector2 ac = points[c] - points[a];

            if (Mathf.Abs(ab.y * ac.x) >= Mathf.Abs(ac.y * ab.x)) { start = a; end = b; }
            else { start = a; end = c; }
        }
        public static void TopHorizontalFacet(this PolygonCollider2D poly, out int start, out int end)
        {
            Vector2[] points = poly.points;
            if (points == null || points.Length < 2) { start = end = -1; return; }

            int a = 0;
            float maxY = points[0].y;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].y > maxY) { maxY = points[i].y; a = i; }
            }

            int b = (a + 1) % points.Length;
            int c = (a - 1 + points.Length) % points.Length;

            Vector2 ab = points[b] - points[a];
            Vector2 ac = points[c] - points[a];

            if (Mathf.Abs(ab.y * ac.x) <= Mathf.Abs(ac.y * ab.x)) { start = a; end = b; }
            else { start = a; end = c; }
        }
        public static void BootomHorizontalFacet(this PolygonCollider2D poly, out int start, out int end)
        {
            Vector2[] points = poly.points;
            if (points == null || points.Length < 2) { start = end = -1; return; }

            int a = 0;
            float minY = points[0].y;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].y < minY) { minY = points[i].y; a = i; }
            }

            int b = (a + 1) % points.Length;
            int c = (a - 1 + points.Length) % points.Length;

            Vector2 ab = points[b] - points[a];
            Vector2 ac = points[c] - points[a];

            if (Mathf.Abs(ab.y * ac.x) <= Mathf.Abs(ac.y * ab.x)) { start = a; end = b; }
            else { start = a; end = c; }
        }

        public static void VerticalFacetAlign(this PolygonCollider2D poly, int start, int end, float x)
        {
            if (poly == null || start < 0 || end < 0) return;
            Vector2[] points = poly.points;
            if (start >= points.Length || end >= points.Length) return;

            // Xử lý từng điểm để đảm bảo tính đúng đắn khi có Scale/Rotation/Offset
            Vector3 worldStart = poly.transform.TransformPoint(points[start] + poly.offset);
            worldStart.x = x;
            points[start] = (Vector2)poly.transform.InverseTransformPoint(worldStart) - poly.offset;

            Vector3 worldEnd = poly.transform.TransformPoint(points[end] + poly.offset);
            worldEnd.x = x;
            points[end] = (Vector2)poly.transform.InverseTransformPoint(worldEnd) - poly.offset;

            poly.points = points;
        }
        public static void HorizontalFacetAlign(this PolygonCollider2D poly, int start, int end, float y)
        {
            if (poly == null || start < 0 || end < 0) return;
            Vector2[] points = poly.points;
            if (start >= points.Length || end >= points.Length) return;

            // Xử lý từng điểm để đảm bảo tính đúng đắn khi có Scale/Rotation/Offset
            Vector3 worldStart = poly.transform.TransformPoint(points[start] + poly.offset);
            worldStart.y = y;
            points[start] = (Vector2)poly.transform.InverseTransformPoint(worldStart) - poly.offset;

            Vector3 worldEnd = poly.transform.TransformPoint(points[end] + poly.offset);
            worldEnd.y = y;
            points[end] = (Vector2)poly.transform.InverseTransformPoint(worldEnd) - poly.offset;

            poly.points = points;
        }

        public static Vector2[] CreateNewFromPoints(this PolygonCollider2D coll, float scaleRatio = 1.3f)
        {
            var points = coll.points;
            var ret = new Vector2[points.Length];
            for (var i = 0; i < ret.Length; i++)
            {
                // Convert to world space (including offset) and then apply scaleRatio
                Vector2 worldPoint = coll.transform.TransformPoint(points[i] + coll.offset);
                ret[i] = worldPoint * scaleRatio;
            }
            return ret;
        }

        public enum AxisScaleEnum
        {
            x = 1, y = 2, both = 3
        }

        public static void ClearRotationAndScale(this PolygonCollider2D coll, float scaleRatio = 1.03f, AxisScaleEnum axis = AxisScaleEnum.y)
        {

            //Vector2 oldOffset = coll.offset;
            //Vector2[] points = coll.points;
            //Vector2[] ret = new Vector2[points.Length];

            //Vector3 localScale = coll.transform.localScale;
            //Quaternion localRotation = coll.transform.localRotation;

            //// Calculate overall scale factor including transform scale and scaleRatio
            //Vector2 scaleFactor = new Vector2(localScale.x, localScale.y);
            switch (axis)
            {
                case AxisScaleEnum.both:
                    coll.ScalePolygonCollider2D_KeepCentroid(scaleRatio);
                    break;
                case AxisScaleEnum.x:
                    coll.ScalePolygonCollider2D_KeepCentroid_X(scaleRatio);
                    break;
                case AxisScaleEnum.y:
                    coll.ScalePolygonCollider2D_KeepCentroid_Y(scaleRatio);
                    break;
            }

            //// 1. Bake rotation and scale into the offset (center)
            //coll.offset = localRotation * Vector2.Scale(oldOffset, scaleFactor);

            //// 2. Bake rotation and scale into each point
            //for (var i = 0; i < points.Length; i++)
            //{
            //    ret[i] = localRotation * Vector2.Scale(points[i], scaleFactor);
            //}

            //coll.points = ret;

            // 3. Reset local transform
            //var samplePoint = coll.points[0] * 1;
            //coll.transform.localScale = coll.transform.localScale*scaleRatio;

            //var sampleNewPoint = coll.points[0] * 1;
            //var dis=Vector2.Distance(samplePoint, sampleNewPoint * scaleRatio);
            //coll.offset = new Vector2(dis, dis)*scaleRatio;
            //coll.transform.localRotation = Quaternion.identity;
            //Vector3 oldCenter = coll.bounds.center;

            //coll.transform.localScale *= scaleRatio;

            //Vector3 newCenter = coll.bounds.center;
            //Vector3 offset = oldCenter - newCenter;

            //coll.transform.position += offset;
            
        }
        public static void ScalePolygonCollider2D_KeepCentroid(this PolygonCollider2D col, float scale)
        {
            var pts = col.points;

            // Tính centroid polygon chuẩn (shoelace formula)
            Vector2 centroid = ComputePolygonCentroid(pts);

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = centroid + (pts[i] - centroid) * scale;
            }

            col.points = pts;
        }
        public static void ScalePolygonCollider2D_KeepCentroid_Y(this PolygonCollider2D col, float scaleY)
        {
            var pts = col.points;

            Vector2 centroid = ComputePolygonCentroid(pts);

            for (int i = 0; i < pts.Length; i++)
            {
                Vector2 p = pts[i];
                float newY = centroid.y + (p.y - centroid.y) * scaleY;

                pts[i] = new Vector2(p.x, newY);
            }

            col.points = pts;
        }
        public static void ScalePolygonCollider2D_KeepCentroid_X(this PolygonCollider2D col, float scaleX)
        {
            var pts = col.points;

            Vector2 centroid = ComputePolygonCentroid(pts);

            for (int i = 0; i < pts.Length; i++)
            {
                Vector2 p = pts[i];
                float newX = centroid.x + (p.x - centroid.x) * scaleX;

                pts[i] = new Vector2(newX, p.y);
            }

            col.points = pts;
        }
        private static Vector2 ComputePolygonCentroid(this Vector2[] pts)
        {
            float signedArea = 0f;
            float cx = 0f;
            float cy = 0f;

            for (int i = 0; i < pts.Length; i++)
            {
                Vector2 p0 = pts[i];
                Vector2 p1 = pts[(i + 1) % pts.Length];

                float a = p0.x * p1.y - p1.x * p0.y;
                signedArea += a;

                cx += (p0.x + p1.x) * a;
                cy += (p0.y + p1.y) * a;
            }

            signedArea *= 0.5f;

            if (Mathf.Abs(signedArea) < 0.00001f)
            {
                // fallback nếu polygon degenerate
                Vector2 avg = Vector2.zero;
                foreach (var p in pts) avg += p;
                return avg / pts.Length;
            }

            cx /= (6f * signedArea);
            cy /= (6f * signedArea);

            return new Vector2(cx, cy);
        }
    }
}
