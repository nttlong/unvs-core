using System;
using System.Collections;
using UnityEngine;
using unvs.interfaces;

namespace Script.unvs.ext
{
    public static class IScenePrefabExtension
    {
        public static ISpawnTarget FindSpawnTargetNullReturnStartPos(this IScenePrefab scene,string targetName)
        {
            var ret=scene.FindSpawnTargetByName(targetName);
            if (ret == null) ret = scene.StartPos;
            return ret;

        }

        public static bool IsInteriorScene(this IScenePrefab scene)
        {
           return scene.GoWorld.GetComponentInParent<IInteriorSceneContainer>()!=null;
        }
    }
}