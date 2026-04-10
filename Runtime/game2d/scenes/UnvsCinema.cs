using Cysharp.Threading.Tasks;
using game2d.scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;

namespace unvs.game2d.scenes{
    public class UnvsCinema : UnvsUIComponentInstance<UnvsCinema>
    {
        public Camera cam;
        public CinemachineCamera vcam;
        public CompositeCollider2D compositeCollider2D;
        public CinemachineConfiner2D confiner;
       
        public BoxCollider2D camColl;
        private MonoBehaviour camTracking;
        Dictionary<UnvsScene,PolygonCollider2D> worldBoundDict=new Dictionary<UnvsScene, PolygonCollider2D>();
        public PolygonCollider2D worldBoundCollider2d;
        public Transform centerWatch;
        CancellationTokenSource ctsChangeOffset;
        CancellationTokenSource ctsChangeOrthoSize;
        public Transform sceneLoaderTracing;
        public BoxCollider2D centerCamTracing;
        public float DurationTimeSmoothChangeSate = 1.5f;
        public void ChangeCameraState(List<UnvsScene> s)
        {
            UnvsScene nearset = CalculateNearestScene(s);
            ctsChangeOffset = ctsChangeOffset.Refresh();
            ctsChangeOrthoSize = ctsChangeOrthoSize.Refresh();
            if (vcam.Lens.OrthographicSize > nearset.OrthographicSize)
            {
                vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token, DurationTimeSmoothChangeSate).Forget();
                vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, DurationTimeSmoothChangeSate, 3, ctsChangeOrthoSize.Token).ContinueWith(() =>
                {
                    camColl.size = cam.GetCameraWorldSize();
                }).Forget();

            }
            else
            {
                vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, DurationTimeSmoothChangeSate, 3, ctsChangeOrthoSize.Token)
                    .ContinueWith(() => { camColl.size = cam.GetCameraWorldSize(); }).Forget();
                vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token, DurationTimeSmoothChangeSate).Forget();
            }
            //ChangeCameraStateAsync(s).Forget();
        }
        public async UniTask ChangeCameraStateAsync(List<UnvsScene> s)
        {
            UnvsScene nearset = CalculateNearestScene(s);
            ctsChangeOffset = ctsChangeOffset.Refresh();
            ctsChangeOrthoSize = ctsChangeOrthoSize.Refresh();
            if(vcam.Lens.OrthographicSize> nearset.OrthographicSize)
            {
                await vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token);
                await vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, -1, 3, ctsChangeOrthoSize.Token);
               
            }
            else
            {
                await vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, -1, 3, ctsChangeOrthoSize.Token);
                await vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token);
            }
                
            camColl.size = cam.GetCameraWorldSize();
        }
        private UnvsScene CalculateNearestScene(List<UnvsScene> scenes)
        {
            if (scenes == null || scenes.Count == 0) return null;

            UnvsScene closestScene = null;
            float minDistance = float.MaxValue;
            float centerX = centerCamTracing.bounds.center.x;

            foreach (var s in scenes)
            {
                if(s==null||s.IsDestroying||s.IsDestroyed()) continue;
               
                float distLeft = math.abs(s.wallLeft.bounds.max.x - centerX);
                float distRight = math.abs(s.wallRight.bounds.min.x - centerX);
                float currentMin = math.min(distLeft, distRight);

               
                if (currentMin < minDistance)
                {
                    minDistance = currentMin;
                    closestScene = s;
                }
            }

            return closestScene;
        }

        public void UpdateMainCameraBoxCollider2dSize()
        {
            


            float orthoSize = vcam.Lens.OrthographicSize;
            float aspect = vcam.Lens.Aspect;


            float height = orthoSize * 2f;
            float width = height * aspect;


            camColl.size = new Vector2(width, height);


            camColl.isTrigger = true;


            camColl.offset = Vector2.zero;
        }
        public void UpdateWorldBound(UnvsScene ret)
        {
            
            this.worldBoundDict.Add(ret, ret.worldBound);
            Action<UnvsScene> OnSceneDestroyTmp = null;
            Action<UnvsScene> OnSceneDestroy = (s) =>
            {
                this.worldBoundDict.Remove(s);
                ret.OnDestroying -= OnSceneDestroyTmp;

            };
            OnSceneDestroyTmp= OnSceneDestroy;
            ret.OnDestroying += OnSceneDestroy;
            var bounds= this.worldBoundDict.Where(p=>p.Key!=null && !p.Key.IsDestroying && !p.Key.IsDestroyed()).Select(p=>p.Value).ToArray();
            this.worldBoundCollider2d.points=   bounds.CreateRectFromVectorList();
            this.confiner.InvalidateBoundingShapeCache();
        }
        public override void InitEvents()
        {
            //throw new System.NotImplementedException();
        }
        private void LateUpdate()
        {
            this.sceneLoaderTracing.transform.position=this.cam.transform.position;
            this.centerWatch.transform.position = this.camColl.bounds.center;
        }



