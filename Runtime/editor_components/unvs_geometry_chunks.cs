#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using unvs.game2d.scenes;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.IO;
namespace unvs.editor.components     {

    
    public class UnvsGeometryChunks : UnvsBaseComponent
    {
        public PolygonCollider2D coll;

        public int ScreenWidth;

        public int ScreeHeight;
        public int numOfCols;
        public int numOfRows;
        public EditorVector2[] Points;
        public UnvsScene scene;
        [SerializeField]
        public SceneInfoResut Folder;
        public string folderPath;

        private void OnValidate()
        {
            coll = this.GetComponent<PolygonCollider2D>();
            ScreenWidth =(int)( coll.bounds.size.x );
            ScreeHeight=(int)( coll.bounds.size.y );
            numOfCols = ScreenWidth / 2048;
            numOfRows = ScreeHeight / 2048;
            Points= coll.points.Select(p=>new EditorVector2
            {
                x=(int) p.x, y=(int) p.y,
            }).ToArray();
            
        }
        private void OnDrawGizmos()
        {
            
            Folder = unvs.core.editorlibs.EditorTools.GetFolderOfGameObjectByScene(gameObject);
        }
        [UnvsButton("Create psd file")]
        public async UniTask CreatePSDFile()
        {
            if (!await unvs.core.editorlibs.UnvsPythonCall.HealthCheck())
            {
                return;
            }
            Folder = unvs.core.editorlibs.EditorTools.GetFolderOfGameObjectByScene(gameObject);
            folderPath = Folder.FolderPath;
            var subFolderPath=System.IO.Path.Combine(folderPath,$"{name}-sprite-{numOfRows}X{numOfCols}");
            if (!Directory.Exists(subFolderPath))
            {
                Directory.CreateDirectory(subFolderPath);
            }
            var colliderPoints = this.GetComponent<PolygonCollider2D>().points;
            
            // 1. Tìm Min X và Min Y để tính Offset
            float minX = colliderPoints.Min(p => p.x);
            float minY = colliderPoints.Min(p => p.y);

            // 2. Tính Width và Height thực tế dựa trên biên độ điểm
            float width = colliderPoints.Max(p => p.x) - minX;
            float height = colliderPoints.Max(p => p.y) - minY;

            var data = new
            {
                // Screen size khớp khít với bao cảnh của các điểm
                screen_width = (int)Mathf.Ceil(width),
                screen_height = (int)Mathf.Ceil(height),
                split_width = 512,

                // 3. Chuẩn hóa điểm: Trừ đi min để đưa về tọa độ dương (0 -> dương)
                points = colliderPoints.Select(p => new
                {
                    // Ta dùng float để chính xác, hoặc (int) sau khi đã cộng offset
                    x = (int)(p.x - minX),
                    y = (int)(p.y - minY)
                }).ToArray(),

                folder_path = subFolderPath,
                file_name = $"{name}-sprite-{numOfRows}X{numOfCols}.psd"
            };
            await unvs.core.editorlibs.UnvsPythonCall.Call("UnvsPsd", "CreatePsdBigSise", data);
        }
    }

}
    
#endif