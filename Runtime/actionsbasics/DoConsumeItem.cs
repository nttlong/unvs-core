//using Cysharp.Threading.Tasks;
//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.Localization;
//using UnityEngine.Rendering;
//using unvs.actions;
//using unvs.ext;
//using unvs.interfaces;

//namespace unvs.actionsbasics
//{
//    public class DoConsumeItem : ActionBase
//    {
//        public override  async UniTask ExecuteAsync(ActionBaseSender Sender)
//        {
//            var movetoAction = new MoveTo();
//            var item = Sender.Target.GetComponent<IConsumableItem>();
//            if (item == null)
//            {
//                return;
//            }
//            await movetoAction.ExecuteAsync(new ActionBaseSender
//            {
//                Cts=Sender.Cts,
//                Source=Sender.Source,
//                Target= item.Owner as MonoBehaviour,
//            });
//        }
//    }
//}