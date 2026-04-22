//using Cysharp.Threading.Tasks;
//using System;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using unvs.actors;
//using unvs.ext;
//
//using unvs.playerobjects;
//using unvs.shares;
//using unvs.ui;

//namespace unvs.players{
//    [RequireComponent(typeof(Rigidbody2D))]
//    [RequireComponent (typeof(CapsuleCollider2D))]
//    public class PlayerBase:MonoBehaviour{

//        public event Action<PlayerBase> OnRuntimeStart;
//        public CancellationTokenSource Cts;
//        private Collider2D coll;
//        [SerializeField]
//        public AnimExractor anims;
//        [SerializeField]
//        public Physical physical;
//        [SerializeField]
//        public Dialogue dialogue;
//        [SerializeField]
//        //public Bagger bagger;

//        public Transform CamWatcher;

       

//        public virtual CancellationTokenSource CancellationTokenSourceRefresh()
//        {

//            Cts = Cts.Refresh();
//            return Cts;
//        }

//        private void Reset()
//        {
//            var col=this.AddComponentIfNotExist<CapsuleCollider2D>();
//            col.size = new Vector2(8, 20);
//            col.offset= new Vector2(0, 10);
//        }
//        public virtual void Awake()
//        {
            
//            this.RuntimeInit();
            
//        }
//        public virtual void Start()
//        {
//            if (Application.isPlaying)
//            {
//                InitAllBaseSubPlayers();
                
//            }
             
//            GlobalApplication.Cinema.VCam.Watch(this.CamWatcher);

//        }
//        public virtual void InitAllBaseSubPlayers()
//        {
//            var fields = typeof(PlayerBase)
//                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
//                .Where(f => typeof(BaseSubPlayer).IsAssignableFrom(f.FieldType))
//                .ToList();

//            foreach (var field in fields)
//            {
//                var subPlayerInstance = field.GetValue(this);
//                if (subPlayerInstance != null)
//                {
//                    // Lấy method từ Type của SubPlayer
//                    var mt = field.FieldType.GetMethod("OnPlayerStart");

//                    if (mt != null)
//                    {
//                        // TARGET: subPlayerInstance (Đối tượng thực thi hàm)
//                        // PARAMETERS: this (Truyền PlayerBase vào cho SubPlayer dùng)
//                        mt.Invoke(subPlayerInstance, new object[] { this });
//                    }
//                }
//            }
//        }

//        public virtual void Update()
//        {
            
//            Vector2 mousePos = Mouse.current.position.ReadValue();

//            var v = mousePos.ToWorld(); 
//            this.physical.Direction(v);
//            if (Mouse.current.leftButton.IsPressed())
//            {
//                this.CancellationTokenSourceRefresh();
//                this.dialogue.SayText("I'm moveing");
//                this.physical.MoveTo(v);
//                this.bagger.Show();
//            } else if(Mouse.current.leftButton.wasReleasedThisFrame)
//            {
//                if(v.IsHitObject<PlayerInteractObject>(out var obj, Constants.Layers.INTERACT_OBJECT))
//                {
//                    obj.ExecAsync(this, this.CancellationTokenSourceRefresh()).Forget();
//                } else
//                {
//                    //this.CancellationTokenSourceRefresh();
//                    this.dialogue.SayText("I stop");
//                    this.anims.BaseMotion("idle");
//                }
                    
//            } else if(!this.physical.IsMoving)
//            {
//                this.CancellationTokenSourceRefresh();
               
//                this.dialogue.SayText("I stop");
//                this.anims.BaseMotion("idle");
//            } else
//            {
//                //this.CancellationTokenSourceRefresh();
//            }
           
//        }
//        public virtual void RuntimeInit()
//        {
//            if (!Application.isPlaying) return;
//            this.RunTimeSetPosition();
//            this.anims.BaseMotion("idle");
            
           
//        }

//        public virtual void RunTimeSetPosition()
//        {
//            var _currentScene=this.GetComponentInParent<IScenePrefab>();
//            coll=this.GetComponent<Collider2D>();
//            if(_currentScene != null )
//            {
//                _currentScene.StartPos.MoveOtherToMe(this);
                
//            }
//        }
//#if UNITY_EDITOR
//        public event Action GismoxDraw;
//        public void EditorExtractAllAnim()
//        {
//            this.anims.EditorExtractAllAnim(this);
//        }

//        public void EditorPhysicalCalculate()
//        {
//            this.physical.EditorPhysicalCalculate(this);
//        }
//        public void EditorGenerateDialogueUI()
//        {
//            this.dialogue.EditorGenerateDialogueUI(this);
//        }
//        private void OnDrawGizmos()
//        {
//            GismoxDraw?.Invoke();
//        }
//        private void OnValidate()
//        {
//            GismoxDraw?.Invoke();
//        }

//        public void EditorGenerateCamWatcher()
//        {
//           this.CamWatcher= this.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.CAM_WATCHER);
//            var coll= this.GetComponentInParent<Collider2D>();
//            if( coll != null)
//            {
//                this.CamWatcher.position = new Vector3(coll.bounds.center.x, coll.bounds.max.y, 0);
//            }
            
//        }

//        public void EditorGenerateBagger()
//        {
//            bagger.EditorGenerateBagger(this);
//        }




//#endif
//    }
//}