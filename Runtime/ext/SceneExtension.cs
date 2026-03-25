using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using unvs.interfaces;

using unvs.shares;
namespace unvs.ext
{
    public static class SceneExtension
    {


        

        public static void LeaveObjectAtScene(this Scene scene, GameObject gameObject)
        {
            if (scene == null) return;
            gameObject.transform.SetParent(null, true);
            SceneManager.MoveGameObjectToScene(gameObject, scene);
        }
        public static void LeaveObjectAtScene(this Scene? scene, GameObject gameObject)
        {
            if (scene == null || scene.Value == null) return;
            SceneManager.MoveGameObjectToScene(gameObject, scene.Value);
        }
        public static string CurrentWord;

        public static async UniTask<IScenePrefab> LoadNewWorldAsync(this Scene scene, string pathToWord)
        {
            var singleScene = SceneManager.GetSceneByName(Constants.Scenes.MAIN_SCENE);
            if (singleScene == null || !singleScene.IsValid())
            {
                CurrentWord = pathToWord;
                return null;
            }
            var goWorld = await Commons.LoadPrefabsAsync(pathToWord);


            if (goWorld == null)
            {
                SLog.Error($"${pathToWord} was not found");
                return null;
            }
            var ret = goWorld.GetComponent<IScenePrefab>();
            if (ret == null)
            {
                SLog.Error($"${pathToWord} was not inherit ffrom {typeof(IScenePrefab).FullName}");
                return null;
            }


            // 3. Đảm bảo scene hợp lệ trước khi move
            if (singleScene.isLoaded)
            {
                SceneManager.MoveGameObjectToScene(goWorld, singleScene);
            }
            return ret;
        }
        public static IScenePrefab LoadNewWorld(this Scene scene, string pathToWord, Transform parent = null)
        {

            var goWorld = Commons.LoadPrefabs(pathToWord);
            if (goWorld == null) return null;
            var ret = goWorld.GetComponent<IScenePrefab>();
            if (ret == null)
            {
                SLog.Error($"{pathToWord} do not inherit form {typeof(IScenePrefab).FullName}");
                return null;
            }
            var singleScene = SceneManager.GetSceneByName(Constants.Scenes.MAIN_SCENE);
            if (singleScene == null || !singleScene.IsValid())
            {
                CurrentWord = pathToWord;
            }




            // 3. Đảm bảo scene hợp lệ trước khi move
            if (singleScene.isLoaded)
            {
                if (goWorld.transform.parent == null)
                    SceneManager.MoveGameObjectToScene(goWorld, singleScene);
            }

            return ret;
        }
    }
}