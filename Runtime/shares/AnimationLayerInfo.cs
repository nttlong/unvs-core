using System;
using System.Collections;
using UnityEngine;

namespace unvs.shares
{
    [Serializable]
    public struct AnimationLayerInfo
    {
        public string Name;
        public int Index;
    }
    public class BlendTreeInfo
    {
        public string motionName;
        public float value;
    }
}