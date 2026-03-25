using System.Collections;
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

        public void MoveTo(Vector2 dir)
        {
            transform.MoveStepByDirection(dir, instance.WalkSpeed);
        }
        private void Awake()
        {
            instance = this as IActorMovable;
        }
    }
}