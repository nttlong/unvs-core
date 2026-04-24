using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D.Animation;
using unvs.actor.skills;
using unvs.ext;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using unvs.shares;

#if UNITY_EDITOR


#endif
using unvs.sys;

namespace unvs.game2d.actors
{
    [RequireComponent(typeof(IKBoneMap))]


    [RequireComponent(typeof(UniqueObject))]

    [RequireComponent(typeof(AudioSource))]

    [RequireComponent(typeof(UnvsActorPhysical))]



    public partial class UnvsActor : UnvsBaseComponent
    {
        [SerializeField]
        public BaseSkillObject[] SkilObjects;
        public UnvsActorSkills Skills;
        public void SayText(string msg) => Skills.Get<unvs.actor.skills.ActorSpeaker>()?.SayText(msg);
        public void SayOff() => UnvsActirDialogue.Instance.Hide();

        public CancellationTokenSource cts => _cls;

        public AbstractActionBaseSkill CurrentSkill { get; set; }


        public float CrawlSpeed = 4f;
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
            this.transform.position = vector2;// + new Vector2((coll.bounds.max.x-coll.bounds.min.x)/2,0);


        }




        public T ScanObject<T>(params string[] layers)
        {
            var coll = GetComponent<Collider2D>();
            return coll.bounds.center.ScanObject<T>(this.scanerBound.size, layers);
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
                physical = GetComponent<UnvsActorPhysical>();

            }

        }
        private void Start()
        {
            if (Application.isPlaying)
            {
                //SayText("Hello");
                //motions.BaseMotion("idle");
                this.CurrentSkill.Status = SkillSpeddEnum.Idle;
            }
        }
        private void FixedUpdate()
        {
            if (this.CurrentSkill != null)
            {

                this.CurrentSkill.OnUpdate();
            }
        }
    }
#if UNITY_EDITOR
    public partial class UnvsActor : UnvsBaseComponent
    {



        [UnvsButton]
        public void FixMarterial()
        {
            foreach (var skin in this.GetComponentsInChildren<SpriteSkin>(true))
            {
                var renderer = skin.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    // Nhân vật Rigged cần dùng Material có Z-Write On và Alpha Clipping
                    foreach (var mat in renderer.sharedMaterials)
                    {
                        mat.SetInt("_ZWrite", 1);
                        mat.EnableKeyword("_ALPHATEST_ON");
                        mat.renderQueue = 2450; // AlphaTest

                        // CỰC KỲ QUAN TRỌNG: 
                        // Ép Z-Test về LessEqual để chân không bị "đục lỗ" bởi chính nó
                        mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                    }
                }
            }

        }
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
        [UnvsButton("ShadowCaster2D")]
        public void EditorShadowCaster2DAll()
        {
            var compositeShadowCaster = this.AddComponentIfNotExist<CompositeShadowCaster2D>();

            foreach (var sp in this.GetComponentsInChildren<SpriteRenderer>(true))
            {
                var shadowGroup = sp.AddComponentIfNotExist<ShadowCaster2D>();
                shadowGroup.selfShadows = true;
                shadowGroup.castingOption = ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow;

            }
        }
        [UnvsButton("Anim controller")]
        public void GenerateAnimatorController()
        {
            this.animEle = this.GetComponentInChildren<SpriteSkin>(true).transform.parent.gameObject;
            string folderPath = unvs.editor.utils.UnvsEditorUtils.EditorGetFolder(this.animEle);
            var controller = unvs.editor.utils.UnvsEditorUtils.EditorCreateAnimatorController(folderPath, this.animEle.name);
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
