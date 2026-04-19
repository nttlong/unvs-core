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

        public async UniTask MovtoTargetAsync(Vector2 vector2, CancellationToken token)
        {
            _animState.BaseMotion("walk");
           await  _actor.transform.MoveToTargetAsync(vector2, token);
        }
        public override void OnBind()
        {
            base.OnBind();
            _actor = Owner.GetComponent<UnvsActor>();
            _animState = _actor.GetComponent<UnvsAnimStates>();
        }
        public override void OnPerform(Action OnCompleted = null)
        {
            
        }
    }
}