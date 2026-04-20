
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using unvs.ext;
using unvs.ext.physical2d;
using unvs.game2d.objects;
using unvs.shares;

namespace unvs.game2d.scenes.actors
{

    public partial class UnvsActorPhysical : UnvsBaseComponent
    {
        [SerializeField]
        public Collider2D[] HitBoxesCollider;
        public Transform bodyBone;
        public float ArmLen;
        public Transform ArmTop;
        public Transform ArmRoot;
        public Transform handBack;
        public Transform handFront;
        public Transform socketHandBack;
        public Transform socketHandFront;
        public Transform headBone;
        private UnvsActorPhysicalSolverRuntime _socketHandBackController;
        private UnvsActorPhysicalSolverRuntime _socketHandFrontController;
        private IKManager2D _ikManager;
        //[SerializeField]
        //public MovingInfo movingInfo;
        internal UnvsPickableObject currentHoldingItem;
        private Collider2D _coll;
        [SerializeField]
        public Transform[] Footers;
        private CalculateSlopeDirectionResull _slopDirectionResult;
        private UnvsActor _actor;

        private IKManager2D ikManager
        {
            get
            {
                if (_ikManager == null)
                {
                    _ikManager = this.GetComponentInChildren<IKManager2D>();
                }
                return _ikManager;
            }
        }

        public UnvsActorPhysicalSolverRuntime socketHandBackController
        {
            get
            {
                if (_socketHandBackController.IsEmpty())
                {
                    _socketHandBackController = new UnvsActorPhysicalSolverRuntime();
                    _socketHandBackController.target = GetTargetByName(socketHandBack.parent.name);
                    //_socketHandBackController.solver = GetSolver2DByName(socketHandBack.parent.name);
                }
                return _socketHandBackController;
            }
        }
        public UnvsActorPhysicalSolverRuntime socketHandFrontController
        {
            get
            {
                if (_socketHandFrontController.IsEmpty())
                {
                    _socketHandFrontController = new UnvsActorPhysicalSolverRuntime();
                    _socketHandFrontController.target = GetTargetByName(socketHandFront.parent.name);
                    //_socketHandFrontController.solver = GetSolver2DByName(socketHandFront.parent.name);
                }
                return _socketHandFrontController;
            }
        }

        public float NormalHeight { get; private set; }

        public async UniTask MoveSocketHandBackToAsync(Vector2 pos, float duration = 1f, CancellationToken token = default)
        {
            // 1. Lấy tham chiếu các thành phần
            var bone = this.socketHandBack.parent;
            var target = GetTargetByName(bone.name);
            var solver = GetSolver2DByName(bone.name);

            if (target == null)
            {
                Debug.LogWarning($"[UnvsActorPhysical] Target for bone {bone.name} not found.");
                return;
            }

            // 2. Đồng bộ target với xương trước khi bật Solver để tránh méo mó (Restore Pose logic)
            target.position = bone.position;

            // 3. Kích hoạt Solver (Bỏ qua vì Animator sẽ quản lý việc này)
            // if (solver != null) solver.enabled = true;

            // 4. Thực hiện di chuyển
            //await this.ikManager.MoveTargetToPointAsync(target, pos, 1f,token);
            await this.ikManager.MoveTargetDirectTestAsync(target, pos);
        }
        public Solver2D GetSolver2DByName(string name)
        {
            var ikCOntrol = this.GetComponentInChildrenByName<Transform>("IK-Control");
            if (ikCOntrol != null)
            {
                var target = ikCOntrol.GetComponentInChildrenByName<Transform>("solvers");
                if (target != null)
                {
                    return target.GetComponentInChildrenByName<Solver2D>(name);
                }
            }
            return null;
        }
        public Transform GetTargetByName(string name)
        {
            var ikCOntrol = this.GetComponentInChildrenByName<Transform>("IK-Control");
            if (ikCOntrol != null)
            {
                var target = ikCOntrol.GetComponentInChildrenByName<Transform>("targets");
                if (target != null)
                {
                    return target.GetComponentInChildrenByName<Transform>(name);
                }
            }
            return null;
        }
        public Vector2 GetReachPoint(Vector2 pos)
        {
            var dir = GetComponent<Collider2D>().bounds.center.GetDirectionTo(pos);
            if (dir < 0) return pos + new Vector2(this.ArmLen, 0);
            if (dir > 1) return pos + new Vector2(-this.ArmLen, 0);
            return pos;
        }
        public virtual float CalculateHeight()
        {
            if (this.headBone != null)
            {
                if (this.Footers != null && this.Footers.Length > 0)
                {
                    var headColl = this.headBone.GetComponent<Collider2D>();
                    var footerColl = this.Footers.FirstOrDefault().GetComponent<Collider2D>();
                    if (headColl != null && footerColl != null)
                    {
                        return headColl.bounds.max.y - footerColl.bounds.min.y;
                    }
                }
            }
            return 0;
        }
        public virtual void IninitStatus()
        {

            this.NormalHeight = CalculateHeight();
        }
        private void Awake()
        {
            if (Application.isPlaying)
            {
                _actor = GetComponent<UnvsActor>();
                IninitStatus();
            }
        }
        public void HoldItemInBackHand(MonoBehaviour item)
        {
            socketHandBack.AttachItemToSocket(item.transform);
            var bone = socketHandBack.parent;

            var sprite=this.GetComponentsInChildren<SpriteSkin>().FirstOrDefault(p=>p.boneTransforms.Contains(bone));
            if (sprite != null)
            {
                var spriteRender=sprite.GetComponent<SpriteRenderer>();
                if (spriteRender != null)
                {
                    item.SetSortingOrder(spriteRender.sortingOrder+1, spriteRender.sortingLayerName);
                }
            }
            item.SetMeOnLayer(Constants.Layers.HOLD_ITEM);
            currentHoldingItem = item.GetComponent<UnvsPickableObject>();
        }
        public void HoldItemInFrontHand(MonoBehaviour item)
        {
            socketHandFront.AttachItemToSocket(item.transform);
            var bone = socketHandBack.parent;

            var sprite = this.GetComponentsInChildren<SpriteSkin>().FirstOrDefault(p => p.boneTransforms.Contains(bone));
            if (sprite != null)
            {
                var spriteRender = sprite.GetComponent<SpriteRenderer>();
                if (spriteRender != null)
                {
                    item.SetSortingOrder(spriteRender.sortingOrder-1, spriteRender.sortingLayerName);
                }
            }
            item.SetMeOnLayer(Constants.Layers.HOLD_ITEM);
            currentHoldingItem = item.GetComponent<UnvsPickableObject>();
        }
    }
#if UNITY_EDITOR
    public partial class UnvsActorPhysical : UnvsBaseComponent
    {


