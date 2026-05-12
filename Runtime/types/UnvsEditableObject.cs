using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
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
        public Sprite[] sprites;
        [SerializeField]
        public Vector2 size;
        [SerializeField] public Vector2 Pivot;

        [SerializeField] public float frameRate; // Tốc độ animation (FPS)
        // Hàm lấy sprite tại thời điểm hiện tại
        public Sprite GetFrame(float time)
        {
            if (sprites == null || sprites.Length == 0) return null;
            if (sprites.Length == 1) return sprites[0];
            int index = Mathf.FloorToInt(time * frameRate) % sprites.Length;
            return sprites[index];
        }
        public async UniTask PlayAnimAsync(UnityEngine.UI.Image virtualCursor, System.Threading.CancellationToken token)
        {
            if (sprites.Length == 1)
            {
                virtualCursor.sprite = sprites[0];
                return;
            }
            if (sprites == null || sprites.Length <= 1) return;

            float startTime = Time.time;

            try
            {
                // Loop until the cancellation is requested
                while (!token.IsCancellationRequested)
                {
                    float elapsedTime = Time.time - startTime;

                    // Here you would typically apply the sprite to a target UI Image or Renderer.
                    // Since this is a struct, we assume the caller handles the display 
                    // or you could pass an Action<Sprite> to this method.
                   
                    virtualCursor.sprite = GetFrame(elapsedTime);
                    // Wait for the next frame to save performance (approx 1/FPS)
                    // Using PlayerLoop.Update ensures it stays in sync with Unity's frame rate
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully if needed
            }
        }
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