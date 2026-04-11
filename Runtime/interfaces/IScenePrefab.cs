
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

        public Vector3 CalculateOffset(float orthographicSize)
        {
            float x = 0;
            float y = 0;
            if (!this.x.IsEmpty)
            {
                x = this.x.OffsetValue;
                if (this.x.ByRatio)
                {
                    x = orthographicSize * x;
                }
            }
            if (!this.y.IsEmpty)
            {
                y = this.y.OffsetValue;
                if (this.y.ByRatio)
                {
                    y = orthographicSize * y;
                }
            }
            var v = new Vector3(x, y, -10);
            return v;

        }
    }
    [Serializable]
    public struct ViewInfo
    {
        [SerializeField]
        public Vector2 Size;
        [SerializeField] public Vector2 Center;
        [SerializeField]
        public Vector2 Offset;

        public void Calculate(OffsetFollow cameraOffsetFolow, float orthographicSize)
        {
           Size=Commons.GetCameraWorldSizeEditorMode(orthographicSize);
            Offset = cameraOffsetFolow.CalculateOffset(orthographicSize);
        }
    }
    //public interface IScenePrefab
    //{
    //    ViewInfo View {  get; }
    //    GameObject DefaultCamWatcher { get; }
    //    OffsetFollow CameraOffsetFolow { get; }

    //    AudioSource Audio { get; }
    //    AudioInfo Ambient { get; }
    //    bool GizmosDraw { get; }
    //    IWorldGlobalLight Globalight { get; }
    //    public IWorldJoinInfoObject JoinInfo { get; }
    //    bool IsDestroying { get; set; }
    //    /// <summary>
    //    /// Limitation of Scene
    //    /// Each Scene has its own a World-Bounding for Limitation of Camera movement
    //    /// Mỗi cảnh đều có giới hạn riêng về phạm vi di chuyển của máy quay.
    //    /// </summary>
    //    //IScenePrefabWorldBound WorldBound { get; }
    //    event Action<IScenePrefab> OnReachLimit;

    //    event Action OnSceneDestroy;
    //    Action<IScenePrefab> OnDestroyMe { get; set; }
    //    public string Name { get; }
    //    /// <summary>
    //    /// Gameobject present for Scene-Prefab
    //    /// </summary>
    //    public GameObject GoWorld { get; }
    //    public IActorObject Actor { get; }
    //    public ISpawnTarget StartPos { get; }


    //    public float OrthographicSize { get; }
    //    public PhysicsMaterial2D FloorMaterial { get; set; }
    //    public EdgeCollider2D Floor { get; }
    //    /// <summary>
    //    /// Left wall: prevents the actor from passing through.
    //    /// </summary>
    //    public BoxCollider2D LeftWall { get; }
    //    /// <summary>
    //    /// Right wall: prevents the actor from passing through.
    //    /// </summary>
    //    public BoxCollider2D RightWall { get; }
    //    /// <summary>
    //    /// Triggers loading of the left scene
    //    /// </summary>
    //    //public ITriggerZone LeftTriggerZone { get; }
    //    ///// <summary>
    //    ///// Triggers loading of the right scene
    //    ///// </summary>
    //    //public ITriggerZone RightTriggerZone { get; }

    //    void RemoveActor();
    //    void SetupActor(IActorObject actor, string spawnName);
    //    ISpawnTarget FindSpawnTargetByName(string spawnName);
    //    void TrimEdge();
    //    ///// <summary>
    //    ///// Get left scene path
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //string GetLeftScenePath();
    //    ///// <summary>
    //    ///// Get right scene path
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //string GetRightScenePath();
    //    /// <summary>
    //    /// Restore global light. Global light of each scene will be added to Single Scene for light calculation, 
    //    /// When the scene put behind (tempo hidden) call this function
    //    /// Khôi phục ánh sáng toàn cục. Ánh sáng toàn cục của mỗi cảnh sẽ được thêm vào Cảnh đơn để tính toán ánh sáng. 
    //    /// Khi cảnh được đặt phía sau (tạm thời ẩn), hãy gọi hàm này.
    //    /// </summary>
    //    void GlobalightRestore();

    //    bool IsReszieWorldBound { get; }

    //    //PolygonCollider2D RefColliderWorldBound { get; set; }
    //    IScenePrefab Left { get; set; }
    //    IScenePrefab Right { get; set; }
    //    /// <summary>
    //    /// Use for tracking when camwacher hit
    //    /// Usage: change environment of hit scene and meny thing
    //    /// </summary>
    //    IWorldTracker WorkTracker { get; }


    //}
}
