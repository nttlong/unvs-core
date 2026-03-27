using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.baseobjects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PickableItem : MonoBehaviour, IPickableObject, IInteractableObject, IStoragableObject
    {
        public ExploringType exploring;
        public InteractionDefinition data;
        public Collider2D coll;
        public Sprite icon;
        public LocalizedString description;
        public LocalizedString gameName;
        public Texture2D texT;
       
        public bool hideSpriteRendererWhenPlaying=true;
        public SpriteRenderer spriteR;

        public ExploringType Exploring => exploring;

        public InteractionDefinition Data => data;

        public Collider2D Collider => coll;

        public Sprite Icon => icon;

        public string Name => name;

        public LocalizedString Description => description;

        public LocalizedString GameName => gameName;

        

        public Texture2D TexT => texT;

        public SpriteRenderer SpriteR => spriteR;

        public bool HideSpriteRendererWhenPlaying => hideSpriteRendererWhenPlaying;

        public Vector2 Size => GetComponent<Collider2D>().bounds.size;

        public async UniTask<bool> ExecAsync(MonoBehaviour target, CancellationTokenSource token)
        {
          return await data.ExecAsync(this, target, token);
        }

        public Vector2 GetPosition()
        {
            return coll.bounds.center;
        }

        public Vector2 GetPosition(float Direction, float ReachDistance)
        {
            if (Direction < 0)
            {
                return new Vector2(coll.bounds.center.x+ ReachDistance, coll.bounds.center.y);
            } else
            {
                return new Vector2(coll.bounds.center.x - ReachDistance, coll.bounds.center.y);
            }
        }
        private void Awake()
        {

            if (Application.isPlaying)
            {
                this.SetMeOnLayer(Constants.Layers.INTERACT_OBJECT);
                spriteR.enabled = hideSpriteRendererWhenPlaying;
            }
            if (spriteR == null) spriteR = GetComponent<SpriteRenderer>();
            if (texT == null)
            {
                texT = Commons.LoadAsset<Texture2D>("Packages/com.unvs.core/Runtime/Sprites/Circle.png");


            }
            spriteR.ApplyTexture(texT);
            coll = GetComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spriteR == null) spriteR = GetComponent<SpriteRenderer>();
            if (texT == null)
            {
                texT = Commons.LoadAsset<Texture2D>("Packages/com.unvs.core/Runtime/Sprites/Circle.png");
                

            }
            if (spriteR.sprite==null && texT!=null)
            {
                spriteR.ApplyTexture(texT);
                UnityEditor.EditorUtility.SetDirty(this);
            }
            
        }
#endif
    }
}