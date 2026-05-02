using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using unvs.components;
using unvs.components;
using unvs.ui;

namespace unvs.types
{
    public class UnvsDragContext
    {
        public UnvsDraggableItem Item;
        public Transform OldSlot;
        public Transform NewSlotOrDropContainer;
        internal bool hasChange;

        public void Reset()
        {
            Item = null;
            OldSlot = null;
            NewSlotOrDropContainer = null;
        }
        // Thêm các thông tin như ItemData, SlotIndex...
    }
    public enum DockType
    {
        None = 0,
        Top=1,
        Bottom=2,
        Left=3,
        Right=4,
        Full=5
    }
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