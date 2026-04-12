/*
    this file define Virtuale scene, acctually, that is prefab of scene
    it contain Main cam
   
   
*/

using Cysharp.Threading.Tasks;
using System;
using System.Linq;

using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.game2d.scenes.actors;
using unvs.interfaces;
using unvs.shares;
#if UNITY_EDITOR
namespace unvs.game2d.scenes
{
    
    public partial class UnvsScene : UnvsComponent
    {
        [Header("Links scene")]
        [SerializeField]
        public _UnvsSceneEditorObject Links;
        [UnvsButton("Review")]
        public void Review()
        {
            ApplyRequireComponents();
            this.actor = this.GetComponentInChildren<UnvsActor>();
            if (this.actor != null)
            {
                this.actor.StandBy(this.GetStartPosition(""));
                this.vcam.Watch(this.actor.camWatcher);

            }
            else
            {

                this.vcam.Watch(defaulCamWatcher);

            }

            //this.vcam.GetComponent<CinemachineConfiner2D>().BoundingShape2D = this.worldBound;
        }
        [UnvsButton("Apply require components")]
        public void ApplyRequireComponents()
        {
            if (this.vcam != null)
            {
                this.vcam.Lens.OrthographicSize = this.OrthographicSize;
                this.vcam.GetComponent<CinemachineFollow>().FollowOffset = this.followOffset;
            }
            if (this.triggerRight != null)
            {
                this.triggerLoadSceneRight = this.triggerRight.AddComponentIfNotExist<LoadSceneTracking>();
                this.triggerLoadSceneRight.direction = LoadeSceneEnum.Right;
            }
            if (this.triggerRight != null)
            {
                this.triggerLoadSceneLeft = this.triggerLeft.AddComponentIfNotExist<LoadSceneTracking>();
                this.triggerLoadSceneLeft.direction = LoadeSceneEnum.Left;
            }
            if (this.sceneTracker == null)
            {
                this.sceneTracker = this.AddChildComponentIfNotExist<Transform>("scene-tracker");
                var poly = this.sceneTracker.AddComponentIfNotExist<PolygonCollider2D>();
                poly.isTrigger = true;
                poly.SetMeOnLayer(Constants.Layers.TRIGGER_SCENE_CHANGE);
            }
            if (this.triggerLeft != null) this.triggerLeft.isTrigger = true;
            if (this.triggerRight != null) this.triggerRight.isTrigger = true;

            this.sceneTracker.AddComponentIfNotExist<UnvsSceneTracker>();
            if (this.SpawnPoints == null)
            {
                this.SpawnPoints = this.AddChildComponentIfNotExist<EditorUnvsSceneSpawPointEditor>("Spawn-Points");
            }
            syncWorldBoundAndScencTracker();
        }

        private void syncWorldBoundAndScencTracker()
        {
            var rate = (this.worldBound.bounds.size.x + 5f) / this.worldBound.bounds.size.x;
            var paths = this.worldBound.ClonePaths(rate, 1);
            this.sceneTracker.GetComponent<PolygonCollider2D>().points = new Vector2[] { };
            for (int i = 0; i < paths.Length; i++)
            {
                this.sceneTracker.GetComponent<PolygonCollider2D>().SetPath(i, paths[i]);
            }
        }

        [UnvsButton("Generate elements")]
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
            this.JoinInfo.Size = this.cam.GetCameraWorldSize();
            //this.edgesWorldBound = this.AddChildComponentIfNotExist<EdgeCollider2D>("edgesWorldBound");
            this.startPoint = this.AddChildComponentIfNotExist<Transform>("start-point");
            this.startPoint.transform.position = new Vector3(this.JoinInfo.Size.x / 2, 0, -10);
            this.defaulCamWatcher.transform.position = this.JoinInfo.Size / 2;
            this.worldBound = this.AddChildComponentIfNotExist<PolygonCollider2D>("world-bound");
            this.worldBound.SetMeOnLayer(Constants.Layers.WORLD_BOUND);
            this.worldBound.isTrigger = true;
            this.worldBound.points = this.defaulCamWatcher.GetSegment().Center().CreateRectFromCenter(this.JoinInfo.Size);
            this.triggerLeft = this.AddChildComponentIfNotExist<BoxCollider2D>("trigger-left");
            this.triggerLeft.SetMeOnLayer(Constants.Layers.TRIGGER_LOAD_SCENE);
            this.triggerLeft.SetMeOnTag(Constants.Tags.TRIGGER_LOAD_SCENE_LEFT);

