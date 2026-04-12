using System;
using UnityEngine;

namespace unvs.game2d.scenes
{
    [Serializable]
    public struct SpawnPointInfo
    {
        public string name;
        public GameObject Target;
        public bool IsSelected;
    }
    public class EditorUnvsSceneSpawPointEditor : UnvsBaseComponent
    {
       
        [SerializeField]
        [UnvsProperty(UnvsPropertyTypeEnum.List)]
        public SpawnPointInfo[] spawnPoints;
//#if UNITY_EDITOR

//#endif
    }
}