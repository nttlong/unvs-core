/*
    this file define Virtuale scene, acctually, that is prefab of scene
    it contain Main cam
   
   
*/

using Cysharp.Threading.Tasks;
using System;
using System.Linq;

using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.game2d.scenes.actors;
using unvs.interfaces;
using unvs.shares;

namespace unvs.game2d.scenes
{
    
    public class UnvsScene : UnvsComponent
    {
        [Header("Links scene")]
        public string SceneLeft;
        public string SceneRight;
        [SerializeField]
        [Header("Sene game world info")]
        public WorldJoinInfo JoinInfo = new WorldJoinInfo();
        public float OrthographicSize = 20;
        [SerializeField]
        public Vector2 followOffset;
       
        public Camera cam;
        public CinemachineCamera vcam;
        public Transform defaulCamWatcher;
        public CinemachineConfiner2D confiner;
        public CinemachineFollow cinemachineFollow;
     
        public EdgeCollider2D edgesWorldBound;
        public Transform startPoint;
       
        public PolygonCollider2D worldBound;
        public BoxCollider2D triggerLeft;
        public BoxCollider2D triggerRight;
        public BoxCollider2D wallLeft;
        public BoxCollider2D wallRight;
        public EdgeCollider2D ground;
        public Light2D light2d;
        public UnvsActor actor;
        public LoadSceneTracking triggerLoadSceneLeft;
        public LoadSceneTracking triggerLoadSceneRight;
        internal UnvsScene leftScene;
        internal UnvsScene rightScene;
        private bool _hastrimEdge;
        public Transform sceneTracker;

        public bool IsDestroying { get; private set; }
        public event Action<UnvsScene> OnDestroying;
        async UniTask initAsync()
        {
            await UniTask.Yield();
        }
        public override void InitRuntime()
        {
            cam.enabled = false;
            cam.gameObject.SetActive(false);
            vcam.enabled = false;
            vcam.gameObject.SetActive(false);
            DestroyImmediate(cam.gameObject);
            DestroyImmediate(vcam.gameObject);
           
            light2d.enabled = false;
            light2d.gameObject.SetActive(false);
            //this.triggerLeft.isTrigger = true;
            //this.triggerRight.isTrigger = true;
            this.TrimGround();
           
           
        }
        public Vector2 GetStartPosition()
        {
            return this.ground.GetIntersetPoint(this.startPoint.transform.GetSegment().Center().x);
        }
        public override void InitDesignTime()
        {
            //throw new System.NotImplementedException();
        }
        public UnvsActor GetActiveActor()
        {
            return this.GetComponentsInChildren<UnvsActor>().FirstOrDefault(p=>p.GetComponent<UnvsPlayer>()!=null && p.GetComponent<UnvsPlayer>().enabled);
        }
        public void TurnOnLeft()
        {
            this.wallLeft.enabled=true;
            this.wallLeft.gameObject.SetActive(true);
            this.triggerLeft.enabled=true;
            this.triggerLeft.gameObject.SetActive(true);
            this.triggerLoadSceneLeft.enabled=true;
            this.triggerLoadSceneLeft.gameObject.SetActive(true );
        }
        public void TurnOffLeft()
        {
            this.wallLeft.enabled = false;
            this.wallLeft.gameObject.SetActive(false);
            this.triggerLeft.enabled = false;
            this.triggerLeft.gameObject.SetActive(false);
            this.triggerLoadSceneLeft.enabled = false;
            this.triggerLoadSceneLeft.gameObject.SetActive(false);
        }
        public void TurnOnRight()
        {
            this.wallRight.enabled = true;
            this.wallRight.gameObject.Serialize(true);
            this.triggerRight.enabled = true;
            this.triggerRight.gameObject.SetActive(true);
            this.triggerLoadSceneRight.enabled = true;
            this.triggerLoadSceneRight.gameObject.SetActive(true);
        }
        public void TurnOffRight()
        {
            this.wallRight.enabled = false;
            this.wallRight.gameObject.Serialize(false);
            this.triggerRight.enabled = false;
            this.triggerRight.gameObject.SetActive(false);
            this.triggerLoadSceneRight.enabled = false;
            this.triggerLoadSceneRight.gameObject.SetActive(false);
        }
        
