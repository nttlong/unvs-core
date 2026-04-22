//using Cysharp.Threading.Tasks;
//using System.Collections;
//using UnityEngine;
//using unvs.actions;
//

//namespace unvs.actionsbasics
//{
//    public class MoveToCarryableObject : ActionBase
//    {
//        public float PreciseDistance = 0.1f;
//        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
//        {
//            var actor=Sender.Target.GetComponent<IActorObject>();
//            var pickObj= Sender.Source.GetComponent<ICarryableObject>();
//            if(pickObj==null|| actor==null)
//            {
//                Sender.Cancel(); return;
//            }
//            var pos = pickObj.Handle.position;
//            var armLen= actor.Physical.ArmLen;
//            var dir = actor.Coll.bounds.center.x < pos.x ? 1 : -1;
//            pos = new Vector2(pos.x-dir*armLen,pos.y);
//            if(Mathf.Abs( actor.Coll.bounds.center.x- pos.x)> PreciseDistance){
//                await actor.MoveToAsync(pos, Sender.Cts);
//            }
           
//            actor.Physical.Direction = dir > 0 ? DirectionEnum.Forward : DirectionEnum.Backward;
//            actor.Motion.Flip(dir);
//            actor.Motion.Idle();

//        }
//    }
//}