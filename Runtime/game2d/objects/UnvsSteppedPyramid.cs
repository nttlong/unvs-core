using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using unvs.ext;
using unvs.game2d.scenes;
using unvs.shares;
#if UNITY_EDITOR
using unvs.shares.editor;
using unvs.core.editorlibs;
using System.Linq;
#endif

namespace game2d.objects
{
    //[RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class UnvsSteppedPyramid : UnvsBaseComponent
    {
        [Header("Visual shape")]
        public float stepHeight;
        public float stepWidth;
        
    }
#if UNITY_EDITOR
    public partial class UnvsSteppedPyramid : UnvsBaseComponent
    {

        private void OnDrawGizmos()
        {
            this.SetMeOnLayer(Constants.Layers.WORLD_GROUND);
            this.SetMeOnTag(Constants.Tags.STAIR);
            var sp = GetComponent<SpriteRenderer>();
            if (sp != null)
            {
                sp.ApplyDefaultBox();
            }
        }
        [UnvsButton("Create from left")]
        public void EditorCreateShape()
        {
            
                var spr = GetComponent<SpriteRenderer>();
            if (spr == null) return;

           
            var polyColl = this.AddComponentIfNotExist<PolygonCollider2D>();

            var stepWidth = this.stepWidth / transform.lossyScale.x;
            var stepHeight = this.stepHeight / transform.lossyScale.y;
            float totalWidth = spr.bounds.size.x / transform.lossyScale.x;
            float totalHeight = spr.bounds.size.y / transform.lossyScale.y;

            // Tính số lượng bậc thang tối đa dựa trên chiều cao sprite
            int numSteps = Mathf.FloorToInt(totalHeight / stepHeight);

            List<Vector2> points = new List<Vector2>();
            var offset= new Vector2(-totalWidth/2, -totalHeight/2);
            // 1. Tạo các bậc thang từ dưới lên trên (phía bên trái)
            // Bắt đầu từ tọa độ (0, 0) - giả định pivot nằm ở góc dưới
            for (int i = 0; i <= numSteps; i++)
            {
                float currentY = (i * stepHeight);
                float currentX = i * stepWidth ;

                // Nếu chiều rộng vượt quá giới hạn sprite thì dừng
                if (currentX > totalWidth) currentX = totalWidth;
                if (currentY > totalHeight) currentY = totalHeight;

                // Điểm ngang (tạo mặt bậc)
                points.Add(new Vector2(currentX, currentY) + offset);

                // Điểm đứng (tạo cổ bậc) - không thêm điểm này ở bậc cuối cùng
                if (i < numSteps && (currentY + stepHeight) <= totalHeight)
                {
                    points.Add(new Vector2(currentX, currentY + stepHeight)+ offset);
                }
            }

            // 2. Chốt các điểm phía bên phải để làm "tường đứng"
            // Điểm góc trên bên phải
            points.Add(new Vector2(totalWidth, totalHeight) + offset);
            // Điểm góc dưới bên phải
            points.Add(new Vector2(totalWidth, 0) + offset);

            // Gán danh sách điểm vào Collider
            polyColl.SetPath(0, points.ToArray());
        }
        [UnvsButton("Export to Png file")]
        public async UniTask EditorExportPng()
        {
            var check = await unvs.core.editorlibs.UnvsPythonCall.HealthCheck();
            if (check)
            {
                unvs.core.editorlibs.Dialogs.Show("OK");
            }
            else
            {
                unvs.core.editorlibs.Dialogs.Show("Fail");
                return;
            }
           
            var scene = this.GetComponentInParent<UnvsScene>();
            if (scene == null || scene.selRef == null) return;

            var pathToAsset = UnvsEditorUtils.EditorGetAddressPath(scene.selRef);
            if (string.IsNullOrEmpty(pathToAsset)) return;

            var folder = System.IO.Path.GetDirectoryName(pathToAsset);
            var subFolder = System.IO.Path.Combine(folder, $"{scene.gameObject.name}-sprites");

            if (!System.IO.Directory.Exists(subFolder))
                System.IO.Directory.CreateDirectory(subFolder);

            var poly = GetComponent<PolygonCollider2D>();
            if (poly == null) return;
            
            
            Vector2[] points = poly.points;

            // 1. Tính toán Bounds của các điểm để xác định size ảnh
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;
            foreach (var p in points)
            {
                if (p.x < minX) minX = p.x; if (p.x > maxX) maxX = p.x;
                if (p.y < minY) minY = p.y; if (p.y > maxY) maxY = p.y;
            }

            // Thêm một chút padding (lề) để đường line không dính sát mép ảnh
            int padding = 10;
            int width = Mathf.CeilToInt((maxX - minX) * 100) + (padding * 2);
            int height = Mathf.CeilToInt((maxY - minY) * 100) + (padding * 2);

            // 2. Khởi tạo Texture
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            // Fill nền trong suốt
            Color[] fill = new Color[width * height];
            for (int i = 0; i < fill.Length; i++) fill[i] = Color.clear;
            tex.SetPixels(fill);

            // 3. Hàm vẽ Line Helper (Bresenham đơn giản)
            void DrawLine(Vector2 p1, Vector2 p2)
            {
                // Chuyển tọa độ Local sang tọa độ Pixel ảnh
                Vector2 start = new Vector2((p1.x - minX) * 100 + padding, (p1.y - minY) * 100 + padding);
                Vector2 end = new Vector2((p2.x - minX) * 100 + padding, (p2.y - minY) * 100 + padding);

                float steps = Mathf.Max(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
                for (int i = 0; i <= steps; i++)
                {
                    float t = (steps == 0) ? 0 : i / steps;
                    Vector2 res = Vector2.Lerp(start, end, t);
                    tex.SetPixel((int)res.x, (int)res.y, Color.white); // Vẽ đường màu trắng
                }
            }

            // 4. Vẽ các cạnh của Polygon
            for (int i = 0; i < points.Length; i++)
            {
                DrawLine(points[i], points[(i + 1) % points.Length]);
            }

            tex.Apply();

            // 5. Lưu ra file
            byte[] bytes = tex.EncodeToPNG();
            var fullPath = System.IO.Path.Combine(subFolder, $"{gameObject.name}.png");
            var fullPathPsd = System.IO.Path.Combine(subFolder, $"{gameObject.name}.psd");
            System.IO.Path.GetFullPath(fullPath);
            System.IO.File.WriteAllBytes(fullPath, bytes);

            Debug.Log($"Đã xuất file tại: {fullPath}");

            // Giải phóng bộ nhớ
            Object.DestroyImmediate(tex);
            UnityEditor.AssetDatabase.Refresh();
            await unvs.core.editorlibs.UnvsPythonCall.Call("UnvsPsd", "CreatePsdFile", new
            {
                FilePath= unvs.core.editorlibs.UnvsPythonCall.ToAbsolutePath( fullPathPsd),
                PngFile= unvs.core.editorlibs.UnvsPythonCall.ToAbsolutePath(fullPath),
                Points= points.Select(p=>new
                {
                    x=p.x, y=p.y,
                }).ToList(),
            });
        }
    }
#endif
}