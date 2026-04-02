using Cysharp.Threading.Tasks;

using System.Collections;
using UnityEngine;
using unvs.actions;
using unvs.ext;
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
            actor.Movable.Direction = ((Vector2)Sender.GetSourceComponent<BoxCollider2D>().bounds.center - actor.Physical.GetPosition()).CalculateDiection();
            var lengOfArm = actor.Physical.ArmLen;
            actor.Motion.Flip(actor.Movable.Direction.x);
            actor.Motion.Walk();
            actor.Speaker.SayText($"{actor.Movable.Direction},{actor.Physical.ArmLen}");
            var p = pickableItem.GetPosition(actor.Movable.Direction.x, actor.Physical.ArmLen);
            p = new Vector2(p.x, 0);
            await actor.Movable.MoveToAsync(p, dir =>
            {
                actor.Physical.Direction = (dir > 0) ? DirectionEnum.Forward : DirectionEnum.Backward;
                actor.Motion.Flip(dir);
                actor.Motion.Walk();
            }, () =>
            {
                actor.Motion.Idle();
            }, Sender.Cts.Token);
        }
    }
}