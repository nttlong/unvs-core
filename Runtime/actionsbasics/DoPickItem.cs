using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using unvs.actions;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.actors;

using unvs.shares;

namespace unvs.actionsbasics
{
    public class DoPickItem : ActionBase
    {
        public LocalizedString MsgFail;
        public LocalizedString MsgSuccess;
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await UniTask.Yield();
            var actor = Sender.GetTargetComponent<UnvsActor>();
            if (actor == null)
            {
                Sender.Cancel();
                return;
            }
            var st = Sender.GetSourceComponent<UnvsPickableObject>();
            var item = actor.motions.animStates[0];
            var animator = item.animationController;



            await actor.motions.animStates.PlayMotionAsync("bend-down", actor.cts.Token, "collect-item", null, null);

            actor.player.ControlDisable();
            await actor.motions.animStates.PlayMotionAsync("stand-up");
            actor.player.ControlEnable();
            actor.motions.BaseMotion("idle");
            actor.physical.HoldItemInBackHand(st);





        }
    }
}