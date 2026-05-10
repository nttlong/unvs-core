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

        public void Cancel()
        {
            Item = null;
            OldSlot = null;
            NewSlotOrDropContainer = null;
        }

        public void Ok()
        {
            Item = null;
            OldSlot = null;
            NewSlotOrDropContainer = null;
            hasChange = true;
        }
        // Thêm các thông tin như ItemData, SlotIndex...
    }
    [Serializable]
    public struct SpawnPointInfo
    {
        public string name;
        public GameObject Target;
        public bool IsSelected;
    }
    [Serializable]
    public class SpriteMaterial
    {
        [SerializeField]
        public SpriteRenderer spriteRenderer;
        [SerializeField]
        public Material material;
    }
    public enum TeleportType
    {
        Interior=0,
        NewScene=1,
        TempScene=2,
        [HideInInspector]
        ReturnToScene = 3
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
    


    [Serializable]
    public struct AudioInfo
    {
        [SerializeField]
        public AudioClip Clip;
        [Range(0, 1)]
        [SerializeField]
        public float volume;

        public bool IsEmpty()
        {
            return Clip == null;
        }
        public static AudioInfo EmptyNew()
        {
            return new AudioInfo()
            {
                volume = 1f
            };
        }

        public AudioInfo GetBetter(AudioInfo[] feedBack)
        {
            foreach (var item in feedBack)
            {
                if(!item.IsEmpty()) return item;
            }
            return this;
        }

        public void Play(AudioSource audioSource)
        {
            if(audioSource == null) return;
            if(this.IsEmpty()) return;
            audioSource.PlayOneShot(this.Clip);
        }
    }
}