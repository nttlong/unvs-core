using System.Collections;
using UnityEngine;

namespace unvs.interfaces
{
    public interface IActorMovable
    {
        float WalkSpeed { get; set; }
        float RunSpeed { get; set; }

        void MoveTo(Vector2 dir);
    }
}