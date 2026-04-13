using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.IK;
using unvs.ext;
using  unvs.shares;

namespace unvs.game2d.scenes.actors
{
    
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
                    _socketHandBackController.solver = GetSolver2DByName(socketHandBack.parent.name);
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
                    _socketHandFrontController.target= GetTargetByName(socketHandBack.parent.name);
                    _socketHandFrontController.solver = GetSolver2DByName(socketHandBack.parent.name);
                }
               return _socketHandFrontController;
            }
        }
        public async UniTask MoveSocketHandBackToAsync(Vector2 pos, float duration = 1f, CancellationToken token = default)
        {
            // Kiểm tra an toàn trước khi chạy

            await this.ikManager.MoveTargetToPointAsync(socketHandFrontController.target, pos, duration, token);
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
       

    }
#if UNITY_EDITOR
    public partial class UnvsActorPhysical : UnvsBaseComponent
    {
        
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
            this.socketHandBack.localPosition = new Vector3(0.5f, 0.5f, 0);
            this.socketHandFront = this.handFront.transform.CreateIfNoExist<Transform>("soket-hand-front");
            this.socketHandFront.localPosition = new Vector3(0.5f, 0.5f, 0);
        }
    }
    #endif
}