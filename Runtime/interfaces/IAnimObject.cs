using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unvs.shares;

namespace unvs.interfaces
{
    public interface IAnimObject
    {
        //ISpeakableObject
        public Animator Anim { get; }

        List<BlendTreeEntry> BlendTreeList { get; }
        void SetValue(string Layer, string VarName, string MotioName);


    }
}