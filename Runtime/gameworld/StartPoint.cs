
using UnityEngine;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;


namespace unvs.gameword
{
    [ExecuteAlways]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class StartPoint : MonoBehaviour, ISpawnTarget
    {
        public BoxCollider2D coll;
        public SpriteRenderer sprite;


        //private Mesh circleMesh;
        //private Material redMaterial;

        public Vector2 Pos
        {
            get
            {
                var edge = coll.FindBottomEdge(unvs.shares.Constants.Layers.SURFACE);
                if (edge == null) edge = GetComponentInParent<IScenePrefab>(true).Floor;

                return edge.GetIntersetPoint(coll.bounds.center.x);
            }
        }

        public Collider2D Coll => coll;

        public SpriteRenderer Renderer => sprite;

        public string Name => name;

        public bool IsTempEntrance { get ; set ; }

        private void Awake()
        {

            coll = GetComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }
        private void Start()
        {

            gameObject.HideWhenPlaying();
        }
        private void OnValidate()
        {
            if (sprite == null || sprite.sprite==null)
            {


                sprite = GetComponent<SpriteRenderer>();
                sprite.sprite = Commons.LoadAsset<Sprite>("Packages/com.unvs.core/Runtime/Sprites/Circle.png");
                //sprite.LoadImage("Packages/com.unvs.core/Runtime/Sprites/Circle.png");


            }
        }
#if UNITY_EDITOR
        //private void OnRenderObject()
        //{
        //    // Chỉ vẽ trong cửa sổ Scene của Editor (hoặc lúc không Play)
        //    // Nếu muốn hiện cả lúc Play thì bỏ điều kiện !Application.isPlaying
        //    if (Application.isPlaying) return;

        //    // 1. Khởi tạo tài nguyên
        //    if (coll == null) coll = GetComponent<BoxCollider2D>();
        //    if (circleMesh == null) circleMesh = CreateEllipseMesh();
        //    if (redMaterial == null) redMaterial = new Material(Shader.Find("Sprites/Default"));

        //    // 2. Thiết lập màu đỏ (Alpha 0.5 để nhìn xuyên thấu được)
        //    redMaterial.color = new Color(1f, 0f, 0f, 0.5f);
        //    redMaterial.SetPass(0);

        //    // 3. Tính toán Ma trận biến đổi để khớp với Collider
        //    // Kết hợp vị trí Object + Offset của Collider + Size của Collider
        //    Matrix4x4 matrix = transform.localToWorldMatrix * Matrix4x4.TRS(coll.offset, Quaternion.identity, new Vector3(coll.size.x, coll.size.y, 1f));

        //    // 4. Vẽ trực tiếp lên màn hình
        //    Graphics.DrawMeshNow(circleMesh, matrix);
        //}
#endif
        // Hàm tạo Mesh hình tròn đơn vị (radius = 0.5)
        private Mesh CreateEllipseMesh()
        {
            Mesh mesh = new Mesh();
            int segments = 36;
            Vector3[] vertices = new Vector3[segments + 1];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < segments; i++)
            {
                float rad = i * Mathf.Deg2Rad * (360f / segments);
                vertices[i + 1] = new Vector3(Mathf.Cos(rad) * 0.5f, Mathf.Sin(rad) * 0.5f, 0);

                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = (i + 1) % segments + 1;
                triangles[i * 3 + 2] = i + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            return mesh;
        }

        public void MoveOtherToMe(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.transform.position = this.Pos;
        }
    }
}