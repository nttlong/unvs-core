using System;
using UnityEngine;

namespace unvs.game2d.transitions
{
    [Serializable]
    public abstract class UnvsTransitionBase
    {
        public string name;
        public abstract void Execute(MonoBehaviour source);
    }
}