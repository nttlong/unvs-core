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
            actor.motions.Motion("bend-down");



        }
    }
}