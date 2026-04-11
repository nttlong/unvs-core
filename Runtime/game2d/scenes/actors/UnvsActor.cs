using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
using unvs.ext;
using unvs.shares;

#if UNITY_EDITOR
using unvs.shares.editor;

#endif
using unvs.sys;
namespace unvs.game2d.scenes.actors
{
    [RequireComponent(typeof(IKBoneMap))]
    [RequireComponent(typeof(AnimMap))]
    [RequireComponent(typeof(UnvsActorSpeaker))]
    [RequireComponent(typeof(UniqueObject))]
    public partial class UnvsActor : UnvsBaseComponent
    {
        public CancellationTokenSource cts => _cls;



        public UnvsActorSpeaker speaker;
        private bool isMoving;
        private Vector2 target;
        private Vector2 direction;
        public float WalkSpeed = 8f;
        public Collider2D coll;
        public Rigidbody2D body;
        public Transform camWatcher;
        public BoxCollider2D scanerBound;
        public GameObject animEle;
        public Animator animator;
        public AnimMap motions;
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
        public async UniTask MovtoTargetAsync(Vector2 pos, CancellationToken tk = default)
        {
            // 1. Lấy CTS chính của Actor (KHÔNG đưa cái này vào using)
            var actorCts = this.cts;

            // 2. Tạo một biến đại diện cho Token sẽ dùng để di chuyển
            CancellationToken finalToken;
            CancellationTokenSource linkedCts = null;

            if (tk != default && tk != actorCts.Token)
            {
                if(actorCts==null) actorCts=this.RefreshToken();
                // Chỉ tạo Link nếu cần thiết
                linkedCts = CancellationTokenSource.CreateLinkedTokenSource(actorCts.Token, tk);
                finalToken = linkedCts.Token;
            }
            else
            {
                actorCts = this.RefreshToken();
                finalToken = actorCts.Token;
            }

            try
            {
                // 3. Thực hiện di chuyển
                await TransformExtension.MoveToAsync(this.transform, this.WalkSpeed, pos,
                    p => {
                        this.motions.direction = p.Direction;
                        this.motions.Motion("walk");
                    },
                    p => {
                        this.motions.direction = p.Direction;
                        this.motions.Motion("idle");

                    },
                    finalToken);
            }
            finally
            {
                // 4. CHỈ Dispose cái LinkedCts (cái tạm thời), KHÔNG Dispose actorCts
                if (linkedCts != null)
                {
                    linkedCts.Dispose();
                }
            }
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
    }
#if UNITY_EDITOR
    public partial class UnvsActor : UnvsBaseComponent
    {
        

        [UnvsButton]
        public void FixLayout()
        {

            this.camWatcher.position = new Vector3(this.coll.bounds.center.x, this.coll.bounds.max.y, -10);
            var coll = this.camWatcher.AddComponentIfNotExist<BoxCollider2D>();
            coll.isTrigger = true;
            coll.SetMeOnTag(Constants.Tags.PLAYER_CAM_WATCHER);
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
            if (this.CheckComponentIfNotExistCreate<CapsuleCollider2D>(out var _coll))
            {

                _coll.size = new Vector2(8, 20);
                _coll.offset = new Vector2(0, 10);
                this.coll = _coll;
            }


            body = this.AddComponentIfNotExist<Rigidbody2D>();
            body.freezeRotation = true;
            this.camWatcher = this.AddChildComponentIfNotExist<Transform>("cam-wacther");
            this.camWatcher.position = new Vector3(this.coll.bounds.center.x, this.coll.bounds.max.y, -10);
        }
        

    }



#endif




}
