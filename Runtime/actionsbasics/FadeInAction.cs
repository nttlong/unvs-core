using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using unvs.actions;
using unvs.shares;

namespace unvs.actionsbasics
{
    public class FadeInAction : actions.ActionBase
    {
        public float fadingTime = 0.2f;
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await GlobalApplication.FadeScreenController.FadeInAsync(fadingTime);
        }
    }
    public class FadeOutAction : actions.ActionBase
    {
        public float fadingTime = 0.2f;
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await GlobalApplication.FadeScreenController.FadeOutAsync(fadingTime);
        }
    }
}