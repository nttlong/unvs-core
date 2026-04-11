//using Cysharp.Threading.Tasks;
//using Cysharp.Threading.Tasks.Triggers;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEngine;
//using unvs.actions;
//using unvs.ext;
//using unvs.interfaces;
//using unvs.players;
//using unvs.shares;

//namespace unvs.playerobjects.actions
//{
//    public class PlayerMoveTo : ActionBase
//    {
//        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
//        {
//            await UniTask.Yield();
//            var player = Sender.GetTargetComponent<PlayerBase>();
//            if(player==null)
//            {
//                Sender.Cancel();
//                return;
//            }
//            player.dialogue.SayText("OK");
//            var interactObj = Sender.GetSourceComponent<PlayerInteractObject>();
//            if(interactObj==null)
//            {
//                Sender.Cancel();
//                return;
//            }
//            await player.physical.MoveToAsync(interactObj.GetPosition(), Sender.Cts);
//        }
//    }
//}
