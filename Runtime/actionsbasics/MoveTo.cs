using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using unvs.actions;
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
            var iteractSource = source.GetComponent<IInteractableObject>();
            if (iteractSource == null)
            {
                sender.Cancel();
                return;
            }
            var actor = target.GetComponent<IActorObject>();
            if (actor != null)
            {
                actor.Motion.Walk();
                await actor.MoveToAsync(iteractSource.GetPosition(), sender.Cts);
            }
            else
            {
                sender.Cancel();

            }

            //await movableObject.MoveToAsync(iteractSource.GetPosition(),  sender.Cts);

        }
    }
}
