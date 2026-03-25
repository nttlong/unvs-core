//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Threading;
//using UnityEngine;
//using UnityEngine.Rendering.Universal;
//namespace unvs.interfaces
//{
//    public interface IWorldMonitor
//    {
//        CancellationTokenSource Cts { get; set; }
//        IScenePrefab CurrentScene { get; set; }


//        UniTask OnChangeSceneAsync(IScenePrefab scenePrefab, CancellationToken ctk = default);
//        UniTask ResolveFloorMarterialAsync(IScenePrefab oldScene, IScenePrefab newScene, CancellationToken ctk = default);
//        UniTask ResolveLightAsync(IScenePrefab oldScene, IScenePrefab newScene, CancellationToken ctk = default);
//        UniTask ResolveAudioAsync(IScenePrefab oldScene, IScenePrefab newScene, CancellationToken ctk = default);
//        void RegisterGlobalLight(IGlobalLightWapper globalLight);
//        void UnRegisterGlobalLight(IGlobalLightWapper globalLight);
//        void CleanUpGlobalLight(params IScenePrefab[] ExcepThese);
//    }
//}