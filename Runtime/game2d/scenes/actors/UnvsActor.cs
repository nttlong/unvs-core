using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.ext;
using unvs.shares;

#if UNITY_EDITOR
using unvs.shares.editor;

#endif
using unvs.sys;
using Unvs.Core.Game2D.Scenes.Actors;
namespace unvs.game2d.scenes.actors
{
    [RequireComponent(typeof(IKBoneMap))]
    [RequireComponent(typeof(UnvsAnimStates))]
    [RequireComponent(typeof(UnvsActorSpeaker))]
    [RequireComponent(typeof(UniqueObject))]
    [RequireComponent(typeof(UnvsPlayer))]
    [RequireComponent (typeof(AudioSource))]
 
    [RequireComponent(typeof(UnvsActorPhysical))]
    [RequireComponent(typeof(UnsvPlayerDoTweenAnim))]
    [RequireComponent(typeof(UnvsActorSwimmable))]
   
    public partial class UnvsActor : UnvsBaseComponent
    {
        
        public CancellationTokenSource cts => _cls;
        [Header("Speed")]
        public float WalkSpeed = 8f;
        public float SprintSpeed = 16f;
        public float JumpingHeight=4f;
        public float CrawlSpeed=4f;
        public UnvsActorPhysical physical;

        public UnvsPlayer player;
        public UnvsActorSpeaker speaker;
        private bool isMoving;
        private Vector2 target;
        private Vector2 direction;
        
        public CompositeCollider2D coll;
        public Rigidbody2D body;
        public Transform camWatcher;
        public BoxCollider2D scanerBound;
        public GameObject animEle;
        public Animator animator;
        public UnvsAnimStates motions;
        private CancellationTokenSource _cls;
        

        public virtual CancellationTokenSource RefreshToken()
        {
            _cls = _cls.Refresh();
            return _cls;
        }
        public void StandBy(Vector2 vector2)
        {
            this.transform.position = vector2;// + new Vector2((coll.bounds.max.x-coll.bounds.min.x)/2,0);


        }
        public void SayText(string Content)
        {
            var pos = new Vector2(coll.bounds.center.x, coll.bounds.max.y + 2);
            UnvsActirDialogue.Instance.Show(pos, Content);
        }
        public void SayOff()
        {
            UnvsActirDialogue.Instance.Hide();
        }
        public async UniTask MovtoTargetAsync(Vector2 pos, CancellationToken tk=default )
        {
            if(tk== default)
            {
                tk = this.RefreshToken().Token;
            }
            

            await TransformExtension.MoveToAsync(this.transform, this.WalkSpeed, pos,
                   p => {
                       this.motions.direction = p.Direction;
                       this.motions.BaseMotion("walk");
                   },
                   p => {
                       this.motions.direction = p.Direction;
                       this.motions.BaseMotion("idle");

                   },
                   tk);
        }

        public T ScanObject<T>(params string[] layers)
        {
            var coll = GetComponent<Collider2D>();
            return coll.bounds.center.ScanObject<T>(this.scanerBound.size, layers);
        }
        public T ScanObjectFromPont<T>(Vector2 pos, params string[] layers)
        {
            if(this.IsDestroyed()||this.gameObject.IsDestroyed()) return default(T);
            return Vector2dExtesion.ScanObject<T>(pos, this.scanerBound.size, layers);
            //return coll.ScanObject<T>(this.scanerBound.size.x, this.scanerBound.size.y, LayerMask.GetMask(layers));
        }
        private void Awake()
        {
            if (Application.isPlaying)
            {
                this.coll = GetComponentInChildren<CompositeCollider2D>();
                player = GetComponent<UnvsPlayer>();
                physical= GetComponent<UnvsActorPhysical>();
               
            }

        }
        private void Start()
        {
            if (Application.isPlaying)
            {
                
                motions.BaseMotion("idle");
            }
        }
    }
#if UNITY_EDITOR
    public partial class UnvsActor : UnvsBaseComponent
    {
       

        

        [UnvsButton]
        public void FixLayout()
        {
            this.coll = GetComponentInChildren<CompositeCollider2D>();
            this.camWatcher.position = new Vector3(this.coll.bounds.center.x, this.coll.bounds.max.y, -10);
            var coll = this.camWatcher.AddComponentIfNotExist<BoxCollider2D>();
            coll.isTrigger = true;
            coll.SetMeOnTag(Constants.Tags.PLAYER_CAM_WATCHER);
            this.SetMeOnLayer(Constants.Layers.ACTOR);
            this.SetMeOnSortLayer(Constants.Layers.ACTOR);
            if (this.scanerBound == null)
            {
                this.scanerBound = this.AddChildComponentIfNotExist<BoxCollider2D>("scaner-bound");
                this.scanerBound.size = this.GetComponent<Collider2D>().bounds.size;
                
            }
            this.scanerBound.SetMeOnLayer(Constants.Layers.INTERACT_SCANER);
            this.scanerBound.SetMeOnTag(Constants.Tags.INTERACT_SCANER);
            this.scanerBound.isTrigger = true;
            if (this.speaker == null)
            {
                speaker = GetComponent<UnvsActorSpeaker>();
            }
            

        }
        [UnvsButton("Anim controller")]
        public void GenerateAnimatorController()
        {
            this.animEle = this.GetComponentInChildren<SpriteSkin>(true).transform.parent.gameObject;
            string folderPath = UnvsEditorUtils.EditorGetFolder(this.animEle);
            var controller = UnvsEditorUtils.EditorCreateAnimatorController(folderPath, this.animEle.name);
            this.animator = this.animEle.transform.AddComponentIfNotExist<Animator>();
            this.animator.runtimeAnimatorController = controller;
        }
        [UnvsButton]
        public void Generate()
        {
            //if (this.CheckComponentIfNotExistCreate<CapsuleCollider2D>(out var _coll))
            //{

            //    _coll.size = new Vector2(8, 20);
            //    _coll.offset = new Vector2(0, 10);
            //    this.coll = _coll;
            //    this.coll.isTrigger = true;
            //}


            body = this.AddComponentIfNotExist<Rigidbody2D>();
            body.freezeRotation = true;
            this.camWatcher = this.AddChildComponentIfNotExist<Transform>("cam-wacther");
            this.camWatcher.position = new Vector3(this.coll.bounds.center.x, this.coll.bounds.max.y, -10);
        }
        private void OnValidate()
        {
            this.SetMeOnTag(Constants.Tags.ACTOR);

        }

    }



#endif




}
