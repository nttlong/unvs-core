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
    public struct CalculateSlopeDirectionResull
    {
        public Vector2 slopeDir;
        public Collider2D hitCollider;
        public bool IsHit;
    }
}