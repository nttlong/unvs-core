using unvs.ext;
using unvs.game2d.scenes;
using unvs.shares;
using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Threading;
using unvs.actions;
namespace unvs.game2d.objects
{

    [ExecuteAlways]
    [RequireComponent(typeof(BoxCollider2D))]
    public partial class UnvsInteractObject : UnvsComponent
    {
        [Header("Interact info")]
        public BoxCollider2D coll;
        public Texture2D mousePoint;
        public InteractionDefinition Data;
        public override void InitRuntime()
        {
            if(this.mousePoint==null)
            this.mousePoint = UnvsInteractUI.Instance.defaultCursorIcon;
        }

        public virtual Vector2 GetPosition()
        {
            return coll.bounds.center;
        }

        public async UniTask ExecuteAsync(MonoBehaviour target, CancellationTokenSource cts)
        {
            if (Data == null) return;
           
            var sender = new ActionBaseSender()
            {
                Target = target,
                Source=this,
                Cts= cts
            };
            //sender.Cts = sender.Cts.Refresh();
            foreach (var item in Data.actions)
            {
                if(item==null) continue;
                await item.ExecuteAsync(sender);
                if (sender.IsCancel ) return;
            }
            
        }
    }
#if UNITY_EDITOR
    public partial class UnvsInteractObject : UnvsComponent
    {

        private void OnValidate()
        {
            this.SetMeOnLayer(Constants.Layers.INTERACT_OBJECT);
        }
        private void OnDrawGizmos()
        {
            if(coll==null)
            coll=this.GetComponent<BoxCollider2D>();
            var sp=this.GetComponent<SpriteRenderer>();
            if (sp != null)
            {
                SpriteRendererExtension.FixCollider2DSize(sp, coll);
                float width = sp.sprite.rect.width / sp.sprite.pixelsPerUnit;
                float height = sp.sprite.rect.height / sp.sprite.pixelsPerUnit;
                coll.size = new Vector2(width, height);// sp.transform.localScale;
                coll.offset = Vector2.zero;
                // coll.offset =new ( sp.transform.position.x/2,sp.transform.position.y);//= coll.offset;
                coll.transform.rotation = transform.rotation;// = coll.transform.rotation;
            } else
            {
                sp = this.GetComponentInChildren<SpriteRenderer>();
                if (sp != null && coll != null)
                {
                    sp.transform.localScale = coll.size;
                    sp.transform.localPosition = coll.offset;
                    // coll.offset =new ( sp.transform.position.x/2,sp.transform.position.y);//= coll.offset;
                    coll.transform.rotation = sp.transform.rotation;// = coll.transform.rotation;
                }
            }
            
        }
    }
#endif
}