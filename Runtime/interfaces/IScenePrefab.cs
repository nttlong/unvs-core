using PlasticGui.Help.Conditions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.shares;
namespace unvs.interfaces
{
    [Serializable]
    public struct OffsetFollowInfo
    {
        public bool ByRatio;
        public float OffsetValue;
        public bool IsEmpty;
        public static OffsetFollowInfo Default()
        {
            var ret = new OffsetFollowInfo();
            ret.IsEmpty = false;
            ret.ByRatio = true;
            ret.OffsetValue = 0.33f;
            return ret;
        }

        public static OffsetFollowInfo Empty()
        {
            var ret = new OffsetFollowInfo();
            ret.IsEmpty = true;
         
            return ret;
        }
    }
    [Serializable]
    public struct OffsetFollow
    {
        public bool IsEmpty;
        [SerializeField]
        public OffsetFollowInfo x;
        [SerializeField]
        public OffsetFollowInfo y;
        public static OffsetFollow Empty()
        {
            var ret= new OffsetFollow();
            ret.IsEmpty= true;
            ret.x = OffsetFollowInfo.Empty();
            ret.y = OffsetFollowInfo.Empty();
            return ret;
        }

        public static OffsetFollow DefaultNew()
        {
            var ret = new OffsetFollow();
            ret.IsEmpty = false;
            ret.x = OffsetFollowInfo.Empty();
            ret.y = OffsetFollowInfo.Default();
            return ret;
        }
    }
    public interface IScenePrefab
    {
        OffsetFollow CameraOffsetFolow { get; }

        AudioSource Audio { get; }
        AudioInfo Ambient { get; }
        bool GizmosDraw { get; }
        IWorldGlobalLight Globalight { get; }
        public IWorldJoinInfoObject JoinInfo { get; }
        bool IsDestroying { get; set; }
        IScenePrefabWorldBound WorldBound { get; }
        event Action<IScenePrefab> OnReachLimit;

        event Action OnSceneDestroy;
        Action<IScenePrefab> OnDestroyMe { get; set; }
        public string Name { get; }
        public GameObject GoWorld { get; }
        public IActorObject Actor { get; }
        public ISpawnTarget StartPos { get; }


        public float OrthographicSize { get; }
        public PhysicsMaterial2D FloorMaterial { get; set; }
        public EdgeCollider2D Floor { get; }

        public BoxCollider2D LeftWall { get; }
        public BoxCollider2D RightWall { get; }
        public ITriggerZone LeftTriggerZone { get; }
        public ITriggerZone RightTriggerZone { get; }

        void RemoveActor();
        void SetupActor(IActorObject actor, string spawnName);
        ISpawnTarget FindSpawnTargetByName(string spawnName);
        void TrimEdge();
        string GetLeftScenePath();
        string GetRightScenePath();
        void GlobalightRestore();

        bool IsReszieWorldBound { get; }

        //PolygonCollider2D RefColliderWorldBound { get; set; }
        IScenePrefab Left { get; set; }
        IScenePrefab Right { get; set; }
        /// <summary>
        /// Use for tracking when camwacher hit
        /// Usage: change environment of hit scene and meny thing
        /// </summary>
        IWorldTracker WorkTracker { get; }


    }
}
