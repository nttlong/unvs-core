using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.XR;
using unvs.interfaces;
using static UnityEngine.Audio.GeneratorInstance;

namespace unvs.ext
{
    public static class Vector3Extension
    {
        static Dictionary<string, int> layerMaskCache = new Dictionary<string, int>();
        public static int GetLayerMaskFromCache(string LayerName, params string[] LayerNames)
        {
            var key = $"{LayerName},{string.Join(',', LayerNames)}";
            if (layerMaskCache.TryGetValue(LayerName, out var mask)) return mask;
            var layers = new List<string>(LayerNames);
            layers.Add(LayerName);
            mask = LayerMask.GetMask(layers.ToArray());
            layerMaskCache.TryAdd(key, mask);
            return mask;
        }
        public static bool IsOverObject(this Vector3 mouseWorldPos, out GameObject hitObject, string LayerName, params string[] LayerNames)
        {
            var mask = GetLayerMaskFromCache(LayerName, LayerNames);
            hitObject = null;

            // 1. Chuyển tọa độ chuột từ Screen sang World (2D)
            // Lưu ý: Đối với Camera Orthographic, giá trị Z không quan trọng nhưng nên để mặc định

            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // 2. Kiểm tra xem có Collider2D nào tại điểm đó không
            Collider2D hit = Physics2D.OverlapPoint(mousePos2D, mask);

            if (hit != null)
            {
                hitObject = hit.gameObject;
                return true;
            }

            return false;
        }
        public static bool IsOverObject(this Vector3 mouseWorldPos, out GameObject hitObject, int mask)
        {

            hitObject = null;

            // 1. Chuyển tọa độ chuột từ Screen sang World (2D)
            // Lưu ý: Đối với Camera Orthographic, giá trị Z không quan trọng nhưng nên để mặc định

            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // 2. Kiểm tra xem có Collider2D nào tại điểm đó không
            Collider2D hit = Physics2D.OverlapPoint(mousePos2D, mask);

            if (hit != null)
            {
                hitObject = hit.gameObject;
                return true;
            }

            return false;
        }
        
        public static bool IsOverObject(this Vector2 mouseWorldPos, out GameObject hitObject, int mask)
        {

            hitObject = null;

            // 1. Chuyển tọa độ chuột từ Screen sang World (2D)
            // Lưu ý: Đối với Camera Orthographic, giá trị Z không quan trọng nhưng nên để mặc định

            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            // 2. Kiểm tra xem có Collider2D nào tại điểm đó không
            Collider2D hit = Physics2D.OverlapPoint(mousePos2D, mask);

            if (hit != null)
            {
                hitObject = hit.gameObject;
                return true;
            }

            return false;
        }
        public static Vector3 ToWorld(this Vector3 position)
        {
            //if(SceneController.instance==null) return Vector3.zero;
            if (Camera.main == null) throw new Exception("Camera.main is null");
            // 1. Lấy vị trí chuột trên màn hình (Pixels)


            // 2. Với Orthographic, Z không quan trọng nhưng nên để bằng khoảng cách từ Camera tới mặt phẳng 0
            position.z = 10f;

            // 3. Chuyển đổi
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);

            // 4. Ép Z về 0 nếu bạn làm game 2D thuần túy
            worldPos.z = 0f;

