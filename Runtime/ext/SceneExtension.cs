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

       
    }
}