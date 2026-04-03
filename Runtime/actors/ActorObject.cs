

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.actors.actions;
using unvs.baseobjects;
using unvs.ext;
using unvs.gameobjects;
using unvs.gameword;
using unvs.interfaces;
using unvs.shares;
using unvs.sys;
using unvs.ui;



namespace unvs.actors
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ActorMotionObject))] // Quan ly Motion
    [RequireComponent(typeof(ActorSpeakerObject))] // Dung khi NV can noi chuyen
    [RequireComponent(typeof(ActorMovableObject))] // Dung khi NV di chuyen hoac combat...
    [RequireComponent(typeof(ActorInteractableObject))] // Dung khi NV can tuong tac voi cac doi tuong khac
    [RequireComponent(typeof(ActorPhysicalObject))] // Dung de quan ly Bone va tinh toan nhu: do dai canh tay, do dai buoc chan,...
    [RequireComponent (typeof(ActorInventoryObject))] // Dung de quan ly tui do cua NV
    [RequireComponent(typeof(AudibleStepObject))] // Dung de quan ly cac am thanh ma NV tao ra do tuong tac voi cac doi tuong khac
    [RequireComponent(typeof(ActorController))] // Dung de ket noi voi input_system, dieu khien NV
    [RequireComponent(typeof(CapsuleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(UniqueObject))]
    [RequireComponent(typeof(SortingGroup))]
    
    [RequireComponent(typeof(ShadowCaster2DObject))]
    public class ActorObject : MonoBehaviour, IActorObject
    {
        IActorObject instance;
        private IActorMotion motion;
        private ISpeakableObject speaker;
        private ICamWacher camWacher;
        private IActorMovable movable;
        private IActorInteractable interactable;
        private IActorPhysical physical;
        private ActorController controller;
        public Rigidbody2D rb;
        private ContactFilter2D floorFilter;
        private ContactPoint2D[] contacts = new ContactPoint2D[10];
        public bool isActive = true;
        private SettingsUIInventory inventory;

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

        public IActorController Controller => controller;

        public Rigidbody2D Body => rb;

        public bool IsActive { get => isActive; set => isActive = value; }
        public Action OnDestroying { get; set; }

        public IInventoryController Inventory => inventory;

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
            if(Application.isPlaying)
            {
                inventory = this.GetComponent<ActorInventoryObject>().InventoryController as SettingsUIInventory;
            }
        }

        // Use this for initialization
        private void Awake()
        {
            _ = CamWacher;
            controller = GetComponent<ActorController>();
            if (!Application.isPlaying)
            {
                if(controller == null)
                {
                    throw new Exception($"require {typeof(ActorsControllers)}");
                }
            }

            instance = this;
            motion = GetComponent<IActorMotion>();
            movable = GetComponent<ActorMovableObject>();
            SetupRigidBody();
            speaker = GetComponent<ISpeakableObject>();
            interactable = GetComponent<IActorInteractable>();
            physical = GetComponent<IActorPhysical>();
            

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
                    this.Motion.Walk();
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
       
        
       
        private void OnDestroy()
        {
            OnDestroying?.Invoke();
        }
    }
}