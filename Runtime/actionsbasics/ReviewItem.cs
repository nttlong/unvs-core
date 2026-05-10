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
using unvs.game2d.scenes;

namespace unvs.actionsbasics
{
    public class ReviewItem : ActionBase
    {
        [SerializeField]
        public types.AudioInfo feedbackAuio;
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
           
            var item=Sender.GetSourceComponent<UnvsCollectableItem>();
            
            if(item==null)
            {
                Sender.Cancel();
                return;
            }
            var itemSprite = item.GetInventoryIcon();
            UnvsApp.SayText($"itemSprite={itemSprite}");
            UnvsDialog.Instance.HideFooter();
            await UnvsDialog.Instance.DoReviewItemAsync(itemSprite, item.Description);
            
            await UniTask.Yield();
        }
    }
    public class ShowInventoty : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            UnvsUIInventory.Instance.Show();
            await UniTask.Yield();
        }
    }
}