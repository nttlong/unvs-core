using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;

namespace unvs.actionsbasics
{
    public class PickCarryableObject : ActionBase
    {
        float PreciseReachDistance = 0.1f;
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await UniTask.Yield();
            var actor = Sender.Target.GetComponent<IActorObject>();

            var obj = Sender.Source.GetComponent<ICarryableObject>();
            var pos = obj.Handle.position;
            var socketPosition = actor.Physical.SocketBack.position;
            var distance = Vector2.Distance(socketPosition, pos);
            //if (socketPosition.y - pos.y > PreciseReachDistance)
            //{


            //    await actor.Motion.BendDownAndPickItemAsync(() => { },Sender.Cts.Token);
               
            //}
            
            
            actor.Motion.Idle();

            actor.Physical.HoldItem(obj as MonoBehaviour);

            if (actor == null)
            {
                Sender.Cancel();
                return;
            }

        }
    }
}