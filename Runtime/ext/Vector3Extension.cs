using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using unvs.interfaces;

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
        public static bool IsOverObject(this Vector2 mouseWorldPos, out GameObject hitObject, string LayerName, params string[] LayerNames)
        {
            var mask = GetLayerMaskFromCache(LayerName, LayerNames);
            return mouseWorldPos.IsOverObject(out hitObject, mask);
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

        public static IScenePrefab GetNearsetScene(this Vector3 position, IScenePrefab[] _scenesList)
        {
            if (_scenesList.Length == 0) return null;
            if (_scenesList.Length == 1) return _scenesList[0];

            IScenePrefab nearest = null;
            float maxInsideDistance = float.MinValue; // Càng lớn nghĩa là càng nằm sâu trong Scene
            Vector2 currentPos = position;

            foreach (var sc in _scenesList.Where(p => !p.GoWorld.IsDestroyed()))
            {
                if (sc == null || sc.LeftWall == null || sc.RightWall == null) continue;

                // Lấy biên phải của tường trái và biên trái của tường phải
                float leftBoundary = sc.LeftWall.bounds.max.x;
                float rightBoundary = sc.RightWall.bounds.min.x;

                // Nếu currentPos.x nằm giữa leftBoundary và rightBoundary:
                // Ta tính xem nó cách biên gần nhất là bao nhiêu.
                // Giá trị này càng lớn thì đối tượng càng ở gần "tâm ngang" của Scene.
                float distFromLeft = currentPos.x - leftBoundary;
                float distFromRight = rightBoundary - currentPos.x;

                // Khoảng cách an toàn tối thiểu so với cả 2 biên
                float internalDist = Mathf.Min(distFromLeft, distFromRight);

                if (internalDist > maxInsideDistance)
                {
                    maxInsideDistance = internalDist;
                    nearest = sc;
                }
            }
            return nearest;
        }

        public static GameObject GetObjectInLayer(this Vector2 point,int LayerMask)
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
        public static Vector2 CalculateDiection(this Vector2 v)
        {
            return new Vector2(
            v.x == 0 ? 0 : Mathf.Sign(v.x),
            v.y == 0 ? 0 : Mathf.Sign(v.y)
            );
        }
    }
}