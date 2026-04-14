using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using unvs.actions;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.scenes.actors;
using unvs.interfaces;

namespace unvs.actionsbasics
{
    public class DoPickItem : ActionBase
    {
        public LocalizedString MsgFail;
        public LocalizedString MsgSuccess;
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await UniTask.Yield();
            var  actor = Sender.GetTargetComponent<UnvsActor>();
            if (actor == null)
            {
                Sender.Cancel();
                return;
            }
            var st = Sender.GetSourceComponent<UnvsPickableObject>();
            var item = actor.motions.animStates[0];
            var animator = item.animationController;
          

            //animator.SetLayerWeight(bendLayer, 1f);
            //animator.Play("bend-down", bendLayer, 0f);

            //animator.SetLayerWeight(pickLayer, 1f);
            //animator.Play("collect-item", pickLayer, 0f);
            //actor.motions.animStates.PlayMotion("bend-down", "collect-item");
            //await actor.motions.animStates.PlayMotionAsync("bend-down");
           
            //UnvsActorPhysicalSolverRuntimeExt.MoveToAsync(socketController, st.GetPosition(), Sender.Cts.Token);
            //await socketController.target.MoveToTargetAsync( st.GetPosition(), Sender.Cts.Token);
            var pos= new Vector2(actor.coll.bounds.center.x-actor.physical.ArmLen,actor.coll.bounds.max.y);
            await actor.physical.MoveSocketHandBackToAsync(pos, 1f, Sender.Cts.Token);
            //await actor.motions.animStates.PlayMotionAsync("stand-up");

            ////  await actor.motions.animStates.PlayMotionAsync("collect-item", "collect-item");
            ////  actor.motions.Motion("stand-up");

            ////actor.motions.BaseMotion("idle");
            ////actor.motions.AddtiveMotion("collect-item");
            actor.motions.BaseMotion("idle");


        }
    }
}