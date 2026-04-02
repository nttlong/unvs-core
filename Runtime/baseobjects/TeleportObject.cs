using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
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
    [RequireComponent (typeof(BoxCollider2D))]
    [RequireComponent(typeof(ShadowCaster2DObject))]
    public class TeleportObject : MonoBehaviour, ITeleportPrefab,IInteractableObject,ISpawnTarget, IConsumerObject
    {
        [Header("Audio")]
        [SerializeField]
        public AudioClip openSound;
        [SerializeField]
        public AudioClip closeSound;
        [Header("Target")]
        public bool isNew;
        public string pathToWord;
        public string spawnTarget;
        [Header("Orher")]
        public bool hideSpriteWhenPlaying;
        public ExploringType exploring;
        public InteractionDefinition data;
        public BoxCollider2D coll;
        
        public SpriteRenderer spriteRenderer;
        public Texture2D texT;
        public Sprite sprite;
        public bool hideSpriteRendererWhenRun=true;
        public InteractionDefinition consumeDefinintion;

        public AudioClip OpenSound => openSound;

        public AudioClip CloseSound => closeSound;

        public string PathToWord => pathToWord;

        public bool IsNew => isNew;

        

        public string TargetName => spawnTarget;

        public bool HideSpriteWhenPlaying => hideSpriteWhenPlaying;

        public ExploringType Exploring => exploring;

        public InteractionDefinition Data => data;

        public Collider2D Collider => coll;

        public Texture2D TextT => texT;

        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public Sprite Sprite => sprite;

        public string Name => name;

        public Collider2D Coll => coll;

        public Vector2 Pos => GetPosition();

        public SpriteRenderer Renderer => spriteRenderer;

        public bool HideSpriteRendererWhenRun => hideSpriteRendererWhenRun;

        public InteractionDefinition ConsumeDefinintion => consumeDefinintion;

        public async UniTask<bool> ExecAsync(MonoBehaviour target, CancellationTokenSource token)
        {
            return await data.ExecAsync(this.GetComponent<IInteractableObject>(), target, token);
        }

        public Vector2 GetPosition()
        {
            var edge = coll.FindBottomEdge(unvs.shares.Constants.Layers.SURFACE);
            if (edge == null) edge = GetComponentInParent<IScenePrefab>(true).Floor;

            return edge.GetIntersetPoint(coll.bounds.center.x);

        }

        public void MoveOtherToMe(MonoBehaviour monoBehaviour)
        {
            var pos=GetPosition();
            monoBehaviour.transform.position = pos;
        }

        private void Awake()
        {
            if(Application.isPlaying)
            {
                if(spriteRenderer!=null) 
                spriteRenderer.enabled = hideSpriteRendererWhenRun;
            }
            if(spriteRenderer==null) spriteRenderer=GetComponent<SpriteRenderer>();
            if (texT == null)
            {
                texT = Commons.LoadAsset<Texture2D>("Packages/com.unvs.core/Runtime/Sprites/Square.png");
                
                 
            }
            spriteRenderer.ApplyTexture(texT);
            if (coll == null)
            {
                GetComponent<BoxCollider2D>().SetSize(2.5f, 2.5f);
                spriteRenderer.SetSizeWorld(2.5f, 2.5f);
            }
            coll = GetComponent<BoxCollider2D>();
           // coll?.Resize(transform);
            if (coll!=null) 
            coll.isTrigger = true;
            
            if(Application.isPlaying)
            this.SetMeOnLayer(unvs.shares.Constants.Layers.INTERACT_OBJECT);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            
        }

        private void OnValidate()
        {

           this.sprite= GetComponent<SpriteRenderer>()?.ApplyTextureIfEmptySprite(texT);

            UnityEditor.EditorUtility.SetDirty(this);
            



        }
#endif
    }

}