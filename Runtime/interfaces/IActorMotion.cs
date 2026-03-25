using System.Collections;
using UnityEngine;

namespace unvs.interfaces
{
    public interface IActorMotion
    {
        Animator Anim { get; }

        void Flip(float x);
        void Idle();
    }
}