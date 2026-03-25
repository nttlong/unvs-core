using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace unvs.interfaces
{
    public interface IGlobalWorldBound
    {

        CompositeCollider2D Coll { get; }
        Rigidbody2D Rigidbody { get; }
        void AddBound(IScenePrefab newScenePrefab);
        void SetBound(IScenePrefab newScenePrefab);
        void RemoveBound(IScenePrefab newScenePrefab);
        void Clear();
        void ClearOrphanBound();

    }
}

