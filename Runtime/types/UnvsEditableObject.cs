using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using unvs.components;
using unvs.components;

namespace unvs.types
{
    [Serializable]
    public abstract class UnvsEditableProperty
    {

    }
    [Serializable]
    public class UnvsProperty<T>: UnvsEditableProperty where T : UnvsBaseComponent
    {
       
        public T Owner;
    }
    [Serializable]
    public struct IconInfo
    {
        [SerializeField]
        public Sprite srpite;
        [SerializeField]
        public Vector2 size;
        [SerializeField] public Vector2 Pivot;
    }
    [Serializable]
    public struct SceneLinkingData
    {
        public AssetReference LeftScene;
        public AssetReference RightScene;
    }

}