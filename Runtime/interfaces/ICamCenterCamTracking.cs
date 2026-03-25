using System.Collections;
using UnityEngine;
using unvs.interfaces;

namespace unvs.interfaces
{
    public interface ICamCenterCamTracking
    {
        BoxCollider2D Collider { get; }
        IScenePrefab[] ScensHit { get; }
        void AddScene(IScenePrefab sc);
        void RemoveScene(IScenePrefab sc);

        void Clear();
    }
}