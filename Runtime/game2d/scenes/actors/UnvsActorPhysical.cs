using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;
using unvs.ext;
using unvs.game2d.objects;
using  unvs.shares;

namespace unvs.game2d.scenes.actors
{
    [Serializable]
    public struct MovingInfo
    {
        public bool isMoving;
        public Vector2 target;
        public float speed;
        public Vector2 direction2;

        public float JumpingHeight;

        public void MoveStep(Transform transform,Vector2 slopeDirection)
        {
            if (Application.isPlaying)
            {
                if (isMoving)
                {
                    if (slopeDirection == Vector2.zero)
                        transform.MoveStep(this.target, speed, out var dir, direction2.x);
                    else
                        //transform.MoveStep(this.target, speed, out var dir, direction2.x);
                        transform.position += (Vector3)slopeDirection * speed * Time.deltaTime;
                }
            }
           
        }
    }
    public partial class UnvsActorPhysical : UnvsBaseComponent
    {
        public float ArmLen;
        public Transform ArmTop;
        public Transform ArmRoot;
        public Transform handBack;
        public Transform handFront;
        public Transform socketHandBack;
        public Transform socketHandFront;
        private UnvsActorPhysicalSolverRuntime _socketHandBackController;
        private UnvsActorPhysicalSolverRuntime _socketHandFrontController;
        private IKManager2D _ikManager;
        [SerializeField]
        public MovingInfo movingInfo;
        internal UnvsPickableObject currentHoldingItem;
        private Collider2D _coll;
        [SerializeField]
        public Transform[] Footers;
        private  IKManager2D ikManager
        {
            get
            {
                if (_ikManager == null)
                {
                    _ikManager=this.GetComponentInChildren<IKManager2D>();
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
                    _socketHandBackController=new UnvsActorPhysicalSolverRuntime();
                    _socketHandBackController.target= GetTargetByName(socketHandBack.parent.name);
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
                    _socketHandFrontController.target= GetTargetByName(socketHandFront.parent.name);
                    //_socketHandFrontController.solver = GetSolver2DByName(socketHandFront.parent.name);
                }
               return _socketHandFrontController;
            }
        }
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
            if (ikCOntrol!=null)
            {
                var target = ikCOntrol.GetComponentInChildrenByName<Transform>("targets");
                if (target!=null)
                {
                    return target.GetComponentInChildrenByName<Transform>(name);
                }
            }
            return null;
        }
        public Vector2 GetReachPoint(Vector2 pos)
        {
            var dir=GetComponent<Collider2D>().bounds.center.GetDirectionTo(pos);
            if (dir < 0) return pos + new Vector2(this.ArmLen, 0);
            if(dir > 1) return pos + new Vector2(-this.ArmLen, 0);
            return pos;
        }
        private void Update()
        {
            if(_coll==null) _coll=GetComponent<Collider2D>();
            if(_coll==null) _coll=GetComponent<Collider2D>();
            var slopeDirection = _coll.CalculateSlopDirection(this.movingInfo.direction2.x);
           
            this.movingInfo.MoveStep(this.transform, slopeDirection);
            
        }

    }
#if UNITY_EDITOR
    public partial class UnvsActorPhysical : UnvsBaseComponent
    {

        [UnvsButton("Footer collider")]
        public void EditorCreateFooterCollider ()
        {
            foreach(var footer in Footers)
            {
                var c = footer.AddComponentIfNotExist<CapsuleCollider2D>();
                footer.SetMeOnTag(Constants.Tags.PLAYER_FOOTER);
                footer.gameObject.SetMeOnLayer(Constants.Layers.PLAYER_FOOTER);

            }
        }
        [UnvsButton("Validate")]
        public void EditorVaildate()
        {
            if(ArmTop==null||ArmRoot==null)
            {
                this.RaiseEditorError($"ArmTop and ArmRoot must be set on {name}");
            }
            ArmLen=Vector2.Distance( ArmTop.GetSegment().End, ArmRoot.GetSegment().Start );
            if(this.handBack==null || this.handFront == null)
            {
                this.RaiseEditorError($"handBack and handFront must be set on {name}");
            }
            this.socketHandBack = this.handBack.transform.CreateIfNoExist<Transform>("soket-hand-back");
            //this.socketHandBack.localPosition = new Vector3(0.5f, 0.5f, 0);
            this.socketHandFront = this.handFront.transform.CreateIfNoExist<Transform>("soket-hand-front");

            this.socketHandBack.SetMeOnTag(Constants.Tags.SOCKET);
            //this.socketHandFront.localPosition = new Vector3(0.5f, 0.5f, 0);
        }
    }
    #endif
}