using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using unvs.ext;
//using unvs.gameword.manager;
using unvs.interfaces;
using unvs.manager;
using unvs.shares;
namespace unvs.gameword
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TriggerZoneObject : MonoBehaviour, ITriggerZone
    {
        public string triggerPath;
        public BoxCollider2D coll;
        public TriggerZoneDirection direction;
        private bool isOff;

        public string TriggerPath => triggerPath;

        public BoxCollider2D Coll
        {
            get
            {
                if(coll == null)
                {
                    coll = GetComponent<BoxCollider2D>();
                    coll.isTrigger=true;
                    return coll;
                }
                return coll;
            }
        }

        public TriggerZoneDirection Direction { get => direction; set => direction = value; }
        void Awake()
        {
            if (string.IsNullOrEmpty(triggerPath))
            {
                var es = GetComponentInParent<IElasticScene>();
                triggerPath = (direction == TriggerZoneDirection.Left) ? es.LeftScenePath : es.RightScenePath;
            }
        }
        private void Start()
        {
            coll = GetComponent<BoxCollider2D>();
            if (coll != null)
                coll.isTrigger = true;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnTriggerEnter2D(Collider2D other)
        {
            
            if (isOff)
            {
                return;
            }
            if (GetComponentInParent<IScenePrefab>().IsDestroying) return;
            
            var cam = other.GetComponentInParent<ICam>();
          
            if (cam != null)
            {

                if (string.IsNullOrEmpty(triggerPath))
                {
                    var parent = GetComponentInParent<IScenePrefab>();

                    if (parent != null)
                    {
                        if (direction == TriggerZoneDirection.Left)
                        {
                            triggerPath = parent.GetLeftScenePath();
                        }
                        if (direction == TriggerZoneDirection.Right)
                        {
                            triggerPath = parent.GetRightScenePath();
                        }
                    }
                }

                this.Off();
                IScenePrefab ret = null;
                if (string.IsNullOrEmpty(triggerPath)) return;
                GlobalApplication.SceneLoaderManagerInstance.LoadChunksAsync(this)
                    .ContinueWith(p =>
                    {
                        ret = p;
                    })
                    .Forget();
                
                //ChunkSceneLoaderUtilsOld.LoadSceneAsync(this.GetComponent<ITriggerZone>())
                //    .ContinueWith(() =>
                //    {

                //    }).Forget();

            }

        }
        public static void CreateLimitColliderDelete(GameObject goWorld, Collider2D boundingShape2D, out ITriggerZone leftZone, out ITriggerZone rightZone)
        {
            if (boundingShape2D == null)
            {
                leftZone = null;
                rightZone = null;
                return;
            }

            Bounds bounds = boundingShape2D.bounds;
            float centerY = bounds.center.y;
            float height = bounds.size.y;
            float thickness = 1f;

            // Tìm hoặc tạo chốt bên Trái
            leftZone = GetOrCreateTriggerZone(goWorld, TriggerZoneDirection.Left,
                new Vector2(bounds.min.x - (thickness / 2f), centerY),
                new Vector2(thickness, height));

            // Tìm hoặc tạo chốt bên Phải
            rightZone = GetOrCreateTriggerZone(goWorld, TriggerZoneDirection.Right,
                new Vector2(bounds.max.x + (thickness / 2f), centerY),
                new Vector2(thickness, height));
        }
        public static ITriggerZone GetOrCreateTriggerZone(GameObject parent, TriggerZoneDirection direction, Vector2 pos, Vector2 size)
        {
            // 1. Tìm trong các con xem có ai là IWall và đúng WallType không
            var name = $"Zone-{direction}";

            var ret = parent.GetComponentInChildrenByName<TriggerZoneObject>(name);


            GameObject zone;

            if (ret == null)
            {
                // 2. Nếu chưa có thì tạo mới
                zone = new GameObject(name);
                zone.transform.SetParent(parent.transform);
                ret = zone.AddComponent<TriggerZoneObject>();

            }
            else
            {
                // 3. Nếu có rồi thì lấy GameObject đó để cập nhật
                zone = ret.gameObject;
            }

            // 4. Cập nhật thông số vật lý (vị trí và kích thước)
            zone.transform.position = pos;

            var box = zone.GetComponent<BoxCollider2D>();
            if (box == null) box = zone.AddComponent<BoxCollider2D>();

            box.size = size;
            box.isTrigger = true;
            ////// Đảm bảo là Static để nhân vật không đẩy được tường
            //if (wallGo.TryGetComponent<Rigidbody2D>(out var rb))
            //{
            //    rb.bodyType = RigidbodyType2D.Static;
            //}
            ret.direction = direction;
            return ret;
        }

        public void Off()
        {
            this.gameObject.SetActive(false);
            this.enabled = false;
            this.isOff = true;
        }

        public void On()
        {
            this.gameObject.SetActive(true);
            this.enabled = true;
            this.isOff = false;
        }
    }
}
