using System;
using Unity.VisualScripting;
using UnityEngine;
using unvs.components;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.objects.editor;
using unvs.types;
#if UNITY_EDITOR
namespace unvs.game2d.scenes
{


    public class EditorUnvsSceneSpawPointEditor : UnvsBaseComponent
    {

        [SerializeField]
        [UnvsProperty(UnvsPropertyTypeEnum.List)]
        public SpawnPointInfo[] spawnPoints;

        [UnvsButton("Apply all spawn points")]
        public void Apply()
        {
            for (var i = 0; i < spawnPoints.Length; i++)
            {
                {
                    var spawnPoint = spawnPoints[i];
                    if (spawnPoint.Target != null)
                    {
                        spawnPoint.name = spawnPoint.Target.name;
                        var s=spawnPoint.Target.transform.AddComponentIfNotExist<UnvsSpawnPoint>();
                        s.name = spawnPoint.Target.name;
                    }
                }
            }

        }

    }
}
#endif