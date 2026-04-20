using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes.actors;

namespace unvs.actor.skills {
    public class ActorUnvsInteractSkill : AbstractActorBaseSkill
    {
        private UnvsActor _actor;
        private UnvsAnimStates _animState;
        private CompositeCollider2D _composite;
        public float MovingSpeed;
        public async UniTask MovtoTargetAsync(Vector2 vector2,Action<float> OnMoving, CancellationToken token)
        {
           
            
            _animState.BaseMotion("Walk");
            await  _actor.transform.MoveToTargetAsync(vector2, OnMoving, token, MovingSpeed);
        }
        public override void OnBind()
        {
            base.OnBind();
            _actor = Owner.GetComponent<UnvsActor>();
            _animState = _actor.GetComponent<UnvsAnimStates>();
            _composite = _actor.GetComponent<CompositeCollider2D>();
        }
        public override void OnPerform(Action OnCompleted = null)
        {
            
        }
    }
}