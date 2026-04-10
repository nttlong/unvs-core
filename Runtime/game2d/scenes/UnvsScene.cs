/*
    this file define Virtuale scene, acctually, that is prefab of scene
    it contain Main cam
   
   
*/
using Cysharp.Threading.Tasks.Triggers;
using game2d.ext;
using game2d.scenes;
using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.game2d.scenes.actors;
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

        public bool IsDestroying { get; private set; }
        public event Action<UnvsScene> OnDestroying;
        public override void InitRuntime()
        {
            DestroyImmediate(cam.gameObject);
            DestroyImmediate(vcam.gameObject);
            light2d.enabled = false;
            light2d.gameObject.SetActive(false);
            this.triggerLeft.isTrigger = true;
            this.triggerRight.isTrigger = true;
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
            this.triggerLeft.enabled=true;
            this.triggerLoadSceneLeft.enabled=true;
        }

        public void TurnOnRight()
        {
            this.wallRight.enabled = true;
            this.triggerRight.enabled = true;
            this.triggerLoadSceneRight.enabled = true;
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
            if(this.triggerRight != null)
            {
                this.triggerLoadSceneRight = this.triggerRight.AddComponentIfNotExist<LoadSceneTracking>();
                this.triggerLoadSceneRight.direction = LoadeSceneEnum.Right;
            }
            if (this.triggerRight != null)
            {
                this.triggerLoadSceneLeft = this.triggerLeft.AddComponentIfNotExist<LoadSceneTracking>();
                this.triggerLoadSceneLeft.direction = LoadeSceneEnum.Left;
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
            this.edgesWorldBound = this.AddChildComponentIfNotExist<EdgeCollider2D>("edgesWorldBound");
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
           
        }

      











#endif
    }
}