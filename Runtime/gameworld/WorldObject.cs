
using System;
using System.Linq;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.gameword
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ElasticScene))]
    [RequireComponent(typeof(WorldGlobalLight))]
    [RequireComponent(typeof(AudioSource))]
    public class WorldObject : MonoBehaviour, IScenePrefab
    {

        WorldJoinInfoObject joinInfo;

        public IWorldGlobalLight Globalight => GetComponent<IWorldGlobalLight>();

        public IWorldJoinInfoObject JoinInfo
        {
            get
            {
                if (joinInfo != null) return joinInfo;
                joinInfo = this.GetComponentInChildrenByName<WorldJoinInfoObject>(Constants.ObjectsConst.WORLD_INFO);

                return joinInfo;
            }
        }
        public string Name => name;
        public Transform actor;
        public IActorObject CurrentActor;
        public ISpawnTarget _startPos;
        public Transform startPos;
        [Header("Camera")]
        [SerializeField]
        public OffsetFollow cameraOffsetFolow = OffsetFollow.DefaultNew();
        public float orthographicSize = Constants.CinemachineDefaut.OrthographicSize;

        public EdgeCollider2D floor;

        public bool isReszieWorldBound;


        [SerializeField]
        public ScenePrefabWorldBound worldBound;
        public EdgeCollider2D groundPhysics;
        public PolygonCollider2D coll;
        [Header("Physical")]
        public PhysicsMaterial2D defaultSurfacePhysicsMaterial2D;
        [Header("Scene settings")]
        [SerializeField]
        public ViewInfo view;
        public GameObject defaultCamWatcher;
        public OffsetFollow CameraOffsetFolow => cameraOffsetFolow;
        public AudioSource audioSource;
        [SerializeField]
        public AudioInfo ambientSound = AudioInfo.EmptyNew();

        public BoxCollider2D leftWall;
        public BoxCollider2D rightWall;
        public ITriggerZone leftTriggerZone;
        public BoxCollider2D collTriggerLeft;
        public ITriggerZone rightTriggerZone;
        public BoxCollider2D collTriggerRight;
        public GameObject DefaultCamWatcher => defaultCamWatcher;

        [Header("Scene-link")]
        public IScenePrefab left;
        public Transform leftTr;
        public IScenePrefab right;
        public Transform rightTr;
        [Header("World info")]
        public int indexOfMaxEdge;
        public int indexOfMinEdge;
        public Transform trWorldJoinInfoObject;
        [SerializeField]
        public Egde2d[] Edges;
        private bool hastrimEdge;
        private IWorldTracker workTracker;
        public bool gizmosDraw = true;
        private System.Action<IScenePrefab> onDestroyMe;
        [SerializeField]
        public bool isDefaultCamWatcherSettings;
        private bool _defaultCamWatcherReady;



        //private Transform runtimeRightPosJoin;
        //private Transform runtimeLeftPosJoin;

        public event System.Action<IScenePrefab> OnReachLimit;
        public event System.Action OnSceneDestroy;

        public GameObject GoWorld
        {
            get
            {
                if (this.IsDestroyed()) return null;
                return this.gameObject;
            }
        }

        public IActorObject Actor
        {
            get
            {
                if (CurrentActor != null) return CurrentActor;
                CurrentActor = GetComponentInChildren<IActorObject>(true);
                if (CurrentActor != null)
                {
                    actor = (CurrentActor as MonoBehaviour)?.gameObject.transform;
                }
                return CurrentActor;
            }
        }

        public ISpawnTarget StartPos
        {
            get
            {
                if (_startPos != null) return _startPos;
                _startPos = this.GetComponentInChildrenByName<ISpawnTarget>(Constants.ObjectsConst.START_POINT);
                if (_startPos != null)
                {
                    startPos = (_startPos as MonoBehaviour)?.gameObject.transform;
                }
                return _startPos;
            }
        }



        public float OrthographicSize => orthographicSize;





        public EdgeCollider2D Floor => floor;

        public BoxCollider2D LeftWall
        {
            get
            {
                if (Application.isPlaying) return leftWall;
                if (leftWall != null) return leftWall;
                leftWall = this.GetComponentInChildrenByName<BoxCollider2D>(Constants.ObjectsConst.LEFT_WALL_NAME);

                if (leftWall == null)
                {
                    leftWall = transform.Create<BoxCollider2D>(Constants.ObjectsConst.LEFT_WALL_NAME);
                }
                return leftWall;
            }
        }

        public BoxCollider2D RightWall
        {
            get
            {
                if (Application.isPlaying) return rightWall;
                if (rightWall != null) return rightWall;
                rightWall = this.GetComponentInChildrenByName<BoxCollider2D>(Constants.ObjectsConst.RIGHT_WALL_NAME);
                if (rightWall == null)
                {
                    rightWall = transform.Create<BoxCollider2D>(Constants.ObjectsConst.RIGHT_WALL_NAME);
                }
                return rightWall;
            }
        }

        public bool IsReszieWorldBound => isReszieWorldBound;







        public IScenePrefabWorldBound WorldBound
        {
            get
            {
                if (worldBound == null && GlobalWorldBound.Instance != null)
                {
                    foreach (var p in GlobalWorldBound.Instance.coll.gameObject.GetComponentsInChildren<ScenePrefabWorldBound>())
                    {
                        if (p.Owner == this as IScenePrefab)
                        {
                            worldBound = p.GetComponent<ScenePrefabWorldBound>();
                        }

                    }
                }
                return worldBound;
            }
        }

        private void CreateWorldBound()
        {
            if (Application.isPlaying) return;
            worldBound = this.GetComponentInChildrenByName<ScenePrefabWorldBound>(Constants.ObjectsConst.MY_WORLD_BOUND);
            if (worldBound == null)
            {
                var tempGO = new GameObject(Constants.ObjectsConst.MY_WORLD_BOUND);
                tempGO.transform.SetParent(transform);
                worldBound = tempGO.AddComponent<ScenePrefabWorldBound>();
                //(worldBound as MonoBehaviour).AddComponentIfNotExist<PolygonCollider2D>();
                _ = worldBound.Coll;

            }
            worldBound.Owner = this;
        }
        void CreateInfo()
        {
            if (Application.isPlaying) return;
            joinInfo = this.GetComponentInChildrenByName<WorldJoinInfoObject>(Constants.ObjectsConst.WORLD_JOIN_INFO);
            if (joinInfo == null)
            {
                var go = new GameObject(Constants.ObjectsConst.WORLD_JOIN_INFO);
                joinInfo = go.AddComponent<WorldJoinInfoObject>();
                go.transform.SetParent(transform);
                trWorldJoinInfoObject = go.transform;
            }
            else
            {
                trWorldJoinInfoObject = (joinInfo as MonoBehaviour).transform;
            }
        }


        public ITriggerZone LeftTriggerZone
        {
            get
            {
                if (Application.isPlaying)
                {
                    if (leftTriggerZone == null)
                    {
                        leftTriggerZone = this.GetComponentInChildrenByName<TriggerZoneObject>(Constants.ObjectsConst.TRGIGER_LEFT_NAME);
                        leftTriggerZone.Direction = TriggerZoneDirection.Left;
                        return leftTriggerZone;
                    }
                    else
                    {
                        return leftTriggerZone;
                    }
                }
                if (leftTriggerZone != null)
                {
                    collTriggerLeft = leftTriggerZone.Coll;
                    return leftTriggerZone;
                }
                leftTriggerZone = transform.CreateIfNoExist<TriggerZoneObject>(Constants.ObjectsConst.TRGIGER_LEFT_NAME);
                leftTriggerZone.Direction = TriggerZoneDirection.Left;
                collTriggerLeft = leftTriggerZone.Coll;
                return leftTriggerZone;
            }
        }

        public ITriggerZone RightTriggerZone
        {
            get
            {
                if (Application.isPlaying)
                {
                    if (rightTriggerZone == null)
                    {
                        rightTriggerZone = this.GetComponentInChildrenByName<TriggerZoneObject>(Constants.ObjectsConst.TRGIGER_RIGHT_NAME);
                        rightTriggerZone.Direction = TriggerZoneDirection.Right;
                        return rightTriggerZone;
                    }
                    else
                    {
                        return rightTriggerZone;
                    }
                }
                if (rightTriggerZone != null)
                {
                    collTriggerRight = rightTriggerZone.Coll;
                    return rightTriggerZone;
                }
                rightTriggerZone = transform.CreateIfNoExist<TriggerZoneObject>(Constants.ObjectsConst.TRGIGER_RIGHT_NAME);
                rightTriggerZone.Direction = TriggerZoneDirection.Right;
                collTriggerRight = rightTriggerZone.Coll;
                return rightTriggerZone;
            }
        }

        //public PolygonCollider2D RefColliderWorldBound { get => refColliderWorldBound; set => refColliderWorldBound=value; }
        public IScenePrefab Left
        {
            get => left; set
            {
                left = value;
                leftTr = value?.GoWorld.transform;
            }
        }
        public IScenePrefab Right
        {
            get => right; set
            {
                right = value;
                rightTr = value?.GoWorld.transform;
            }
        }




        public System.Action<IScenePrefab> OnDestroyMe { get => onDestroyMe; set => onDestroyMe = value; }
        public bool IsDestroying { get; set; }

        public PhysicsMaterial2D FloorMaterial
        {
            get => defaultSurfacePhysicsMaterial2D;
            set
            {
                defaultSurfacePhysicsMaterial2D = value;
                if (groundPhysics != null)
                    groundPhysics.sharedMaterial = this.defaultSurfacePhysicsMaterial2D;
            }
        }

        public IWorldTracker WorkTracker
        {
            get
            {
                if (workTracker != null) return workTracker;
                workTracker = this.GetComponentInChildren<IWorldTracker>(true);
                return workTracker;
            }
        }

        public bool GizmosDraw { get => gizmosDraw; }

        public AudioInfo Ambient => ambientSound;

        public AudioSource Audio => audioSource;

        public ViewInfo View => view;

        /// <summary>
        /// Calculate all positions of all objects of scene before runtime
        /// </summary>

        void CalculateBound()
        {

            Edges = this.floor.ToEdge2dArray();

            if (joinInfo != null)
            {
                joinInfo.worldJoinInfo =
                Edges.Calculate(
                    this.LeftWall.bounds.max.x,
                    this.RightWall.bounds.min.x,
                    this.floor.transform // Truyền transform để chuyển sang World Space
                );
                joinInfo.worldJoinInfo.LeftGroundIndex = this.worldBound.coll.GetMostVerticalEdgeAtMinX();
                joinInfo.worldJoinInfo.RightGroundIndex = this.worldBound.coll.GetMostVerticalEdgeAtMaxX();
                joinInfo.worldJoinInfo.Center = this.worldBound.coll.bounds.center;
                joinInfo.worldJoinInfo.Max = this.worldBound.coll.bounds.max;
                joinInfo.worldJoinInfo.Min = this.worldBound.coll.bounds.min;
                joinInfo.worldJoinInfo.WorldFacets = new WorldBoundFacets();
                var start = -1;
                var end = -1;
                this.worldBound.coll.LeftVerticalFacet(out start, out end);
                joinInfo.worldJoinInfo.WorldFacets.Left = new FacetInfo
                {
                    End = end,
                    Start = start,
                };
                this.worldBound.coll.RightVerticalFacet(out start, out end);
                joinInfo.worldJoinInfo.WorldFacets.Right = new FacetInfo
                {
                    End = end,
                    Start = start,
                };
                this.worldBound.coll.TopHorizontalFacet(out start, out end);
                joinInfo.worldJoinInfo.WorldFacets.Top = new FacetInfo
                {
                    End = end,
                    Start = start,
                };
                this.worldBound.coll.BootomHorizontalFacet(out start, out end);
                joinInfo.worldJoinInfo.WorldFacets.Bottom = new FacetInfo
                {
                    End = end,
                    Start = start,
                };
            }

            //var (v1, v2) = this.floor.CalculateIntersection(this.leftWall.bounds.max.x, this.rightWall.bounds.min.x);

        }
        void Awake()
        {

            audioSource = GetComponent<AudioSource>();
            _ = this.LeftTriggerZone;
            _ = this.RightTriggerZone;
            if (Application.isPlaying)
            {
                if (defaultCamWatcher != null && defaultCamWatcher.GetComponent<SpriteRenderer>() != null)
                    defaultCamWatcher.GetComponent<SpriteRenderer>().enabled = false;
                worldBound = this.GetComponentInChildrenByName<ScenePrefabWorldBound>(Constants.ObjectsConst.WORLD_BOUND);
                groundPhysics = this.GetComponentInChildrenByName<EdgeCollider2D>(Constants.ObjectsConst.GROUND_PHYSICAL);
                groundPhysics.SetMeOnLayer(Constants.Layers.SURFACE);
                groundPhysics.sharedMaterial = this.defaultSurfacePhysicsMaterial2D;
                joinInfo = this.GetComponentInChildrenByName<WorldJoinInfoObject>(Constants.ObjectsConst.WORLD_JOIN_INFO);
                StartPos.Coll.gameObject.SetActive(false);
                (StartPos as MonoBehaviour).gameObject.SetActive(false);
                if (joinInfo == null) throw new Exception($"WorldJoinInfoObject is empty in {name}");
                trWorldJoinInfoObject = (joinInfo as MonoBehaviour).transform;
            }


            else
            {
                CreateWorldBound();
                CreateInfo();
                if (joinInfo == null) throw new Exception($"WorldJoinInfoObject is empty in {name}");
                trWorldJoinInfoObject = (joinInfo as MonoBehaviour).transform;
                if (defaultCamWatcher == null)
                {
                    defaultCamWatcher = this.AddChildComponentIfNotExist<MonoBehaviour>(Constants.ObjectsConst.DEFAULT_CAM_WATCHER).gameObject;

                    _defaultCamWatcherReady = true;

                }
                else
                {
                    _defaultCamWatcherReady = true;
                }
                //CreateLeftRightTrigger();
            }
        }
        void Start()
        {

            _ = this.Actor;
            _ = this.StartPos;






            leftWall = this.GetComponentInChildrenByName<BoxCollider2D>(Constants.ObjectsConst.LEFT_WALL_NAME);

            rightWall = this.GetComponentInChildrenByName<BoxCollider2D>(Constants.ObjectsConst.RIGHT_WALL_NAME);

            if (leftWall == null && rightWall == null)
            {
                IWall lw = null;
                IWall rw = null;
                WallObject.CreateLimitCollider(this.gameObject, coll, out lw, out rw);
                leftWall = lw.Coll;
                rightWall = rw.Coll;
            }


            var tr = this.GetComponentInChildrenByName<Transform>(Constants.ObjectsConst.GROUND_PHYSICAL);
            if (tr != null)
            {
                tr.gameObject.SetMeOnLayer(Constants.Layers.SURFACE);
            }
        }



        void CreateChildGround()
        {

            // 1. Tạo một GameObject mới
            GameObject groundChild = new GameObject(unvs.shares.Constants.ObjectsConst.GROUND_PHYSICAL);


            // 2. Đặt nó làm con của đối tượng hiện tại (WorldObject)
            groundChild.transform.SetParent(this.transform);

            // 3. Reset vị trí về (0,0,0) so với cha
            groundChild.transform.localPosition = Vector3.zero;

            // 4. Thêm EdgeCollider2D vào đối tượng con vừa tạo
            floor = groundChild.AddComponent<EdgeCollider2D>();

            // 5. (Tùy chọn) Cấu hình sơ bộ cho Collider
            floor.edgeRadius = 0.1f; // Làm mượt các góc nhọn
            floor.SetMeOnLayer(Constants.Layers.SURFACE);


        }
#if UNITY_EDITOR

        private void OnValidate()
        {

           // this.View.Calculate(this.cameraOffsetFolow, this.orthographicSize);
            coll = this.WorldBound?.Coll;

            if (Application.isPlaying) return;

            if (startPos == null)
            {

                // Create a new GameObject for StartPoint
                GameObject spGo = new GameObject("StartPoint");
                spGo.transform.SetParent(this.transform);
                spGo.transform.localPosition = Vector3.zero;
                spGo.AddComponent<StartPoint>();
                startPos = spGo.transform;
                _startPos = spGo.GetComponent<ISpawnTarget>();
            }
            if (floor == null)
            {
                CreateChildGround();
            }

            if (floor != null)
            {
                EdgeCollider2DExtension.ResizeByPolygonCollider2D(floor, coll);
            }
            if (this.worldBound != null)
            {
                var collider2d = this.worldBound.GetComponent<PolygonCollider2D>();
                if (collider2d != null)
                {
                    var parent = collider2d.GetComponentInParent<IScenePrefab>();
                    if (parent != null)
                    {


                        PolygonCollider2Extension.AlignWall(collider2d, this.LeftTriggerZone?.Coll, this.RightTriggerZone?.Coll, true);
                        if (rightWall == null) _ = RightWall;
                        if (leftWall == null) _ = LeftWall;
                        PolygonCollider2Extension.AlignWall(collider2d, leftWall, rightWall);

                    }
                }
            }

            CalculateBound();
            if (defaultCamWatcher != null && !isDefaultCamWatcherSettings)
            {
                var sr = defaultCamWatcher.GetOrAddComponent<SpriteRenderer>();
                sr.sprite = Commons.LoadAsset<Sprite>("Packages/com.unvs.core/Runtime/Sprites/Circle.png");
                defaultCamWatcher.transform.position = this.worldBound.coll.bounds.center;
                isDefaultCamWatcherSettings = true;
            }
            //// Nếu đây là một Prefab đang mở trong Prefab Mode, hãy lưu Scene của nó
            //var stage = PrefabStageUtility.GetCurrentPrefabStage();
            //if (stage != null)
            //{
            //    EditorSceneManager.MarkSceneDirty(stage.scene);
            //}

        }
        private void OnDrawGizmos()
        {
            if (!gizmosDraw && Application.isPlaying) return;

            OnValidate();
            this.floor.GizmosDraw(Color.cyan, 5);
            this.leftWall?.GizmosDraw(Color.red, 5);
            //  this.leftWall?.GismosDrawHatchBox(Color.red, 25);
            this.rightWall?.GizmosDraw(Color.red, 5);
            // this.rightWall?.GismosDrawHatchBox(Color.red, 25,-45);
            if (this.LeftTriggerZone != null
                && !(this.LeftTriggerZone as MonoBehaviour).IsDestroyed()
                && this.LeftTriggerZone.Coll != null
                && (!this.LeftTriggerZone.Coll.gameObject.IsDestroyed()))
                this.LeftTriggerZone?.Coll?.GizmosDraw(Color.whiteSmoke, 5);
            if (this.RightTriggerZone != null
                && !(this.RightTriggerZone as MonoBehaviour).IsDestroyed())
                this.RightTriggerZone?.Coll?.GizmosDraw(Color.whiteSmoke, 5);
            this.WorldBound?.Coll?.GizmosDraw(Color.yellow, 5);
            var wt = this.GetComponentInChildren<IWorldTracker>();
            if (wt != null)
            {
                wt.Coll?.GizmosDraw(Color.aliceBlue, 25);
                //  wt.Coll?.GismosDrawHatchPolygon(Color.azure, 100);
            }
            if (joinInfo != null)
            {
                joinInfo.worldJoinInfo.LeftPos.DrawCircle(10, 0.1f, Color.red);


                // Vẽ điểm chốt bên phải màu xanh
                joinInfo.worldJoinInfo.RightPos.DrawCircle(10, 0.1f, Color.red);
            }
            //if (defaultCamWatcher == null)
            //{
            //    defaultCamWatcher = this.AddChildComponentIfNotExist<MonoBehaviour>(Constants.ObjectsConst.DEFAULT_CAM_WATCHER).gameObject;
            //    var coll = defaultCamWatcher.transform.AddComponentIfNotExist<BoxCollider2D>();
            //    coll.isTrigger = false;
            //    coll.gameObject.SetActive(false);
            //    _defaultCamWatcherReady = true;

            //}
            if (defaultCamWatcher == null)
            {
                var tr = this.GetComponentsInChildren<Transform>(true).FirstOrDefault(p => p.name == Constants.ObjectsConst.DEFAULT_CAM_WATCHER);
                if (tr != null) defaultCamWatcher = tr.gameObject;
            }
            if (defaultCamWatcher != null)
            {
                Collider2DExtension.GizmosDrawCamView(defaultCamWatcher.transform.position, this.OrthographicSize,this.CameraOffsetFolow, Color.yellow, 2f);
            }

        }

        private void DrawScreenView()
        {
            if (defaultCamWatcher == null)
            {
                var tr = GetComponentsInChildren<Transform>(true).FirstOrDefault(p => p.name == Constants.ObjectsConst.DEFAULT_CAM_WATCHER);
                if (tr != null) defaultCamWatcher = tr.gameObject;
            }

            if (defaultCamWatcher != null)
            {

                //var coll = defaultCamWatcher.transform.AddComponentIfNotExist<BoxCollider2D>();
                Collider2DExtension.GizmosDrawCamView(defaultCamWatcher.transform.position, this.orthographicSize, this.CameraOffsetFolow, Color.yellow, 2f);
            }
        }

        //private void OnDrawGizmosSelected()
        //{
        //    if (!Application.isPlaying)
        //    {
        //        OnValidate();
        //    }
        //}
#endif
        public void RemoveActor()
        {
            if (this.CurrentActor != null)
            {
                var go = (this.CurrentActor as MonoBehaviour).gameObject;
                if (go == null && go.IsDestroyed()) return;
                Destroy(go);
            }
        }

        public void SetupActor(IActorObject actor)
        {
            this.CurrentActor = actor;
            if (this._startPos != null)
            {
                this._startPos.MoveOtherToMe(actor as MonoBehaviour);

            }
            GlobalApplication.Cinema.VCam.Watch((this.CurrentActor.CamWacher as MonoBehaviour).transform);
        }



        public void SetupActor(IActorObject actor, string spawnName)
        {

            var target = this.FindSpawnTargetByName(spawnName);
            if (target != null)
            {
                var go = (actor as MonoBehaviour).gameObject;
                go.transform.position = target.Pos;

                GlobalApplication.Cinema.VCam.Watch((actor.CamWacher as MonoBehaviour).transform);
            }
            else
            {
                var go = (actor as MonoBehaviour).gameObject;
                go.transform.position = this.startPos.position;
                GlobalApplication.Cinema.VCam.Watch((actor.CamWacher as MonoBehaviour).transform);
            }
        }

        public ISpawnTarget FindSpawnTargetByName(string spawnName)
        {
            var ret = this.GoWorld.GetComponentsInChildren<Transform>()
                .Select(p => p.GetComponent<ISpawnTarget>())
                .FirstOrDefault(p => p != null && p.Name.Equals(spawnName, System.StringComparison.OrdinalIgnoreCase));
            return ret;
        }

        public void SetPos(Vector2 pos)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Nối cạnh PHẢI của prefab này vào một tọa độ pos cho trước (thường là điểm bắt đầu của Scene bên phải)
        /// </summary>
        public void DockToRightPos(Vector2 targetPos)
        {
            // 1. Tìm vị trí hiện tại của điểm giao bên PHẢI (World Space)
            // Lưu ý: Dùng LastEdge cho bên phải theo logic của bạn
            Vector2 currentEdgePos = this.floor.GetIntersectPointWithLastEdge(this.rightWall);

            // 2. Tính toán độ lệch (Vector từ điểm hiện tại tới điểm đích)
            Vector2 offset = targetPos - currentEdgePos;

            // 3. Dịch chuyển toàn bộ Transform để "hít" sàn vào đúng vị trí
            // Chúng ta cộng thêm offset vào position hiện tại
            transform.position += (Vector3)offset;
        }

        /// <summary>
        /// Nối cạnh TRÁI của prefab này vào một tọa độ pos cho trước (thường là điểm kết thúc của Scene bên trái)
        /// </summary>
        public void DockToLeftPos(Vector2 targetPos)
        {
            // 1. Tìm vị trí hiện tại của điểm giao bên TRÁI (World Space)
            Vector2 currentEdgePos = this.floor.GetIntersectPointWithFirstEdge(this.leftWall);

            // 2. Tính toán độ lệch
            Vector2 offset = targetPos - currentEdgePos;

            // 3. Thực hiện "bắt dính"
            transform.position += (Vector3)offset;
        }

        private void OnDestroy()
        {

            this.IsDestroying = true;
            this.OnSceneDestroy?.Invoke();
            this.OnDestroyMe?.Invoke(this);

        }

        public void TrimEdge()
        {
            if (Floor == null || JoinInfo == null || JoinInfo.WorldJoinInfo == null) return;
            if (hastrimEdge) return;

            Floor.ClipByFirstEdgeByX(JoinInfo.WorldJoinInfo.LeftPos.x);
            Floor.ClipLastEdgeByX(JoinInfo.WorldJoinInfo.RightPos.x);

            hastrimEdge = true;
        }

        public string GetLeftScenePath()
        {
            if (!string.IsNullOrEmpty(leftTriggerZone.TriggerPath)) return leftTriggerZone.TriggerPath;
            return GetComponent<IElasticScene>().LeftScenePath;
        }

        public string GetRightScenePath()
        {
            if (!string.IsNullOrEmpty(rightTriggerZone.TriggerPath)) return rightTriggerZone.TriggerPath;
            return GetComponent<IElasticScene>().RightScenePath;
        }

        public void GlobalightRestore()
        {

            var light = GlobalApplication.LightManagerObjectInstance.GlobalLight.transform.GetComponentsInChildren<IGlobalLightWapper>(true)
                 .Where(p => p.Owner != null && !p.Owner.IsDestroying)
                 .FirstOrDefault(p => p.Owner == this as IScenePrefab);

            this.GetComponent<IWorldGlobalLight>().GlobalLight = light;
        }



        // Start is called once before the first execution of Update after the MonoBehaviour is created

    }
}