        [UnvsButton("Create Hit box collider")]
        public void EditorCreateHitBoxCollider()
        {

            var cc = this.AddComponentIfNotExist<CompositeCollider2D>();
            cc.geometryType = CompositeCollider2D.GeometryType.Polygons;
            var lst = new List<Collider2D>();
            foreach (var footer in Footers)
            {
                var c = footer.AddComponentIfNotExist<PolygonCollider2D>();
                footer.SetMeOnTag(Constants.Tags.PLAYER_FOOTER);
                footer.gameObject.SetMeOnLayer(Constants.Layers.PLAYER_FOOTER);

                lst.Add(c);

                c.SetPath(0, footer.Collider2dGeneratePoints());
                c.compositeOperation = Collider2D.CompositeOperation.Merge;
            }
            if (this.headBone != null)
            {
                var c = headBone.AddComponentIfNotExist<PolygonCollider2D>();
                headBone.SetMeOnTag(Constants.Tags.PLAYER_HEADER);
                headBone.gameObject.SetMeOnLayer(Constants.Layers.PLAYER_HEADER);
                c.compositeOperation = Collider2D.CompositeOperation.Merge;

                c.SetPath(0, headBone.Collider2dGeneratePoints());
                lst.Add(c);

            }
            else
            {
                Debug.LogWarning($"headBone in {this.GetType()}.{name} is null");
            }
            HitBoxesCollider = lst.ToArray();
        }
        [UnvsButton("Validate")]
        public void EditorVaildate()
        {
            if (ArmTop == null || ArmRoot == null)
            {
                this.RaiseEditorError($"ArmTop and ArmRoot must be set on {name}");
            }
            ArmLen = Vector2.Distance(ArmTop.GetSegment().End, ArmRoot.GetSegment().Start);
            if (this.handBack == null || this.handFront == null)
            {
                this.RaiseEditorError($"handBack and handFront must be set on {name}");
            }
            this.socketHandBack = this.handBack.transform.CreateIfNoExist<Transform>("soket-hand-back");
            //this.socketHandBack.localPosition = new Vector3(0.5f, 0.5f, 0);
            this.socketHandFront = this.handFront.transform.CreateIfNoExist<Transform>("soket-hand-front");

            this.socketHandBack.SetMeOnTag(Constants.Tags.SOCKET);
            if (this.bodyBone == null)
            {
                this.RaiseEditorError($"bodyBone set on {name}, {this.GetType()}");
            }
            //this.socketHandFront.localPosition = new Vector3(0.5f, 0.5f, 0);
        }
    }
#endif
}