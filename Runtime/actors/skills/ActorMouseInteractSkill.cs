using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.actors;
using unvs.shares;

namespace unvs.actor.skills {
    /// <summary>
    /// Mouse interact actor
    /// </summary>
    public class ActorMouseInteractSkill : ActorDefaultSkill
    {
        private UnvsActor _actor;
        private CompositeCollider2D _composite;

        public async UniTask InteractAsync(Vector2 pos)
        {
            await UniTask.Yield();
            // Debug actor show interact point
         
            // adjust direction of actor following pos
            Direction = new Vector2( _composite.bounds.center.CalculateDirection(pos),0);
            //check does positon hit Interactable Object
           
            var objInteract = pos.ToScreen().GetHitCollider<UnvsInteractObject>(Constants.Layers.INTERACT_OBJECT);
            if( objInteract != null )
            {
                // Now, cancel all before action by using _actor.RefreshToken()
                // and make an interaction from objInteract
                await objInteract.ExecuteAsync(_actor, _actor.RefreshToken());
            } else // nothing can interact
            {
                _actor.RefreshToken();
                // so, Actor now moving
                CurrentSpeed = SprintSpeed;
            }
                
            
        }
        public override void OnBind()
        {
            base.OnBind();
            _actor = Owner.GetComponent<UnvsActor>();
            _composite=Owner.GetComponent<CompositeCollider2D>();
        }
    }
}