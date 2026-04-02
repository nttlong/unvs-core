using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;


namespace unvs.actors
{
    public class ActorMovableObject : MonoBehaviour,IActorMovable
    {
        IActorMovable instance;
        public float walkSpeed;
        public float runSpeed;

        public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
        public float RunSpeed { get => runSpeed; set => runSpeed = value; }
        public Vector2 Direction { get; set ; }

        public void MoveTo(Vector2 dir)
        {
            transform.MoveStepByDirection(dir, instance.WalkSpeed);
        }
        private void Awake()
        {
            instance = this as IActorMovable;
        }

        public async UniTask MoveToAsync(Vector2 Pos, Action<float> OnMoving, Action Stop, CancellationToken ct,float distance=0)
        {
            if (ct == null)
            {
                return;
            }

            if (ct.IsCancellationRequested) return;
            ct.ThrowIfCancellationRequested();
            var speaker = GetComponent<ISpeakableObject>();

            try
            {
               


               

                //var dir = this.Coll.bounds.center.x < Pos.x ? 1 : -1;
                //Physical.Direction = dir > 0 ? DirectionEnum.Forward : DirectionEnum.Backward;

                var ret = await this.transform.MoveToAsync(WalkSpeed, Pos, p =>
                {
                    var x = p.Direction ;
                    this.Direction = new Vector2(x, Direction.y);
                    OnMoving?.Invoke(x);
                }, p =>
                {
                    Stop?.Invoke();
                }, ct,distance);
                //if (!ct.IsCancellationRequested) // if not cancel -> finished routine

            }
            catch (System.OperationCanceledException)
            {

                return;
            }
        }

        public Vector2 GetPostion()
        {
            var coll=GetComponent<Collider2D>();
            if( coll == null ) return Vector2.zero;
            return coll.bounds.center;
        }
    }
}