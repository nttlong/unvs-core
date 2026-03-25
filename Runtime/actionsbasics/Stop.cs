using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using unvs.actions;
using unvs.interfaces;


namespace unvs.actionsbasics
{
    public class Stop : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender sender)
        {
            await UniTask.Yield();
            var target = sender.Target;
            if (target.IsDestroyed())
            {
                sender.Cancel();
                return;
            }
            var source = sender.Source;
            var actor = target.GetComponent<IActorObject>();
            if (actor == null)
            {
                sender.Cancel();
            }
            actor.Motion.Idle();

        }
    }
}