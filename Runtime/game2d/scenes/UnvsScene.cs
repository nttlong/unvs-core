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
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.game2d.actors;
using unvs.game2d.objects.components;
using unvs.shares;

namespace unvs.game2d.scenes
{
    
    public partial class UnvsScene : UnvsComponent
    {

        [Header("Sene game world info")]
        public Transform checkPoints;
        public WorldJoinInfo JoinInfo = new WorldJoinInfo();
        [SerializeField] public float OrthographicSize = 20;
        public Transform deadZones;
        [SerializeField]
        public Vector2 followOffset = new Vector3(0, 4);
#if UNITY_EDITOR
        [Header("Links scene", order = -1)]
        [SerializeField]

        public _UnvsSceneEditorObject Links;
        public AssetReference selRef;
#endif
        public string SceneLeft;
        public string SceneRight;
        public EditorUnvsSceneSpawPointEditor SpawnPoints;
        [SerializeField]
        
        public Transform support;
        public Camera cam;
        public CinemachineCamera vcam;
        public Transform defaulCamWatcher;
        public CinemachineConfiner2D confiner;
        public CinemachineFollow cinemachineFollow;
     
        
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
        public UnvsBackgound background;
        private Transform pickableItems;
        public PolygonCollider2D groundThickness;
       
        public bool IsDestroying { get; private set; }
        

        public event Action<UnvsScene> OnDestroying;
        async UniTask initAsync()
        {
            await UniTask.Yield();
        }
        public override void InitRuntime()
        {
           
            if (cam != null)
            {
                cam.enabled = false;
                cam.gameObject.SetActive(false);
            }
            vcam.enabled = false;
            vcam.gameObject.SetActive(false);
            //Destroy(cam.gameObject);
            //Destroy(vcam.gameObject);
           
            light2d.enabled = false;
            light2d.gameObject.SetActive(false);
            //this.triggerLeft.isTrigger = true;
            //this.triggerRight.isTrigger = true;
            this.TrimGround();
           
           
        }
        public Vector2 GetStartPosition(string spawnName)
        {
            if(!string.IsNullOrEmpty(spawnName))
            {
                var tr = this.GetComponentInChildrenByName<Transform>(spawnName);
                var centrePoint = tr.GetSegment().Center();
                var hit = centrePoint.RayCast(Vector2.down, Math.Abs(centrePoint.y - worldBound.bounds.max.y));
                if(hit.collider!=null)
                {
                    return hit.point;
                }
                return this.ground.GetIntersetPoint(tr.GetSegment().Center().x);
            } else
            {
                var centrePoint = this.startPoint.GetSegment().Center();
                var hit = ((Vector2)this.startPoint.transform.position).RayCast(Vector2.down, 100f);
                if (hit.collider != null)
                {
                    return hit.point;
                }
                return this.ground.GetIntersetPoint(this.startPoint.transform.GetSegment().Center().x);
            }


               
        }
       
        public UnvsActor GetActiveActor()
        {
            //return this.GetComponentsInChildren<UnvsActor>(true).FirstOrDefault(p => p.GetComponent<UnvsPlayer>() != null);
            return this.GetComponentsInChildren<UnvsActor>(true).FirstOrDefault(p => p.IsActivePlayer);
        }
        public UnvsScene TurnOnLeft()
        {
            this.wallLeft.enabled=true;
            this.wallLeft.gameObject.SetActive(true);
            this.triggerLeft.enabled=true;
            this.triggerLeft.gameObject.SetActive(true);
            this.triggerLoadSceneLeft.enabled=true;
            this.triggerLoadSceneLeft.gameObject.SetActive(true );
            return this;
        }
        public UnvsScene TurnOffLeft()
        {
            this.wallLeft.enabled = false;
            this.wallLeft.gameObject.SetActive(false);
            this.triggerLeft.enabled = false;
            this.triggerLeft.gameObject.SetActive(false);
            this.triggerLoadSceneLeft.enabled = false;
            this.triggerLoadSceneLeft.gameObject.SetActive(false);
            return this;
        }
        public UnvsScene TurnOnRight()
        {
            this.wallRight.enabled = true;
            this.wallRight.gameObject.SetActive(true);
            this.triggerRight.enabled = true;
            this.triggerRight.gameObject.SetActive(true);
            this.triggerLoadSceneRight.enabled = true;
            this.triggerLoadSceneRight.gameObject.SetActive(true);
            return this;
        }
        public UnvsScene TurnOffRight()
        {
            this.wallRight.enabled = false;
            this.wallRight.gameObject.SetActive(false);
            this.triggerRight.enabled = false;
            this.triggerRight.gameObject.SetActive(false);
            this.triggerLoadSceneRight.enabled = false;
            this.triggerLoadSceneRight.gameObject.SetActive(false);
            return this;
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
        

    }
}