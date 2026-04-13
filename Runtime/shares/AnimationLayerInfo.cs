using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.IK;

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
        public int layerIndex;
        public string blendName;
        public int blendIndex;
    }
    [Serializable]
    public struct AudioInfo
    {
        [SerializeField]
        public AudioClip Clip;
        [Range(0,1)]
        [SerializeField]
        public float volume;

        public bool IsEmpty()
        {
            return Clip == null;
        }
        public static AudioInfo EmptyNew()
        {
            return new AudioInfo()
            {
                volume = 1f
            };
        }
        
    }
    [Serializable]
    public struct UnvsActorPhysicalSolverRuntime
    {
        public Transform target;
        //public Solver2D solver;

        public bool IsEmpty()
        {
            return target == null;
        }
    }
}