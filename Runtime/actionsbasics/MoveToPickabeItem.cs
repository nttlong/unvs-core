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
               await actor.Speaker.SayICanNotDoThatAsync();
            }
            float dir = actor.Movable.Direction.x;
            var lengOfArm = actor.Physical.ArmLen;
            actor.Motion.Walk();
            await actor.Movable.MoveToAsync(pickableItem.GetPosition(dir, actor.Physical.ArmLen), dir =>
            {

            }, () =>
            {
                actor.Motion.Idle();
            }, Sender.Cts.Token);
        }
    }
}