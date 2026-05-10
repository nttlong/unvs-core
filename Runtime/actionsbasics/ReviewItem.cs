using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using unvs.actions;
using unvs.actor.skills;
using unvs.game2d.objects;
using unvs.game2d.actors;
using unvs.ui;

namespace unvs.actionsbasics
{
    public class ReviewItem : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            var item=Sender.GetSourceComponent<UnvsCollectableItem>();
            if(item!=null)
            {
                Sender.Cancel();
                return;
            }
            UnvsDialog.Instance.DoReviewItem(item.icon.srpite, item.Description);
            await UniTask.Yield();
        }
    }
}