            this.triggerRight = this.AddChildComponentIfNotExist<BoxCollider2D>("trigger-right");
            this.triggerRight.SetMeOnLayer(Constants.Layers.TRIGGER_LOAD_SCENE);
            this.triggerRight.SetMeOnTag(Constants.Tags.TRIGGER_LOAD_SCENE_LEFT);
            this.wallLeft = this.AddChildComponentIfNotExist<BoxCollider2D>("wall-left");
            this.wallRight = this.AddChildComponentIfNotExist<BoxCollider2D>("wall-right");
            this.ground = this.AddChildComponentIfNotExist<EdgeCollider2D>("ground");
            var dx = this.defaulCamWatcher.transform.GetSegment().Center().x;
            this.ground.points = new Vector2[] { new Vector2(dx - this.JoinInfo.Size.x / 2 - 5, 0), new Vector2(dx + this.JoinInfo.Size.x / 2 + 5, 0) };

            this.worldBound.AlignWall(this.wallLeft, this.wallRight);
            this.worldBound.AlignWall(this.triggerLeft, this.triggerRight, true);
            this.light2d = this.AddChildComponentIfNotExist<Light2D>("light2d");
            this.light2d.lightType = Light2D.LightType.Global;


            this.ApplyRequireComponents();
            calculateJoinPoint();
        }

        private bool calculateJoinPoint()
        {

            this.JoinInfo = ground.CalculateBound(this.worldBound);

            return this.JoinInfo != null;
        }

        private void OnValidate()
        {
            if(this.vcam!=null)
            this.vcam.SetOrthoSizeImmediate(this.OrthographicSize);
            if (this.cinemachineFollow != null)
            {
                 this.cinemachineFollow.FollowOffset= this.followOffset;
            }
            if (this.Links.LeftScene != null)
                this.SceneLeft = unvs.shares.editor.UnvsEditorUtils.EditorGetAddressPath(this.Links.LeftScene);
            if (this.Links.RightScene != null)
                this.SceneRight = unvs.shares.editor.UnvsEditorUtils.EditorGetAddressPath(this.Links.RightScene);
        }
        private void OnDrawGizmos()
        {
            if (this.worldBound != null) this.worldBound.GizmosDraw(Color.green, 1);
            if (this.defaulCamWatcher != null) this.defaulCamWatcher.transform.GetSegment().Center().DrawCircle(1f, Color.red);
            if (this.startPoint != null) this.startPoint.GetSegment().Center().DrawCircle(1f, Color.green);
            if (!Application.isPlaying)
            {
                calculateJoinPoint();
            }
            if (this.JoinInfo != null)
            {
                this.JoinInfo.LeftPos.DrawCircle(1, Color.red);
                this.JoinInfo.RightPos.DrawCircle(1, Color.red);
            }
            if (this.worldBound != null && this.wallLeft != null && this.wallRight != null)
            {
                this.worldBound.AlignWall(this.wallLeft, this.wallRight);
                this.wallLeft.GizmosDraw(Color.red, 1);
                this.wallRight.GizmosDraw(Color.red, 1);
            }
            if (this.worldBound != null && this.triggerLeft != null && this.triggerRight != null)
            {
                this.worldBound.AlignWall(this.triggerLeft, this.triggerRight, true);
                this.triggerLeft.GizmosDraw(Color.rosyBrown, 2);
                this.triggerRight.GizmosDraw(Color.rosyBrown, 2);
            }
            if (this.ground != null)
            {
                this.ground.GizmosDraw(Color.red, 3f);
            }
            if (!Application.isPlaying)
            {
                this.vcam.SetOrthoSizeImmediate(this.OrthographicSize);
            }
                

            if (this.JoinInfo != null)
            {
                this.JoinInfo.LeftPos.DrawCircle(1, Color.blue, 3f);
                this.JoinInfo.RightPos.DrawCircle(1, Color.darkGreen, 3f);
            }
            if (this.sceneTracker != null && this.worldBound != null)
            {
                syncWorldBoundAndScencTracker();
                if (this.sceneTracker.GetComponent<PolygonCollider2D>() != null)
                    this.sceneTracker.GetComponent<PolygonCollider2D>().GizmosDraw(Color.azure, 2f);
            }
            
        }


        [Serializable]
        public class _UnvsSceneEditorObject
        {
            public AssetReference LeftScene;
            public AssetReference RightScene;
        }

    }
}
#endif