using System;
using UnityEngine;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;

namespace unvs.game2d.scenes
{
#if UNITY_EDITOR
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



    }
#endif
}