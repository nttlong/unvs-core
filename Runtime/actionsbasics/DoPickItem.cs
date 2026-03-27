using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;

namespace unvs.actionsbasics
{
    public class DoPickItem : ActionBase
    {
        public LocalizedString MsgFail;
        public LocalizedString MsgSuccess;
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
           IActorObject actor= Sender.GetTargetComponent<IActorObject>();
            if (actor == null)
            {
                Sender.Cancel();
                return;
            }
           IStoragableObject st= Sender.GetSourceComponent<IStoragableObject>();
            if(st == null)
            {
               await actor.Speaker.SayIThisDoesNotDoAnythingAsync();
                Sender.Cancel();
                return;
            }
            IInteractableObject interactObj= Sender.GetSourceComponent<IInteractableObject>();

            if (interactObj == null)
            {
                await actor.Speaker.SayIThisDoesNotDoAnythingAsync();
                Sender.Cancel();
                return;
            }
            var pos= interactObj.GetPosition();
           
            var handPosition = actor.Physical.GetHandPosition();
            var d = Vector2.Distance(actor.Physical.GetPosition(), pos); //Vector2.Distance(GetPosition(),pos)
            var v = math.abs(Vector2.Distance(actor.Physical.GetPosition(), pos)- actor.Physical.ArmLen);
            if (!actor.Physical.CanReachTarget(pos))
            {
               
                actor.Speaker.SayText($"{v<=0.05f},No {d},{actor.Physical.ArmLen}");
            } else
            {
                if (actor.Physical.IsTargetLower(pos))
                {
                   await  actor.Motion.BendDownAndPickItemAsync();

                } else
                {

                    await actor.Motion.PickItemAsync();
                    // if (MsgFail.IsValid()) await actor.Speaker.SayAsync(MsgFail);
                    //await actor.Speaker.SayICanNotDoThatAsync();
                }
                    
            }
            actor.Motion.Idle();


        }
    }
}