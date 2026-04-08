/*
    this file define Virtuale scene, acctually, that is prefab of scene
    it contain Main cam
   
   
*/
using game2d.ext;
using game2d.scenes;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using unvs.ext;
using unvs.shares;

namespace unvs.game2d.scenes
{
    
    public class VScene : UnvsComponent
    {
        public Camera cam;
        public CinemachineCamera vcam;
        public Transform defaulCamWatcher;
        public CinemachineConfiner2D confiner;
        public CinemachineFollow cinemachineFollow;
        [SerializeField]
        public Vector2 defaultSize;
        public EdgeCollider2D edgesWorldBound;
        public Transform startPoint;
        [SerializeField]
        private Vector2 followOffset;
        private PolygonCollider2D worldBound;

        public override void InitRuntime()
        {
            DestroyImmediate(cam.gameObject);
        }


#if UNITY_EDITOR
        [InspectorButton("Generate elements")]
        public void Generate()
        {
            this.cam = this.AddChildComponentIfNotExist<Camera>("Main Camera");
            this.cam.tag = "MainCamera";
            this.cam.orthographic = true;
            this.cam.AddComponentIfNotExist<CinemachineBrain>();
            this.vcam = this.AddChildComponentIfNotExist<CinemachineCamera>("vcam");
            this.defaulCamWatcher = this.AddChildComponentIfNotExist<Transform>("default-cam-watcher");
            this.confiner = this.vcam.GetOrAddComponent<CinemachineConfiner2D>();
            this.cinemachineFollow = this.vcam.GetOrAddComponent<CinemachineFollow>();
            this.vcam.Follow = this.defaulCamWatcher;
            this.defaultSize = this.cam.GetCameraWorldSize();
            this.edgesWorldBound = this.AddChildComponentIfNotExist<EdgeCollider2D>("edgesWorldBound");
            this.startPoint = this.AddChildComponentIfNotExist<Transform>("start-point");
            this.startPoint.transform.position = new Vector3(this.defaultSize.x / 2, 0, 0);
            this.defaulCamWatcher.transform.position = this.defaultSize / 2;
            this.worldBound = this.AddChildComponentIfNotExist<PolygonCollider2D>("world-bound");
            this.worldBound.points = this.defaulCamWatcher.position.CreateRectFromCenter(this.defaultSize);
            //this.edgesWorldBound.points=Vector2dExtesion.
        }
        private void OnValidate()
        {
            if (this.cinemachineFollow != null)
            {
                this.followOffset = this.cinemachineFollow.FollowOffset;
            }
        }
        private void OnDrawGizmos()
        {
            if (this.worldBound != null) this.worldBound.GizmosDraw(Color.green, 1);
            if (this.defaulCamWatcher != null) this.defaulCamWatcher.transform.position.DrawCircle(1f,Color.red);
        }

#endif
    }
}