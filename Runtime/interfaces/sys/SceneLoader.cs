using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace unvs.interfaces.sys
{
    public enum LoadTypeEnum
    {
        New,
        Left,
        Right,
        Top,
        Bottom,
        Interior
    }
    public interface ISceneLoader
    {

        event Action<IScenePrefab, LoadTypeEnum>  OnLoadBegin;
        event Action<IScenePrefab, LoadTypeEnum> OnLoadComplete;
        IChunkScenes Chunks {  get; }
        IActorObject CurrentActor { get; set; }
        int HorizontalChunkSize { get; }
        Transform ActorPlaceHolder { get; }
        IScenePrefab LastInteriorScene { get; set; }

        UniTask ClearAsync();
        UniTask<IScenePrefab> LoadInteriorAsync(string pathToWord, string targetName,IScenePrefab FromScene=null);
        
        UniTask<IScenePrefab> LoadNewAsync(string pathToWord, string targetName);
        UniTask<IScenePrefab> LoadRightSceneAsync(ITriggerZone zone);
        UniTask<IScenePrefab> LoadLeftSceneAsync(ITriggerZone zone);
        UniTask<IScenePrefab> LoadChunksAsync(ITriggerZone triggerZone);
        bool PlayActiveSceneFromEditor();
        void InitActor(IScenePrefab scene, string targetName);
        void SetupLayout(IScenePrefab scene);
        void SetUpEnvironment(IScenePrefab scene);
        void SetUpCam(IScenePrefab scene);
        UniTask ClearChunksAsync();
        void BackupGlobalLight();
        UniTask ClearAllAsync();
    }
    public interface IChunkScenes
    {
        UniTask CheckLeftAsync(Transform tempDelete, int horizontalChunkSize);
        UniTask CheckRightAsync(Transform tempDelete, int horizontalChunkSize);
        void Hide();
        void Show();
    }
}

