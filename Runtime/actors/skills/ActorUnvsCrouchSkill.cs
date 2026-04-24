using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using unvs.actor.skills;
using unvs.ext;
using unvs.ext.physical2d;
using unvs.game2d.actors;

namespace unvs.actor.skills {
    
    public class ActorUnvsCrouchSkill :ActorDefaultSkill
    {
        [SerializeField]
        public AnimatorOverrideController controller;
        private UnvsActor _actor;
        private RuntimeAnimatorController _originalController;
        private bool _hasStarted;
        private float _trackDistanceUp = -1;

        public override void OnPerform(Action OnCompleted = null)
        {
            if (coll == null) return;
            if (_lastSpeed != CurrentSpeed)
            {

                if (CurrentSpeed <= 0)
                {
                    motions.animStates.PlayBaseLayer("Idle", "", controller);
                }
                if (CurrentSpeed == this.MoveSpeed)
                {
                    motions.animStates.PlayBaseLayer("Walk", "", controller);

                }

                _lastSpeed = CurrentSpeed;
            }
            ref var r = ref this._slopDirectionResult;
            coll.CalculateSlopDirection(ref r, Direction.x);

            this.MoveStep(Owner.transform, r.slopeDir,0);
        }

        public override async UniTask<bool> StartAysnc(CancellationToken tk = default)
        {

            var ret = await base.StartAysnc(tk);
            if (_actor == null)

                _actor = Owner.GetComponent<UnvsActor>();
            var compositeColl = _actor.GetComponent<CompositeCollider2D>();
            if (compositeColl != null)
            {
                _trackDistanceUp = compositeColl.bounds.size.y;
            }
            if (!_actor.IsValidate()) return false;
            _originalController = _actor.animator.runtimeAnimatorController;
            await motions.MotionAsync("crouch-start", tk);
            _actor.animator.runtimeAnimatorController = this.controller;
            _hasStarted = true;

            //_trackDistanceUp = _trackDistanceUp - compositeColl.bounds.size.y;
            return ret;
        }
        public bool IsHitTopGround(float distance = -1f)
        {
            _actor = Owner.GetComponent<UnvsActor>();
            if (!_actor.IsValidate()) return false;
            var compositeColl = _actor.GetComponent<CompositeCollider2D>();
            if (distance < 0)
            {
                if (_trackDistanceUp < 0)
                    distance = compositeColl.bounds.size.y;
                else
                    distance = _trackDistanceUp;
            }
            if (compositeColl.GetHit(out var hit, Vector2.up, distance))
            {
                return true;
            }
            return false;
        }
        public override async UniTask<bool> StopAsync(CancellationToken tk = default)
        {
            var compositeColl = _actor.GetComponent<CompositeCollider2D>();
            if (IsHitTopGround())
            {

                return false;
            }

            var ret = await base.StopAsync(tk);

            //var dx = 1f;
            await motions.MotionAsync("crouch-end", tk);
            _actor.animator.runtimeAnimatorController = _originalController;
            _hasStarted = false;
            return ret;
        }
    }
}