        public void TrimGround()
        {
            if (ground == null || JoinInfo == null ) return;
            if (_hastrimEdge) return;
           
            ground.ClipByFirstEdgeByX(this.wallLeft.bounds.max.x);
            ground.ClipLastEdgeByX(this.wallRight.bounds.min.x);

            //_hastrimEdge = true;
        }
        private void OnDestroy()
        {
            this.IsDestroying=true;
            this.OnDestroying?.Invoke(this);
            UnvsApp.Instance.RaiseEventScenseDestroying(this);
        }
        
#if UNITY_EDITOR
        [UnvsButton("Review")]
        public void Review()
        {
            ApplyRequireComponents();
            this.actor = this.GetComponentInChildren<UnvsActor>();
            if(this.actor != null )
            {
                this.actor.StandBy(this.GetStartPosition());
                this.vcam.Watch(this.actor.camWatcher);
                
            } else
            {
               
                this.vcam.Watch(defaulCamWatcher);
               
            }
            //this.vcam.GetComponent<CinemachineConfiner2D>().BoundingShape2D = this.worldBound;
        }
        [UnvsButton("Apply require components")]
        public void ApplyRequireComponents()
        {
            if(this.vcam!=null)
            {
                this.vcam.Lens.OrthographicSize = this.OrthographicSize;
                this.vcam.GetComponent<CinemachineFollow>().FollowOffset = this.followOffset;
            }
            if (this.triggerRight != null)
            {
                this.triggerLoadSceneRight = this.triggerRight.AddComponentIfNotExist<LoadSceneTracking>();
                this.triggerLoadSceneRight.direction = LoadeSceneEnum.Right;
            }
            if (this.triggerRight != null)
            {
                this.triggerLoadSceneLeft = this.triggerLeft.AddComponentIfNotExist<LoadSceneTracking>();
                this.triggerLoadSceneLeft.direction = LoadeSceneEnum.Left;
            }
            if (this.sceneTracker == null)
            {
                this.sceneTracker = this.AddChildComponentIfNotExist<Transform>("scene-tracker");
                var poly = this.sceneTracker.AddComponentIfNotExist<PolygonCollider2D>();
                poly.isTrigger=true;
                poly.SetMeOnLayer(Constants.Layers.TRIGGER_SCENE_CHANGE);
            }
            if (this.triggerLeft != null) this.triggerLeft.isTrigger = true;
            if (this.triggerRight != null) this.triggerRight.isTrigger = true;

            this.sceneTracker.AddComponentIfNotExist<UnvsSceneTracker>();
            syncWorldBoundAndScencTracker();
        }

        private void syncWorldBoundAndScencTracker()
        {
            var rate = (this.worldBound.bounds.size.x +5f) / this.worldBound.bounds.size.x;
            var paths = this.worldBound.ClonePaths(rate, 1);
            this.sceneTracker.GetComponent<PolygonCollider2D>().points = new Vector2[] { };
            for (int i = 0; i < paths.Length; i++)
            {
                this.sceneTracker.GetComponent<PolygonCollider2D>().SetPath(i, paths[i]);
            }
        }

