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