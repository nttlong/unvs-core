using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using Unity.Mathematics.Geometry;
using UnityEngine;
using unvs.ext;
//using unvs.gameword.manager;
using unvs.interfaces;
using unvs.shares;

namespace unvs.gameword
{
    public class WorldTrackerObject : MonoBehaviour, IWorldTracker
    {
        //private ICamTracking xRight;
        //private ICamTracking xLeft;
        //private ICamTracking xTop;
        //private ICamTracking xBottom;
        private CancellationTokenSource cts;
        private bool stop;
        private IScenePrefab currentSceneWatch;
        private IScenePrefab[] lastScenesTracking = new IScenePrefab[0];
        private Vector3 lastPos;

        private void Awake()
        {
            GlobalApplication.WorldTrackerObject = this as IWorldTracker;
        }
        public PolygonCollider2D Coll
        {
            get
            {
                return GetComponent<PolygonCollider2D>();
            }
        }

        public CancellationTokenSource Cts { get => cts; set => cts = value; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var camWacher = other.GetComponent<ICamWacher>();
            if (camWacher == null) return;
            if (!(camWacher as MonoBehaviour).GetComponentInParent<IActorObject>().IsActive) return;
            var Instance = SingleScene.Instance;


            var tracker = GlobalApplication.CamTracking;





           // var actor = Instance.CurrentActor;
            var myScene = GetComponentInParent<IScenePrefab>();
            tracker.AddScene(myScene);

            var nearestScene = camWacher.Coll.bounds.center.GetNearsetScene(tracker.ScensHit);
            if (camWacher.Coll.bounds.center != lastPos && !this.stop && nearestScene!=null)
            {
                this.stop = true;
                

                var ascynValue = SingleScene.Instance.VCam.SetOrthoSizeSmoothlyAsync(nearestScene.OrthographicSize, 2f);

                ascynValue.ContinueWith(() =>
                {
                    if (nearestScene == null || nearestScene.IsDestroying) return;
                    this.stop = false;
                    tracker.Clear();
                    currentSceneWatch = nearestScene;

                    


                }).Forget();

                lastPos = camWacher.Coll.bounds.center;

            }

        }

       

        private void OnTriggerExit2D(Collider2D other)
        {
            var camWacher = other.GetComponent<ICamWacher>();
            if (camWacher == null) return;
            //this.stop = false;
            var Instance = SingleScene.Instance;
            var tracker = GlobalApplication.CamTracking;

            //var watcher = other.GetComponent<ICamTracking>();
            if (tracker == null) return;

            //var actor = Instance.CurrentActor;
            var myScene = GetComponentInParent<IScenePrefab>();
            tracker.RemoveScene(myScene);
            var nearestScene = camWacher.Coll.bounds.center.GetNearsetScene(tracker.ScensHit);
            //actor.Speaker.SayText($"currentSceneWatch={currentSceneWatch.Name}\n,nearestScene={nearestScene.Name}");
            if (nearestScene == null) return;
            if (camWacher.Coll.bounds.center != lastPos && !stop)
            {
                this.stop = true;
                //lastScenesTracking = Clone(tracker.ScensHit);

                SingleScene.Instance.VCam.SetOrthoSizeSmoothlyAsync(nearestScene.OrthographicSize, 2f).ContinueWith(async () =>
                {
                    this.stop = false;
                    tracker.Clear();
                    currentSceneWatch = nearestScene;
                    //await WorldMonitorManager.RaiseChangeSceneAsync(nearestScene);
                }).Forget();
                lastPos = camWacher.Coll.bounds.center;

            }

        }

        public void Off()
        {
            this.enabled = false;
            if (Coll != null) Coll.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void On()
        {
            gameObject.SetActive(true);
            if (Coll != null) Coll.gameObject.SetActive(true);
           
            this.enabled = true;
           
        }
    }
}