#if UNITY_EDITOR
        [UnvsButton()]
        public void Generate()
        {
            var cinema = this;
            cinema.cam = cinema.AddChildComponentIfNotExist<Camera>("Main Camera");
            cinema.cam.tag = "MainCamera";
            cinema.cam.orthographic = true;
            cinema.cam.AddComponentIfNotExist<CinemachineBrain>();
            cinema.cam.AddComponentIfNotExist<AudioListener>();
            cinema.vcam = cinema.AddChildComponentIfNotExist<CinemachineCamera>("VCam");
            cinema.vcam.AddComponentIfNotExist<CinemachineFollow>();
            cinema.compositeCollider2D = cinema.AddChildComponentIfNotExist<CompositeCollider2D>("compositeCollider2D");

            cinema.compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
            cinema.confiner = cinema.vcam.AddComponentIfNotExist<CinemachineConfiner2D>();
            cinema.confiner.BoundingShape2D = cinema.compositeCollider2D;
          
            
            cinema.worldBoundCollider2d = this.AddChildComponentIfNotExist<PolygonCollider2D>("worldBoundCollider2d");
            cinema.worldBoundCollider2d.compositeOperation = Collider2D.CompositeOperation.Merge;
            confiner.BoundingShape2D = cinema.worldBoundCollider2d;
            cinema.worldBoundCollider2d.transform.SetParent(cinema.compositeCollider2D.transform);
            cinema.compositeCollider2D.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            cinema.compositeCollider2D.isTrigger = true;
            this.sceneLoaderTracing = this.AddChildComponentIfNotExist<Transform>("sceneLoaderTracing");
            var b = this.sceneLoaderTracing.AddComponentIfNotExist<Rigidbody2D>();
            b.bodyType = RigidbodyType2D.Kinematic;
            b.gravityScale = 0;
            b.angularDamping = 0;
            var c = b.AddComponentIfNotExist<BoxCollider2D>();
            c.SetMeOnTag(Constants.Tags.TRIGGER_LOAD_SCENE);
            c.isTrigger = true;
            camColl = c;
            c.size = cam.GetCameraWorldSize();
            this.centerWatch = this.AddChildComponentIfNotExist<Transform>("center-watch");
            var rb = this.centerWatch.AddComponentIfNotExist<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.angularDamping = 0;
            var cwc = this.centerWatch.AddComponentIfNotExist<BoxCollider2D>();
            cwc.isTrigger = true;
            cwc.size = new Vector2(0.1f, 0.1f);
            cwc.SetMeOnTag(Constants.Tags.TRIGGER_SCENE_CHANGE);
            cwc.SetMeOnTag(Constants.Layers.TRIGGER_SCENE_CHANGE);
            cwc.isTrigger = true;
            this.centerCamTracing= cwc;

        }

        





#endif
    }
}