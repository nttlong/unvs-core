

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.actors.actions;
using unvs.ext;
using unvs.gameobjects;
using unvs.gameword;
using unvs.interfaces;
using unvs.shares;
using unvs.sys;



namespace unvs.actors
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ActorMotionObject))]
    [RequireComponent(typeof(ActorSpeakerObject))]
    [RequireComponent(typeof(ActorMovableObject))]
    [RequireComponent(typeof(ActorInteractableObject))]
    [RequireComponent(typeof(ActorPhysicalObject))]
    [RequireComponent(typeof(AudibleStepObject))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(UniqueObject))]
    [RequireComponent(typeof(SortingGroup))]
    public class ActorObject : MonoBehaviour, IActorObject
    {
        IActorObject instance;
        private IActorMotion motion;
        private ISpeakableObject speaker;
        private ICamWacher camWacher;
        private IActorMovable movable;
        private IActorInteractable interactable;
        private IActorPhysical physical;
        private ActorsControllers controller;
        public Rigidbody2D rb;
        private ContactFilter2D floorFilter;
        private ContactPoint2D[] contacts = new ContactPoint2D[10];
        public bool isActive = true;

        public event Action<IActorObject> OnMoving;

        public CancellationTokenSource Cts { get; set; }
        public IActorMotion Motion => motion;

        public ISpeakableObject Speaker => speaker;

        public ICamWacher CamWacher
        {
            get
            {
                if (camWacher != null) return camWacher;
                camWacher = this.transform.CreateIfNoExist<CharatorCamWacher>(Constants.ObjectsConst.CAM_WATCHER);
                return camWacher;
            }
        }

        public IActorMovable Movable => movable;

        public CapsuleCollider2D Coll => GetComponent<CapsuleCollider2D>();

        public IActorInteractable Interactable
        {
            get
            {
                if (interactable != null) return interactable;
                interactable = GetComponent<IActorInteractable>();

                return interactable;
            }
        }

        public IActorPhysical Physical => physical;

        public DirectionEnum SideView
        {
            get { return physical.Direction; }
            set
            {
                physical.Direction = value;

                if (value == DirectionEnum.Backward)
                {
                    this.Motion.Flip(-1);

                }
                else
                {
                    this.Motion.Flip(1);
                }
            }
        }

        public ActorsControllers Controller => controller;

        public Rigidbody2D Body => rb;

        public bool IsActive { get => isActive; set => isActive = value; }
        public Action OnDestroying { get; set; }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();


            // Thiết lập bộ lọc: Chỉ lấy va chạm với layer "Floor"
            floorFilter = new ContactFilter2D();
            floorFilter.SetLayerMask(LayerMask.GetMask(Constants.Layers.SURFACE));
            floorFilter.useLayerMask = true;
            if (!isActive)
            {
                var edge = this.GetComponent<CapsuleCollider2D>().FindBottomEdge(Constants.Layers.SURFACE);
                if (edge != null)
                {
                    var pos = edge.GetIntersetPoint(this.GetComponent<CapsuleCollider2D>().bounds.center.x);
                    this.transform.position = pos;
                }


            }
        }

        // Use this for initialization
        private void Awake()
        {
            _ = CamWacher;
            if (!Application.isPlaying) return;

            instance = this;
            motion = GetComponent<IActorMotion>();
            movable = GetComponent<ActorMovableObject>();
            SetupRigidBody();
            speaker = GetComponent<ISpeakableObject>();
            interactable = GetComponent<IActorInteractable>();
            physical = GetComponent<IActorPhysical>();
            controller = GetComponent<ActorsControllers>();

            unvs.manager.GameEvents.OnSceneLoadComplete += GameEvents_OnSceneLoadComplete; ;
            //unvs.manager.ChunkSceneLoaderUtils.OnLoadNew += ChunkSceneLoaderUtils_OnLoadNew;
            if (isActive)
                InitActionController();
            if (!IsActive)
            {
                this.SetMeOnLayer(Constants.Layers.NPC, true);


            }
            else
            {
                this.SetMeOnLayer(Constants.Layers.ACTOR, true);
            }

        }


        private void InitActionController()
        {
            if (!Application.isPlaying) return;

            controller.OnMoving = (dir) =>
            {
                speaker.Off();
                this.Cts = this.Cts.Refresh();
                controller.IsMoving = true;
                controller.Direction = dir;

                Motion.Walk();

                Motion.Flip(dir.x);
                controller.Speed = Movable.WalkSpeed;
                Movable.Direction = dir;

            };
            controller.OnStop = () =>
            {
                this.Cts = this.Cts.Refresh();

                Motion.Idle();
                //direction = Vector2.negativeInfinity;
                controller.Speed = 0;
                controller.IsMoving = false;
            };
            controller.OnSprint = (dir) =>
            {
                this.Cts = this.Cts.Refresh();
                speaker.Off();
                controller.Speed = Movable.RunSpeed;
                controller.Direction = dir;
                Motion.Sprint();
            };
            controller.OnInteract = (go) =>
            {
                var interactObject = go.GetComponent<IInteractableObject>();
                if (interactObject != null)
                {
                    var oldSpeed = controller.Speed;
                    controller.Speed = 0;
                    this.Cts = this.Cts.Refresh();
                    interactObject.ExecAsync(this, this.Cts).ContinueWith(p =>
                    {
                        controller.IsInteracting = false;
                        controller.Speed = oldSpeed;
                    }).Forget();
                }
                //Speaker.SayText($"OnInteract {go.name}");
            };
        }

        private void GameEvents_OnSceneLoadComplete(IScenePrefab arg1, manager.SceneState arg2, ISpawnTarget startSpawn)
        {
            if (this.IsDestroyed())
            {
                unvs.manager.GameEvents.OnSceneLoad -= GameEvents_OnSceneLoadComplete;
                return;
            }
            var uniqueObj = this.GetComponent<IUniqueObject>();
            if (uniqueObj != null && !uniqueObj.IsValidate)
            {
                unvs.manager.GameEvents.OnSceneLoad -= GameEvents_OnSceneLoadComplete;
                return;
            }

            this.transform.SetParent(null, false);
            if (startSpawn != null) startSpawn.MoveOtherToMe(this);
            unvs.shares.GlobalApplication.SingleScene.VCam.Watch(this.CamWacher.Coll.transform);
            unvs.manager.GameEvents.CurrentActor = this;
            unvs.manager.GameEvents.OnSceneLoad -= GameEvents_OnSceneLoadComplete;
        }




        public async UniTask MoveToAsync(Vector2 Pos, CancellationToken ct)
        {
            if (ct == null)
            {
                return;
            }
           
            if (ct.IsCancellationRequested) return;
            ct.ThrowIfCancellationRequested();
            var speaker = GetComponent<ISpeakableObject>();

            try
            {
                this.Motion.Walk();


                this.speaker.Off();

                var dir = this.Coll.bounds.center.x < Pos.x ? 1 : -1;
                Physical.Direction = dir > 0 ? DirectionEnum.Forward : DirectionEnum.Backward;

                var ret = await this.transform.MoveToAsync(Movable.WalkSpeed, Pos, p =>
                {
                    Physical.Direction = p.Direction > 0 ? DirectionEnum.Forward : DirectionEnum.Backward;

                }, p =>
                {
                    this.motion.Idle();
                }, ct);
                //if (!ct.IsCancellationRequested) // if not cancel -> finished routine
                    
            }
            catch (System.OperationCanceledException)
            {

                return;
            }


        }
        private void SetupRigidBody()
        {
            var rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }
        private void Update()
        {   if (controller == null) return;
            if (controller.IsInteracting) return;
            if (!controller.IsMoving) return;
            OnMoving?.Invoke(this);
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            if (controller?.Speed > 0)
            {
                rb.transform.MoveContinuous(controller.Direction, controller.Speed);
            }
        }
        void UpdateDelete()
        {
            if(controller!=null)
            Speaker.SayText($"IsInteracting={controller.IsInteracting}");
            if (rb == null || controller == null) return;
            if (controller.IsInteracting ) return;
            int contactCount = rb.GetContacts(floorFilter, contacts);
            bool isGrounded = false;
            Vector2 groundNormal = Vector2.up;

            if (contactCount > 0)
            {
                for (int i = 0; i < contactCount; i++)
                {
                    // Độ dốc cho phép (0.5f tương đương dốc 45 độ)
                    if (contacts[i].normal.y > 0.5f)
                    {
                        isGrounded = true;
                        groundNormal = contacts[i].normal;
                        break;
                    }
                }
            }

            Vector2 finalVelocity = rb.linearVelocity;
            float moveInput = controller.Direction.x;

            if (Mathf.Abs(moveInput) > 0.01f && controller.Speed > 0)
            {
                if (isGrounded)
                {
                    // Tính hướng dốc chuẩn
                    Vector2 slopeDir = new Vector2(groundNormal.y, -groundNormal.x);

                    // Tính vận tốc dựa trên hướng dốc
                    Vector2 moveVelocity = slopeDir * (moveInput * controller.Speed);

                    // BÍ QUYẾT: Chỉ ép Y mạnh khi đang thực sự ở trên dốc đi xuống
                    // Nếu mặt ngang (Normal.y xấp xỉ 1), chỉ ép nhẹ để giữ Grounded
                    float stickyForce = (groundNormal.y < 0.99f) ? -2f : -0.1f;

                    finalVelocity.x = moveVelocity.x;
                    finalVelocity.y = moveVelocity.y + stickyForce;
                }
                else
                {
                    // Trên không: giữ X, Y rơi tự do theo trọng lực Unity
                    finalVelocity.x = moveInput * controller.Speed;
                }
            }
            else
            {
                // Khi không bấm nút: Dừng X, Y giữ nguyên để bám sàn
                finalVelocity.x = 0;
                if (isGrounded) finalVelocity.y = -0.5f;
            }

            rb.linearVelocity = finalVelocity;
        }
        void HandleSlopeMovement(Vector2 groundNormal)
        {
            // Triệt tiêu lực nảy bằng cách điều chỉnh vận tốc theo độ nghiêng của sàn
            // Giúp nhân vật bám dính lấy dốc
            float slopeAngle = Vector2.Angle(Vector2.up, groundNormal);

            if (slopeAngle > 0)
            {
                // Ép vận tốc Y tỉ lệ thuận với vận tốc X để "dán" vào mặt dốc
                Vector2 velocity = rb.linearVelocity;
                float speed = velocity.x;
                rb.linearVelocity = new Vector2(speed, -Mathf.Abs(speed) * Mathf.Tan(slopeAngle * Mathf.Deg2Rad));
            }
        }
        private void OnDestroy()
        {
            OnDestroying?.Invoke();
        }
    }
}