        [UnvsButton("Generate elements")]
        public void Generate()
        {
            this.cam = this.AddChildComponentIfNotExist<Camera>("Main Camera");
            this.cam.tag = "MainCamera";
            this.cam.orthographic = true;
            this.cam.AddComponentIfNotExist<CinemachineBrain>();
            this.vcam = this.AddChildComponentIfNotExist<CinemachineCamera>("vcam");
            this.defaulCamWatcher = this.AddChildComponentIfNotExist<Transform>("default-cam-watcher");
            this.confiner = this.vcam.GetOrAddComponent<CinemachineConfiner2D>();
            this.cinemachineFollow = this.vcam.GetOrAddComponent<CinemachineFollow>();
            this.vcam.Follow = this.defaulCamWatcher;
            this.JoinInfo.Size = this.cam.GetCameraWorldSize();
            //this.edgesWorldBound = this.AddChildComponentIfNotExist<EdgeCollider2D>("edgesWorldBound");
            this.startPoint = this.AddChildComponentIfNotExist<Transform>("start-point");
            this.startPoint.transform.position = new Vector3(this.JoinInfo.Size.x / 2, 0, -10);
            this.defaulCamWatcher.transform.position = this.JoinInfo.Size / 2;
            this.worldBound = this.AddChildComponentIfNotExist<PolygonCollider2D>("world-bound");
            this.worldBound.SetMeOnLayer(Constants.Layers.WORLD_BOUND);
            this.worldBound.isTrigger = true;
            this.worldBound.points = this.defaulCamWatcher.GetSegment().Center().CreateRectFromCenter(this.JoinInfo.Size);
            this.triggerLeft = this.AddChildComponentIfNotExist<BoxCollider2D>("trigger-left");
            this.triggerLeft.SetMeOnLayer(Constants.Layers.TRIGGER_LOAD_SCENE);
            this.triggerLeft.SetMeOnTag(Constants.Tags.TRIGGER_LOAD_SCENE_LEFT);

            this.triggerRight = this.AddChildComponentIfNotExist<BoxCollider2D>("trigger-right");
            this.triggerRight.SetMeOnLayer(Constants.Layers.TRIGGER_LOAD_SCENE);
            this.triggerRight.SetMeOnTag(Constants.Tags.TRIGGER_LOAD_SCENE_LEFT);
            this.wallLeft = this.AddChildComponentIfNotExist<BoxCollider2D>("wall-left");
            this.wallRight = this.AddChildComponentIfNotExist<BoxCollider2D>("wall-right");
            this.ground = this.AddChildComponentIfNotExist<EdgeCollider2D>("ground");
            var dx = this.defaulCamWatcher.transform.GetSegment().Center().x;
            this.ground.points = new Vector2[] { new Vector2(dx - this.JoinInfo.Size.x / 2 - 5, 0), new Vector2(dx + this.JoinInfo.Size.x / 2 + 5, 0) };

            this.worldBound.AlignWall(this.wallLeft,this.wallRight); 
            this.worldBound.AlignWall(this.triggerLeft, this.triggerRight,true);
            this.light2d = this.AddChildComponentIfNotExist<Light2D>("light2d");
            this.light2d.lightType = Light2D.LightType.Global;
           
           
            this.ApplyRequireComponents();
            calculateJoinPoint();
        }
        
        private bool calculateJoinPoint()
        {
           
            this.JoinInfo= ground.CalculateBound(this.worldBound);
           
            return this.JoinInfo!=null;
        }

        private void OnValidate()
        {
            if (this.cinemachineFollow != null)
            {
                this.followOffset = this.cinemachineFollow.FollowOffset;
            }
        }
        private void OnDrawGizmos()
        {
            if (this.worldBound != null) this.worldBound.GizmosDraw(Color.green, 1);
            if (this.defaulCamWatcher != null) this.defaulCamWatcher.transform.GetSegment().Center().DrawCircle(1f,Color.red);
            if(this.startPoint!=null) this.startPoint.GetSegment().Center().DrawCircle(1f, Color.green);
            if(calculateJoinPoint())
            {
                this.JoinInfo.LeftPos.DrawCircle(1,Color.red);
                this.JoinInfo.RightPos.DrawCircle(1,Color.red);
            }
            if(this.worldBound!=null && this.wallLeft!=null && this.wallRight != null)
            {
                this.worldBound.AlignWall(this.wallLeft, this.wallRight);
                this.wallLeft.GizmosDraw(Color.red, 1);
                this.wallRight.GizmosDraw(Color.red, 1);
            }
            if (this.worldBound != null && this.triggerLeft != null && this.triggerRight != null)
            {
                this.worldBound.AlignWall(this.triggerLeft, this.triggerRight, true);
                this.triggerLeft.GizmosDraw(Color.rosyBrown, 2);
                this.triggerRight.GizmosDraw(Color.rosyBrown, 2);
            }
            if(this.ground!=null)
            {
                this.ground.GizmosDraw(Color.red,3f);
            }
            this.OrthographicSize = this.vcam.GetOrthoSize();
            if (this.JoinInfo != null)
            {
                this.JoinInfo.LeftPos.DrawCircle(1,Color.blue,3f);
                this.JoinInfo.RightPos.DrawCircle(1, Color.darkGreen, 3f);
            }
            if (this.sceneTracker != null && this.worldBound!=null)
            {
                syncWorldBoundAndScencTracker();
                if(this.sceneTracker.GetComponent<PolygonCollider2D>()!=null)
                this.sceneTracker.GetComponent<PolygonCollider2D>().GizmosDraw(Color.azure, 2f);
            }
           
        }

       













#endif
    }
}