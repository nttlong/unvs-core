#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace unvs.ext
{
    public static class GizmosExtension
    {
        public static void GizmosDraw(this EdgeCollider2D edgeCollider, Color? color = null, float thickness = 5f)
        {
            if (edgeCollider == null || edgeCollider.pointCount < 2) return;

            // Thiết lập màu sắc (mặc định là đỏ nếu không truyền vào)
            Handles.color = color ?? Color.red;

            // Lấy ma trận biến đổi của đối tượng để các điểm vẽ đúng vị trí/xoay/tỉ lệ
            Matrix4x4 drawingMatrix = edgeCollider.transform.localToWorldMatrix;
            Handles.matrix = drawingMatrix;

            Vector2[] points = edgeCollider.points;

            for (int i = 0; i < points.Length - 1; i++)
            {
                // Sử dụng Handles.DrawBezier hoặc Handles.DrawAAPolyLine để có độ dày
                // Ở đây dùng DrawLine với độ dày (thickness)
                Handles.DrawBezier(
                    points[i],
                    points[i + 1],
                    points[i],
                    points[i + 1],
                    Handles.color,
                    null,
                    thickness
                );
            }

            // Reset ma trận về mặc định sau khi vẽ xong để không ảnh hưởng Gizmos khác
            Handles.matrix = Matrix4x4.identity;
        }
        public static void GizmosDraw(this BoxCollider2D box, Color? color = null, float thickness = 5f)
        {
            if (box == null) return;

            Handles.color = color ?? Color.red;
            Handles.matrix = box.transform.localToWorldMatrix;

            // Tính toán 4 góc của Box dựa trên size và offset của Collider
            Vector2 size = box.size;
            Vector2 offset = box.offset;

            Vector3 topLeft = new Vector3(offset.x - size.x / 2f, offset.y + size.y / 2f, 0f);
            Vector3 topRight = new Vector3(offset.x + size.x / 2f, offset.y + size.y / 2f, 0f);
            Vector3 bottomLeft = new Vector3(offset.x - size.x / 2f, offset.y - size.y / 2f, 0f);
            Vector3 bottomRight = new Vector3(offset.x + size.x / 2f, offset.y - size.y / 2f, 0f);

            // Tạo mảng các điểm để vẽ đường khép kín
            Vector3[] corners = { topLeft, topRight, bottomRight, bottomLeft, topLeft };

            // Vẽ đường đa tuyến có độ dày (AA Poly Line)
            Handles.DrawAAPolyLine(thickness, corners);

            Handles.matrix = Matrix4x4.identity;
        }

        // --- Vẽ cho PolygonCollider2D ---
        public static void GizmosDraw(this PolygonCollider2D poly, Color? color = null, float thickness = 5f)
        {
            if (poly == null || poly.pathCount == 0) return;

            Handles.color = color ?? Color.red;
            Handles.matrix = poly.transform.localToWorldMatrix;

            // Polygon có thể có nhiều đường (path), ta vẽ từng đường một
            for (int i = 0; i < poly.pathCount; i++)
            {
                Vector2[] pathPoints = poly.GetPath(i);
                if (pathPoints.Length < 2) continue;

                // Chuyển Vector2[] sang Vector3[] và thêm điểm cuối trùng điểm đầu để khép kín
                // Chuyển Vector2[] sang Vector3[] và thêm điểm cuối trùng điểm đầu để khép kín
                Vector3[] pointsToDraw = new Vector3[pathPoints.Length + 1];
                for (int j = 0; j < pathPoints.Length; j++)
                {
                    pointsToDraw[j] = (Vector3)pathPoints[j];
                }
                pointsToDraw[pathPoints.Length] = (Vector3)pathPoints[0]; // Điểm đóng

                Handles.DrawAAPolyLine(thickness, pointsToDraw);
            }

            Handles.matrix = Matrix4x4.identity;
        }
        public static void GismosDrawHatchBox(this BoxCollider2D box, Color color, float opacity = 0.5f, float spacing = 0.2f, float angle = 45f)
        {
            if (box == null) return;

            // Thiết lập màu sắc với độ trong suốt
            color.a = opacity;
            Gizmos.color = color;

            // --- Giữ nguyên logic tính toán tọa độ của bạn ---
            Vector2 size = box.size;
            Vector2 offset = box.offset;
            Transform t = box.transform;

            Vector2 p1 = t.TransformPoint(offset + new Vector2(-size.x, -size.y) * 0.5f);
            Vector2 p2 = t.TransformPoint(offset + new Vector2(size.x, -size.y) * 0.5f);
            Vector2 p3 = t.TransformPoint(offset + new Vector2(size.x, size.y) * 0.5f);
            Vector2 p4 = t.TransformPoint(offset + new Vector2(-size.x, size.y) * 0.5f);

            Vector2[] corners = { p1, p2, p3, p4 };

            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector2 normal = new Vector2(-dir.y, dir.x);

            float minDot = float.MaxValue;
            float maxDot = float.MinValue;
            foreach (var corner in corners)
            {
                float dot = Vector2.Dot(corner, normal);
                minDot = Mathf.Min(minDot, dot);
                maxDot = Mathf.Max(maxDot, dot);
            }

            for (float d = minDot + spacing; d < maxDot; d += spacing)
            {
                List<Vector2> intersections = new List<Vector2>();
                for (int i = 0; i < 4; i++)
                {
                    Vector2 start = corners[i];
                    Vector2 end = corners[(i + 1) % 4];
                    float dot1 = Vector2.Dot(start, normal) - d;
                    float dot2 = Vector2.Dot(end, normal) - d;

                    if (dot1 * dot2 < 0)
                    {
                        float factor = Mathf.Abs(dot1) / (Mathf.Abs(dot1) + Mathf.Abs(dot2));
                        intersections.Add(Vector2.Lerp(start, end, factor));
                    }
                }

                if (intersections.Count >= 2)
                {
                    intersections.Sort((a, b) => Vector2.Dot(a, dir).CompareTo(Vector2.Dot(b, dir)));
                    Gizmos.DrawLine(intersections[0], intersections[1]);
                }
            }
        }
        public static void GismosDrawHatchPolygon(this PolygonCollider2D poly, Color color, float opacity = 0.5f, float spacing = 0.2f, float angle = 45f)
        {
            if (poly == null) return;

            // 1. Thiết lập màu sắc với độ trong suốt
            color.a = opacity;
            Gizmos.color = color;

            // 2. Tạo vector hướng của đường gạch (hatch direction)
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            // Vector vuông góc để quét
            Vector2 normal = new Vector2(-dir.y, dir.x);

            // 3. Lấy các điểm world của Polygon và xác định phạm vi quét
            float minDot = float.MaxValue;
            float maxDot = float.MinValue;

            Vector2[][] paths = new Vector2[poly.pathCount][];
            for (int i = 0; i < poly.pathCount; i++)
            {
                Vector2[] localPoints = poly.GetPath(i);
                paths[i] = new Vector2[localPoints.Length];
                for (int j = 0; j < localPoints.Length; j++)
                {
                    // Chuyển sang tọa độ World
                    paths[i][j] = poly.transform.TransformPoint(localPoints[j] + poly.offset);

                    float dot = Vector2.Dot(paths[i][j], normal);
                    if (dot < minDot) minDot = dot;
                    if (dot > maxDot) maxDot = dot;
                }
            }

            // 4. Bắt đầu quét các đường gạch chéo
            for (float d = minDot + spacing; d < maxDot; d += spacing)
            {
                List<Vector2> intersections = new List<Vector2>();

                for (int i = 0; i < paths.Length; i++)
                {
                    Vector2[] path = paths[i];
                    for (int j = 0; j < path.Length; j++)
                    {
                        Vector2 p1 = path[j];
                        Vector2 p2 = path[(j + 1) % path.Length];

                        float dot1 = Vector2.Dot(p1, normal) - d;
                        float dot2 = Vector2.Dot(p2, normal) - d;

                        // Kiểm tra xem đường quét có nằm giữa 2 điểm của cạnh không
                        if (dot1 * dot2 < 0)
                        {
                            float t = Mathf.Abs(dot1) / (Mathf.Abs(dot1) + Mathf.Abs(dot2));
                            intersections.Add(Vector2.Lerp(p1, p2, t));
                        }
                    }
                }

                // 5. Sắp xếp các giao điểm dọc theo hướng đường gạch
                intersections.Sort((a, b) => Vector2.Dot(a, dir).CompareTo(Vector2.Dot(b, dir)));

                // 6. Vẽ các đoạn thẳng nối từng cặp giao điểm (xử lý được cả đa giác lõm/có lỗ)
                for (int k = 0; k < intersections.Count - 1; k += 2)
                {
                    Gizmos.DrawLine(intersections[k], intersections[k + 1]);
                }
            }
        }

        public static void DrawCircle(this Vector2 pos, float radius, Color color, float thickness=0f)
        {
            if (pos == Vector2.zero || pos == Vector2.negativeInfinity || pos == Vector2.positiveInfinity) return;
            Gizmos.color = color;

            // Nếu thickness <= 0, chỉ vẽ 1 vòng đơn cho nhẹ
            if (thickness <= 0.01f)
            {
                Gizmos.DrawWireSphere((Vector3)pos, radius);
                return;
            }

            // Vẽ nhiều vòng đồng tâm để tạo cảm giác "dày"
            // Điều này giúp team họa sĩ của bạn dễ nhìn thấy điểm chốt hơn
            float halfThick = thickness / 2f;
            int stepCount = 3; // Số lớp vẽ thêm
            for (int i = 0; i <= stepCount; i++)
            {
                float currentRadius = radius - halfThick + (thickness * i / stepCount);
                Gizmos.DrawWireSphere((Vector3)pos, currentRadius);
            }
        }
        public static void DrawCircle(this Vector3 pos, float radius, Color color,float thickness=0 )
        {
            if (pos == Vector3.zero || pos == Vector3.negativeInfinity || pos == Vector3.positiveInfinity) return;
            Gizmos.color = color;

            // Nếu thickness <= 0, chỉ vẽ 1 vòng đơn cho nhẹ
            if (thickness <= 0.01f)
            {
                Gizmos.DrawWireSphere((Vector3)pos, radius);
                return;
            }

            // Vẽ nhiều vòng đồng tâm để tạo cảm giác "dày"
            // Điều này giúp team họa sĩ của bạn dễ nhìn thấy điểm chốt hơn
            float halfThick = thickness / 2f;
            int stepCount = 3; // Số lớp vẽ thêm
            for (int i = 0; i <= stepCount; i++)
            {
                float currentRadius = radius - halfThick + (thickness * i / stepCount);
                Gizmos.DrawWireSphere((Vector3)pos, currentRadius);
            }
        }
    }
}
#endif
