
using unvs.actor.player;
using unvs.actor.skills;

using unvs.components;
using unvs.ext;


using unvs.shares;
using unvs.ui;
using unvs.controllers;
using unvs.ext.physical2d;
using UnityEngine;
using Unity.VisualScripting;
using System.Threading;
using UnityEngine.Rendering;
using unvs.game2d.objects;













#if UNITY_EDITOR
using unvs.editor.components;
using unvs.game2d.objects.editor;
#endif
using unvs.sys;

namespace unvs.game2d.actors
{
   


    [RequireComponent(typeof(UniqueObject))]

    [RequireComponent(typeof(AudioSource))]
  
  



    public partial class UnvsActor : UnvsBaseComponent
    {
        public bool IsActivePlayer = true;
       
        [SerializeField]
        public actor_physical2d<UnvsActor> physical;
        [SerializeField]
        public unvs.animators_controllers.ik_manager_controllers<UnvsActor> ik_manager;

        [SerializeField]
        public unvs.animators_controllers.motion_controllers<UnvsActor> motions;

        [SerializeField]
        public unvs.data.properties.Inventory<UnvsActor> inventory; 

        [SerializeField]
        public BaseSkillObject[] SkilObjects;
        public UnvsActorSkills Skills;
        public void SayText(string msg) => Skills.Get<unvs.actor.skills.ActorSpeaker>()?.SayText(msg);
        public void SayOff() => UnvsActorDialogue.Instance.Hide();

        public CancellationTokenSource cts => _cls;

        public AbstractActionBaseSkill CurrentSkill { get; set; }


        public float CrawlSpeed = 4f;
        //public UnvsActorPhysical physical_actor;

        public UnvsPlayer player;
        public UnvsActorSpeaker speaker;
     
        public CompositeCollider2D coll;
        //public Rigidbody2D body;
        public Transform camWatcher;
        public BoxCollider2D scanerBound;
      
        private CancellationTokenSource _cls;
        private Vector3 oririnalScale = Vector3.negativeInfinity;

        public void DirectionBy(float dir)
        {
            if (oririnalScale.Equals(Vector3.negativeInfinity))
            {
                oririnalScale = transform.localScale.CopyToNew();
            }
            if (dir > 0)
            {
                transform.localScale = oririnalScale.CopyToNew();

            }
            else
            {
                transform.localScale = new Vector3(-oririnalScale.x, transform.localScale.y, transform.localScale.z);
            }

        }
        private bool _isSkillsCloned = false;
        

        public virtual void OnEnable()
        {
            if (Skills != null && !_isSkillsCloned)
            {

                Skills = Instantiate(Skills);
                Skills.Initialize(this);
                if (Skills)
                    this.CurrentSkill = Skills.Get<unvs.actor.skills.ActorDefaultSkill>();

            }
        }
        public virtual CancellationTokenSource RefreshToken()
        {
            _cls = _cls.Refresh();
            return _cls;
        }
        public void StandBy(Vector2 vector2)
        {
            this.transform.position = vector2;// - (Vector2)this.coll.bounds.center;  //new Vector2((coll.bounds.max.x-coll.bounds.min.x)/2, -this.coll.bounds.size.y);


        }




        public T ScanObject<T>(params string[] layers) where T : Component
        {
           return  scanerBound.DetectObject<T>(0, 0, Constants.Layers.INTERACT_OBJECT, layers);
        
           
        }
        public T ScanObjectFromPont<T>(Vector2 pos, params string[] layers)
        {
            if (this.IsDestroyed() || this.gameObject.IsDestroyed()) return default(T);
            return Vector2dExtesion.ScanObject<T>(pos, this.scanerBound.size, layers);
            //return coll.ScanObject<T>(this.scanerBound.size.x, this.scanerBound.size.y, LayerMask.GetMask(layers));
        }
        private void Awake()
        {
            if (Application.isPlaying)
            {
                this.coll = GetComponentInChildren<CompositeCollider2D>();
                player = GetComponent<UnvsPlayer>();
                this.SetMeOnTag(Constants.Tags.PLAYER);
                this.scanerBound.SetMeOnTag(Constants.Tags.PLAYER_SCANER);

            }

        }
        private void Start()
        {
            if (Application.isPlaying)
            {
                if(this.coll.GetHit(out var hit, Vector2.down))
                {
                    this.StandBy(hit.point);
                }
                if (!this.IsActivePlayer)
                {
                    GetComponent<CompositeCollider2D>().excludeLayers = LayerMask.GetMask(Constants.Layers.NPC, Constants.Layers.ACTOR);
                    if (GetComponent<BasicController>() != null)
                    {
                        GetComponent<BasicController>().ControlDisable();
                        this.SetMeOnLayer(Constants.Layers.NPC);
                    } else
                    {
                        GetComponent<BasicController>().ControlEnable();
                        this.SetMeOnLayer(Constants.Layers.ACTOR);
                    }
                }
#if UNITY_EDITOR
                if (this.CurrentSkill == null)
                {
                    unvs.editor.utils.Dialogs.Show($"Please, add skill to actor {name}");
                    return;
                } 
#endif
                this.CurrentSkill.Status = SkillSpeddEnum.Idle;
            }
        }
        private void FixedUpdate()
        {
            if (this.CurrentSkill != null)
            {
                //body
                this.CurrentSkill.OnUpdate();
            }
        }
    }
#if UNITY_EDITOR
    [RequireComponent(typeof(UnvsDummyActor))]
    [RequireComponent(typeof(UnvsEditorShaderApply))]
    public partial class UnvsActor : UnvsBaseComponent
    {
        [SerializeField]
        public controllers.ActorController<UnvsActor> controller;
        [SerializeField]
        public accessories.components.accessories_editor<UnvsActor> accessories;

        
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
            //this.scanerBound.AddComponentIfNotExist<UnvsInteractScaner>();
            this.scanerBound.SetMeOnLayer(Constants.Layers.INTERACT_SCANER);
            this.scanerBound.SetMeOnTag(Constants.Tags.INTERACT_SCANER);
            this.scanerBound.isTrigger = true;
            if (this.speaker == null)
            {
                speaker = GetComponent<UnvsActorSpeaker>();
            }


        }
        
        public override void OnValidate()
        {
            base.OnValidate();
            this.SetMeOnTag(Constants.Tags.ACTOR);


            //accessories.owner = this as MonoBehaviour;

           
        }
        [UnvsButton]
        public void EditorCreateBagger()
        {
            this.AddComponentIfNotExist<UnvsBagger>();
        }

    }



#endif




}
