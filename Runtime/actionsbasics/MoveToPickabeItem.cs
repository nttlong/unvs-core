using Cysharp.Threading.Tasks;

using System.Collections;
using UnityEngine;
using unvs.actions;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.scenes.actors;
using unvs.interfaces;

namespace unvs.actionsbasics
{
    public class MoveToPickabeItem : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            var actor = Sender.Target.GetComponent<UnvsActor>();
            if (actor == null)
            {
                Sender.Cancel();
                return;
            }
            var pickableItem = Sender.Source.GetComponent<UnvsPickableObject>();
            if (pickableItem == null)
            {
                actor.speaker.SayIThisDoesNotDoAnything();
            }
            Vector2 pos = actor.physical.GetReachPoint(pickableItem.GetPosition());

            await actor.MovtoTargetAsync(pos,  Sender.Cts.Token);
            actor.motions.direction = actor.coll.bounds.center.GetDirectionTo(pickableItem.GetPosition());
        }
    }
}