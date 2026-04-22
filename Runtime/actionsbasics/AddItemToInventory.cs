//using Cysharp.Threading.Tasks;
//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.Localization;
//using UnityEngine.Rendering;
//using unvs.actions;
//using unvs.ext;
//

//namespace unvs.actionsbasics
//{
//    public class InventoryAddItem : ActionBase
//    {
//        public async override UniTask ExecuteAsync(ActionBaseSender Sender)
//        {
//            var item = Sender.GetSourceComponent<IStoragableObject>();
//            var actor = Sender.GetTargetComponent<IActorObject>();
//            if (actor == null)
//            {
//                Sender.Cancel();
//                return;
//            }
//            if (item == null)
//            {
//               await  actor.Speaker.SayIThisDoesNotDoAnythingAsync();
//                Sender.Cancel();
//                return;
//            }
//            actor.Inventory.Add(Sender.Source);

//        }
//    }
//}