            return worldPos;
        }
        public static Vector2 ToWorld(this Vector2 position)
        {
            if (Camera.main == null) throw new Exception("Camera.main is null");
            // 1. Lấy vị trí chuột trên màn hình (Pixels)




            // 3. Chuyển đổi
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(position);



            return worldPos;
        }
        /// <summary>
        /// Chuyển đổi tọa độ World sang tọa độ Screen (Pixel)
        /// </summary>
        /// <param name="worldPos">Vị trí trong Game World</param>
        /// <param name="camera">Camera thực hiện tính toán (mặc định là Camera.main)</param>
        /// <returns>Tọa độ Vector2 trên màn hình</returns>
        public static Vector2 ToScreen(this Vector3 worldPos, Camera camera = null)
        {
            return ((Vector2)worldPos).ToScreen();
        }
        /// <summary>
        /// Chuyển đổi tọa độ World sang tọa độ Screen (Pixel)
        /// </summary>
        /// <param name="worldPos">Vị trí trong Game World</param>
        /// <param name="camera">Camera thực hiện tính toán (mặc định là Camera.main)</param>
        /// <returns>Tọa độ Vector2 trên màn hình</returns>
        public static Vector2 ToScreen(this Vector2 worldPos, Camera camera = null)
        {
            // Sử dụng camera truyền vào, nếu không có thì lấy Camera.main
            Camera cam = camera != null ? camera : Camera.main;

            if (cam == null)
            {
                Debug.LogError("ToScreen: No camera found!");
                return Vector2.zero;
            }

            // Chuyển sang Vector3 và đảm bảo Z không gây lỗi tính toán
            // Trong game 2D/2.5D, ta thường giả định vật thể nằm ở mặt phẳng Z=0
            Vector3 worldPos3D = new Vector3(worldPos.x, worldPos.y, 0f);

            Vector3 screenPoint = cam.WorldToScreenPoint(worldPos3D);

            // Kiểm tra nếu vật thể nằm sau lưng Camera (z < 0 trong Screen Space)
            if (screenPoint.z < 0)
            {
                // Debug.LogWarning("Vật thể nằm ngoài tầm nhìn của Camera");
                return Vector2.zero;
            }

            return new Vector2(screenPoint.x, screenPoint.y);
        }
        public static float CalculateDirection(this Vector3 v,Vector2 other)
        {
            if (v.x > other.x) return 1;
            if (v.x < other.x) return -1;
            return 0;
        }
        public static float GetDirectionTo(this Vector3 From, Vector2 To)
        {
            if (From.x > To.x) return -1;
            if (From.x < To.x) return 1;
            return 0;
        }
        public static float GetDirectionTo(this Vector3 From, Vector3 To)
        {
            if (From.x > To.x) return -1;
            if (From.x < To.x) return 1;
            return 0;
        }
        public static Vector3 CloneToNew(this Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        public static Vector3 FlipX(this Vector3 v)
        {
            return new Vector3(-v.x, v.y, v.z);
        }
        public static Vector2 CloneToNew(this Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }
        public static Vector2 FlipX(this Vector2 v)
        {
            return new Vector2(-v.x, v.y);
        }

      

        public static GameObject GetObjectInLayer(this Vector2 point, int LayerMask)
        {
            Collider2D col = Physics2D.OverlapPoint(point, LayerMask);
            if (col != null)
            {
                return col.gameObject;
            }
            return null;
        }
        public static GameObject GetClosestTarget(this Vector3 currentPosition, Collider2D[] targets)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;


            foreach (Collider2D potentialTarget in targets)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.gameObject;
                }
            }
            return bestTarget;
        }
        public static GameObject GetClosestTarget(this Vector2 currentPosition, Collider2D[] targets)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;


            foreach (Collider2D potentialTarget in targets)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - (Vector3)currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.gameObject;
                }
            }
            return bestTarget;
        }

        public static Vector3 CopyToNew(this Vector3 localScale)
        {
            return new Vector3(localScale.x, localScale.y, localScale.z);
        }
    }
    public static class Vector2dExtesion
    {
        public static Bounds CalculateLocalBoundsths(this Vector2[] points)
        {
            if (points == null || points.Length == 0) return new Bounds(Vector3.zero, Vector3.zero);

            Vector2 min = points[0];
            Vector2 max = points[0];

            for (int i = 1; i < points.Length; i++)
            {
                min = Vector2.Min(min, points[i]);
                max = Vector2.Max(max, points[i]);
            }

            Bounds b = new Bounds();
            b.SetMinMax(min, max);
            return b;
        }
        public static bool IsOverObject(this Vector2 pos, out GameObject hitObject, string LayerName, params string[] LayerNames)
        {
            return ((Vector3)pos).IsOverObject(out hitObject,LayerName, LayerNames);
        }
        /// <summary>
        /// Kiểm tra có va chạm với object chứa component T không
        /// </summary>
        public static bool IsHitObject<T>(this Vector2 pos, out T hitObject, string LayerName, params string[] LayerNames)
            where T : Component
        {
            hitObject = null;

            if (((Vector3)pos).IsOverObject(out GameObject obj, LayerName, LayerNames))
            {
                hitObject = obj.GetComponent<T>();
                return hitObject != null;
            }

            return false;
        }
        public static T IsHitObject<T>(this Vector2 pos, string LayerName, params string[] LayerNames)
        {
            if (((Vector3)pos).IsOverObject(out var hitObject, LayerName, LayerNames))
            {
                return hitObject.GetComponent<T>();
            }
            return default(T);
        }
        public static float GetDirection(this Vector2 v, Vector2 other)
        {
            if (v.x > other.x) return 1;
            if (v.x < other.y) return -1;
            return 0;
        }
        public static float GetDirection(this Vector2 v, Vector3 other)
        {
            if (v.x > other.x) return 1;
            if (v.x < other.y) return -1;
            return 0;
        }
        public static Vector2 CalculateDiection(this Vector2 v)
        {
            return new Vector2(
           Mathf.Sign(v.x),
            Mathf.Sign(v.y)
            );
        }

        public static T GetHitCollider<T>(this Vector2 pos)
        {
            // 1. Use GetRayIntersectionAll to detect 2D Colliders from a 3D Camera ray
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

            if (hits.Length == 0) return default(T);

            // 2. Map hits to sorting info
            var candidates = hits
                .Select(hit => new
                {
                    // hit.collider is a Collider2D
                    Component = hit.collider.GetComponent<T>(),
                    // distance is from the ray origin to the intersection point
                    Distance = hit.fraction,
                    SortingGroup = hit.collider.GetComponentInParent<SortingGroup>(),
                    Renderer = hit.collider.GetComponent<SpriteRenderer>()
                })
                .Where(c => c.Component != null)
                .ToList();

            if (candidates.Count == 0) return default(T);

            // 3. Sorting Logic
            var bestCandidate = candidates
                .OrderByDescending(c => c.SortingGroup != null ?
                    SortingLayer.GetLayerValueFromID(c.SortingGroup.sortingLayerID) :
                    (c.Renderer != null ? SortingLayer.GetLayerValueFromID(c.Renderer.sortingLayerID) : 0))
                .ThenByDescending(c => c.SortingGroup != null ? c.SortingGroup.sortingOrder :
                    (c.Renderer != null ? c.Renderer.sortingOrder : 0))
                .ThenBy(c => c.Distance)
                .FirstOrDefault();

            return bestCandidate != null ? bestCandidate.Component : default(T);
        }
        /// <summary>
        /// Detects all 2D components of type T at the given screen position.
        /// Optimized for 2.5D games using 3D Cameras and 2D Colliders.
        /// </summary>
        public static List<T> GetAllHitComponents<T>(this Vector2 pos)
        {
            List<T> results = new List<T>();

            // 1. Convert screen position (Mouse/Gamepad) to a 3D Ray
            Ray ray = Camera.main.ScreenPointToRay(pos);

            // 2. Use GetRayIntersectionAll to find where the 3D Ray hits 2D Colliders
            // This is the bridge between 3D Camera space and 2D Physics space
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

            if (hits == null || hits.Length == 0) return results;

            foreach (var hit in hits)
            {
                // 3. Search for the component on the hit GameObject
                // Note: Use GetComponents to capture multiple instances if they exist
                T[] components = hit.collider.GetComponents<T>();

                if (components != null && components.Length > 0)
                {
                    results.AddRange(components);
                }
            }

            return results;
        }
        /// <summary>
        /// This function will get all hit GameObjects located at specific Layers in a 2.5D environment.
        /// Compatible with 2D Colliders (BoxCollider2D, CircleCollider2D, etc.)
        /// </summary>
        /// <param name="pos">The screen position (Mouse or Hybrid Pointer).</param>
        /// <param name="layers">Array of layer names to filter the raycast.</param>
        /// <returns>A list of GameObjects hit by the ray, or an empty list if nothing hit.</returns>
        public static List<GameObject> GetAllHitComponents(this Vector2 pos, params string[] layers)
        {
            // 1. Basic validation
            if (layers == null || layers.Length == 0) return null;

            // 2. Create the bitmask for the specified layers
            int layerMask = LayerMask.GetMask(layers);

            // 3. Generate a 3D ray from the Camera through the screen point
            Ray ray = Camera.main.ScreenPointToRay(pos);

            // 4. Use Physics2D.GetRayIntersectionAll to detect 2D Colliders along that 3D Ray
            // This is the correct method for 2.5D interaction (3D Camera -> 2D Physics)
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity, layerMask);

            if (hits == null || hits.Length == 0) return new List<GameObject>();

            // 5. Extract GameObjects from the 2D hits
            List<GameObject> hitObjects = new List<GameObject>();
            foreach (var hit in hits)
            {
                // Check if the hit object is valid before adding
                if (hit.collider != null)
                {
                    hitObjects.Add(hit.collider.gameObject);
                }
            }

            return hitObjects;
        }
        public static T GetHitCollider<T>(this Vector2 pos, params string[] layers)
        {
            if(Camera.main==null) return default(T);
            // 1. Validate input and layers
            if (layers == null || layers.Length == 0) return default(T);

            // 2. Create the layer mask from names
            int layerMask = LayerMask.GetMask(layers);

            // 3. Generate a 3D ray from the perspective camera
            Ray ray = Camera.main.ScreenPointToRay(pos); 

            // 4. CRITICAL FIX: Use Physics2D.GetRayIntersectionAll for 2.5D
            // Standard Physics.RaycastAll ignores BoxCollider2D.
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity, layerMask);

            if (hits == null || hits.Length == 0) return default(T);

            // 5. Map hits to sorting data for 2.5D layering logic
            var candidates = hits
                .Select(hit =>
                {
                    // Find the component T on the hit collider
                    var component = hit.collider.GetComponent<T>();

                    // Get sorting info (Essential for overlapping sprites in 2.5D)
                    var sGroup = hit.collider.GetComponentInParent<SortingGroup>();
                    var sRenderer = hit.collider.GetComponent<SpriteRenderer>();

                    return new
                    {
                        Component = component,
                        // fraction represents the distance along the ray
                        Distance = hit.fraction,
                        LayerID = sGroup != null ? sGroup.sortingLayerID : (sRenderer != null ? sRenderer.sortingLayerID : 0),
                        OrderValue = sGroup != null ? sGroup.sortingOrder : (sRenderer != null ? sRenderer.sortingOrder : 0)
                    };
                })
                .Where(c => c.Component != null)
                .ToList();

            if (candidates.Count == 0) return default(T);

            // 6. Sorting Logic:
            // Priority 1: Sorting Layer Value (Actual stack order in Inspector)
            // Priority 2: Sorting Order (Z-index within the same layer)
            // Priority 3: Distance (Depth relative to camera)
            var bestCandidate = candidates
                .OrderByDescending(c => SortingLayer.GetLayerValueFromID(c.LayerID))
                .ThenByDescending(c => c.OrderValue)
                .ThenBy(c => c.Distance)
                .FirstOrDefault();

            return bestCandidate != null ? bestCandidate.Component : default(T);
        }
        /// <summary>
        /// Tạo 4 điểm của hình chữ nhật quanh tâm dựa trên kích thước size.
        /// Thứ tự các điểm: Top-Left, Top-Right, Bottom-Right, Bottom-Left (Theo chiều kim đồng hồ hoặc ngược lại)
        /// </summary>
        public static Vector2[] CreateRectFromCenter(this Vector2 center, Vector2 size)
        {
            return ((Vector3)center).CreateRectFromCenter(size);
        }
        /// <summary>
        /// Tạo 4 điểm của hình chữ nhật quanh tâm dựa trên kích thước size.
        /// Thứ tự các điểm: Top-Left, Top-Right, Bottom-Right, Bottom-Left (Theo chiều kim đồng hồ hoặc ngược lại)
        /// </summary>
        public static Vector2[] CreateRectFromCenter(this Vector3 center, Vector2 size)
        {
            float halfWidth = size.x / 2f;
            float halfHeight = size.y / 2f;

            Vector2[] points = new Vector2[4];

            // Top Left
            points[0] = new Vector2(center.x - halfWidth, center.y + halfHeight);

            // Top Right
            points[1] = new Vector2(center.x + halfWidth, center.y + halfHeight);

            // Bottom Right
            points[2] = new Vector2(center.x + halfWidth, center.y - halfHeight);

            // Bottom Left
            points[3] = new Vector2(center.x - halfWidth, center.y - halfHeight);

            return points;
        }
        public static T ScanObject<T>(this Vector3 scanPoint, Vector2 Scope, string[] layers)
        {
            return ScanObject<T>((Vector2)scanPoint, Scope, layers);
        }
        public static T ScanObject<T>(this Vector2 scanPoint,Vector2 Scope, string[] layers)
        {
            // 1. Lấy vị trí tâm của nhân vật (hoặc một điểm phía trước mặt nhân vật)
            

          
            // 2. Quét tất cả các Collider2D nằm trong vùng hình hộp và thuộc LayerMask

            Collider2D[] results = Physics2D.OverlapBoxAll(scanPoint, Scope, 0f, LayerMask.GetMask(layers));
            var colls = results.Where(p => p.GetComponent<T>() != null).ToArray();


            // 3. Xử lý các đối tượng tìm được
            if (colls.Length > 0)
            {
                // Thường chúng ta sẽ lấy vật thể gần nhất
                GameObject closestGO = scanPoint.GetClosestTarget(colls);
                return closestGO.GetComponent<T>();

            }
            else if (results.Length == 1)
            {
                return results[0].GetComponent<T>();
            }
            return default(T);
        }
        
    }
}