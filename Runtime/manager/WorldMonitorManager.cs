//using Cysharp.Threading.Tasks;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Rendering.Universal;
//using unvs.ext;
//
//using unvs.manager;
//using unvs.shares;
//namespace unvs.gameword.manager
//{
//    public class WorldMonitorManager : MonoBehaviour, IWorldMonitor
//    {
//        const string GlobalLightStorageName = "Global-Light";
//        public static IWorldMonitor Instance;
//        public IScenePrefab currentScene;
//        public Transform currentSceneTr;

//        public float currentOrthographicSize;
//        private CancellationTokenSource cts;
//        private Transform _globalLingStorage;


//        public IScenePrefab CurrentScene { get => currentScene; set => currentScene = value; }

//        public CancellationTokenSource Cts { get => cts; set => cts = value; }
//        //public Transform GlobalLingStorage
//        //{
//        //    get
//        //    {
//        //        if (_globalLingStorage != null) return _globalLingStorage;
//        //        _globalLingStorage = this.GetComponentInChildrenByName<Transform>("GlobalLingStorage");
//        //        if (_globalLingStorage == null)
//        //        {
//        //            _globalLingStorage = (new GameObject("GlobalLingStorage")).transform;
//        //            _globalLingStorage.SetParent(transform);
//        //            return _globalLingStorage;

//        //        }
//        //        return null;
//        //    }
//        //}



//        public static async UniTask RaiseChangeSceneAsync(IScenePrefab scenePrefab)
//        {
//            if (Instance == null) return;
//            Instance.Cts = Instance.Cts.Refresh();
//            await Instance.OnChangeSceneAsync(scenePrefab, Instance.Cts.Token);
//        }
//        public static void RaiseHitScene(IScenePrefab scenePrefab, int dir)
//        {
//            var instance = SingleScene.Instance;
//            var actor = instance.CurrentActor;

//        }




//        private void Awake()
//        {
//            Instance = this;

//            GlobalApplication.WorldMonitorManager = this as IWorldMonitor;



//        }

//        public async UniTask OnChangeSceneAsync(IScenePrefab scenePrefab, CancellationToken ctk = default)
//        {
//            await Instance.ResolveFloorMarterialAsync(Instance.CurrentScene, scenePrefab, ctk);
//            await Instance.ResolveLightAsync(Instance.CurrentScene, scenePrefab, ctk);
//            currentScene = scenePrefab;
//        }

//        public async UniTask ResolveFloorMarterialAsync(IScenePrefab oldScene, IScenePrefab newScene, CancellationToken ctk = default)
//        {
//            await UniTask.Yield();
//            if (oldScene != null && !oldScene.IsDestroying && newScene != null && !newScene.IsDestroying)
//            {
//                if (newScene.FloorMaterial == null)
//                {
//                    var oldMaterial = oldScene.FloorMaterial;
//                    if (oldMaterial != null)
//                    {
//                        newScene.FloorMaterial = oldMaterial;
//                    }
//                }


//            }
//        }

//        public async UniTask ResolveLightAsync(IScenePrefab oldScene, IScenePrefab newScene, CancellationToken ctk = default)
//        {
//            LightManagerObject.Add(oldScene, newScene);
//            //SingleScene.Instance.CurrentActor.Speaker.SayText($"move {oldScene?.Name}->{newScene?.Name}");
//            //if (Instance.CurrentGlobalLight == null)
//            //{
//            //    Instance.CurrentGlobalLight = newScene.Globalight.GlobalLight.Light;

//            //    Instance.CurrentGlobalLight.gameObject.SetActive(true);
//            //} else
//            //{
//            //    newScene.Globalight.GlobalLight.Light.transform.position = (newScene as MonoBehaviour).GetComponentInChildren<IWorldTracker>().Coll.bounds.center;
//            //    await newScene.Globalight.GlobalLight.Light.TransitionLightDirectAsync(Instance.CurrentGlobalLight);
//            //    Instance.CurrentGlobalLight = newScene.Globalight.GlobalLight.Light;
//            //}
//        }

//        public UniTask ResolveAudioAsync(IScenePrefab oldScene, IScenePrefab newScene, CancellationToken ctk = default)
//        {
//            throw new NotImplementedException();
//        }

//        public void RegisterGlobalLight(IGlobalLightWapper globalLight)
//        {
//            //if (this.CurrentGlobalLight == null)
//            //{
//            //    this.CurrentGlobalLight = globalLight.Light;
//            //    this.CurrentGlobalLight.lightType = Light2D.LightType.Global;
//            //    this.CurrentGlobalLight.gameObject.SetActive(true);
//            //    this.CurrentGlobalLight.transform.SetParent(GlobalLingStorage.transform, true);
//            //    return;

//            //}
//            //;
//            //globalLight.Light.transform.SetParent(GlobalLingStorage.transform, true);
//            //var lights = this.GlobalLingStorage.GetComponentsInChildren<Light2D>(true).ToArray();
//            //var data=Light2DExtension.MixLightSources(GlobalApplication.CamTracking.Collider.bounds.center, lights);
//            //this.CurrentGlobalLight.color = data.Color;
//            //this.CurrentGlobalLight.intensity = data.Intensity;
//        }

//        //public void UnRegisterGlobalLight(IGlobalLightWapper globalLight)
//        //{
//        //    //if (Instance.CurrentGlobalLight == globalLight.Light)
//        //    //{
//        //    //    Destroy(Instance.CurrentGlobalLight.gameObject);
//        //    //    return;
//        //    //}
//        //    foreach (Transform light in GlobalLingStorage.GetComponentInChildren<Transform>())
//        //    {
//        //        if (light != null)
//        //        {
//        //            Destroy(light.gameObject);
//        //        }
//        //    }
//        //}



//        //public void CleanUpGlobalLight(params IScenePrefab[] ExcepThese)
//        //{
//        //    foreach (Transform light in GlobalLingStorage.GetComponentInChildren<Transform>())
//        //    {
//        //        if (light != null)
//        //        {
//        //            if (ExcepThese.Any(p => p == light.GetComponent<IGlobalLightWapper>().Owner)) continue;
//        //            Destroy(light.gameObject);
//        //        }
//        //    }
//        //}
//    }
//}