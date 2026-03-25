using unvs.interfaces;
using System;

namespace unvs.manager
{
    public enum SceneState
    {
        LoadNew,
        LoadLeft,
        LoadRight,
        UnloadLeft,
        UnloadRight,
        UnloadAll,
        LoadInterior,
        UnloadInterior,
    }
    public class GameEvents
    {
        public static event Action<IScenePrefab,SceneState, ISpawnTarget> OnSceneChange;
        public static event Action<IScenePrefab,SceneState, ISpawnTarget> OnSceneDrop;
        public static event Action<IScenePrefab,SceneState, ISpawnTarget> OnSceneLoad;
        public static event Action<IScenePrefab,SceneState,ISpawnTarget> OnSceneLoadComplete;

        public static void TriggerOnSceneChange(IScenePrefab scene, SceneState state, ISpawnTarget startSpawn) => OnSceneChange?.Invoke(scene, state, startSpawn);
        public static void TriggerOnSceneDrop(IScenePrefab scene, SceneState state, ISpawnTarget startSpawn) => OnSceneDrop?.Invoke(scene, state, startSpawn);
        public static void TriggerOnSceneLoad(IScenePrefab scene, SceneState state, ISpawnTarget startSpawn) => OnSceneLoad?.Invoke(scene, state, startSpawn);
        public static void TriggerOnSceneLoadComplete(IScenePrefab scene, SceneState state, ISpawnTarget startSpawn) => OnSceneLoadComplete?.Invoke(scene, state, startSpawn);

        public static IActorObject CurrentActor { get; set; }
        public static IScenePrefab CurrentScene { get; set; }
    }
}