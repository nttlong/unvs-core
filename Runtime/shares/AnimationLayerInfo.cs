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
    [Serializable]
    public class BlendTreeInfo
    {
        public string motionName;
        public float value;
        public string layerName;
        public Animator animationController;
        public string paramName;
        public int index;
        public string blendName;
        public int blendIndex;
    }
}