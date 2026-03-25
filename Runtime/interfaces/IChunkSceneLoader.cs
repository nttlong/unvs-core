using System;
using System.Collections;
using UnityEngine;
using unvs.interfaces;
using unvs.shares;
namespace unvs.interfaces
{
    [Obsolete("Depreciate use ISceneLoader")]
    public interface IChunkSceneLoader
    {
        Transform Container { get; }
        Action<float> OnHitWall { get; set; }
        IActor actor { get; }
        IScenePrefab CurrentScene { get; set; }
        BoxCollider2D Left { get; set; }
        BoxCollider2D Right { get; set; }
        ChunkSceneRange CurrentRange { get; set; }
        float TrackRangeSize { get; set; }

        Vector2 CalutateDistance(ChunkSceneRange range, Vector3 center);
        ChunkSceneRange GetCurrentRange();
        string GetScenePath(float direction, out IElasticScene scene);
    }
}