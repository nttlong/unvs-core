using Cysharp.Threading.Tasks;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using UnityEngine;
using unvs.actions;
using unvs.interfaces;

namespace unvs.actionsbasics
{
    public class MoveToPickabeItem : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            var actor=Sender.Target.GetComponent<IActorObject>();
            if(actor==null)
            {
                Sender.Cancel();
                return;
            }
            var pickableItem = Sender.Source.GetComponent<IPickableObject>();
            if (pickableItem == null)
            {
               await actor.Speaker.SayIThisDoesNotDoAnythingAsync();
            }
            float dir = actor.Movable.Direction.x;
            var lengOfArm = actor.Physical.ArmLen;
            actor.Motion.Walk();
            var p = pickableItem.GetPosition(dir, actor.Physical.ArmLen);
            p = new Vector2(p.x, 0);
            await actor.Movable.MoveToAsync(p, dir =>
            {

            }, () =>
            {
                actor.Motion.Idle();
            }, Sender.Cts.Token);
        }
    }
}