using System;
using System.Collections;
using Unity.Burst;
using UnityEngine;

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
    }
}