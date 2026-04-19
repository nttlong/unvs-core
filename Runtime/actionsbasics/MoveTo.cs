using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using unvs.actions;
using unvs.actor.skills;
using unvs.game2d.objects;
using unvs.game2d.scenes.actors;
using unvs.interfaces;
namespace unvs.actionsbasics
{
    public class MoveTo : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender sender)
        {
            var source = sender.Source;
            var target = sender.Target;

            await UniTask.Yield();
            var iteractSource = source.GetComponent<UnvsInteractObject>();
            if (iteractSource == null)
            {
                sender.Cancel();
                return;
            }
            var actor = target.GetComponent<UnvsActor>();
            if (actor != null)
            {
                var skill = actor.Skills.Get<ActorUnvsInteractSkill>();
                if(skill==null)
                {
                    sender.Cancel();
                    return;
                }
                await skill.MovtoTargetAsync(iteractSource.GetPosition(), sender.Cts.Token);
            }
            else
            {
                sender.Cancel(); // sender.Cts se bi huy bo trong ham nay

            }





            //await movableObject.MoveToAsync(iteractSource.GetPosition(), sender.Cts);

        }
    }
}
