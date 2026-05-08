using Cysharp.Threading.Tasks;
using unvs.actions;
using unvs.components;
using unvs.game2d.objects;

namespace unvs.actionsbasics
{
    public class DoCollectItem : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await UniTask.Yield();
            var bagger = Sender.GetTargetComponent<UnvsBagger>();
            if (bagger==null)
            {
                Sender.Cancel();
                return;
            }
            var item=Sender.GetSourceComponent<UnvsCollectableItem>();
            if(item==null)
            {
                Sender.Cancel();
                return;
            }
            bagger.AddItem(item);

        }
    }
}