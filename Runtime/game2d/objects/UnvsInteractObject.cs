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
using UnityEngine.Rendering;
using unvs.unvsobjects;
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
        public UnvsObjectAttributesData ObjectDefinition;
        /// <summary>
        /// This event is called only once when the object is interacted for the first time.
        /// </summary>
        public Func<ActionBaseSender,UniTask> OnFirstTimeInteract;
        /// <summary>
        /// This event is called when the object is interacted for the last time.
        /// </summary>
        public Func<ActionBaseSender,UniTask> OnCompletedAsync;
        /// <summary>
        /// This event is called every time the object is interacted.
        /// </summary>
        public Func<ActionBaseSender,UniTask> OnStartInteract;
        public string LayerName = Constants.Layers.INTERACT_OBJECT;
        public override void InitRuntime()
        {
            
            if(this.mousePoint==null)
            this.mousePoint = UnvsInteractUI.Instance.defaultCursorIcon;
        }

        public virtual Vector2 GetPosition()
        {
            return coll.bounds.center;
        }

        public async UniTask<ActionBaseSender> ExecuteAsync(MonoBehaviour target, CancellationTokenSource cts)
        {
            
          
            var sender = new ActionBaseSender()
            {
                Target = target,
                Source=this,
                Cts= cts
            };
            if (OnFirstTimeInteract != null)
            {
                await OnFirstTimeInteract(sender);
                OnFirstTimeInteract = null;
                if (sender.IsCancel) return sender;
            }

            if (OnStartInteract != null)
            {
                await OnStartInteract(sender);
                if (sender.IsCancel) return sender;
            }
            if (Data == null)
            {
                sender.Cancel();
                return sender;

            }
            //sender.Cts = sender.Cts.Refresh();
            foreach (var item in Data.actions)
            {
                if(item==null) continue;
                await item.ExecuteAsync(sender);
                if (sender.IsCancel ) return sender;
            }
            if (OnCompletedAsync != null)
            {
                await OnCompletedAsync(sender);
            }
            return sender;
        }

        
    }
#if UNITY_EDITOR
    public partial class UnvsInteractObject : UnvsComponent
    {

        public virtual void OnValidate()
        {
            this.SetMeOnLayer(LayerName);
        }
        public virtual void OnDrawGizmos()
        {
            var sp = GetComponent<SpriteRenderer>();
            if (sp != null)
            {
                if (sp.sprite == null)
                {
                    sp.ApplyDefaultBox();
                }
            }
            if (coll == null)
            {
                coll = this.GetComponent<BoxCollider2D>();
                coll.isTrigger = true;
            }

            if (sp != null)
            {
                sp.EditorSyncSize(coll, transform);

            }
            else
            {
                sp = this.GetComponentInChildren<SpriteRenderer>();
                sp?.EditorSyncSize(coll);

            }

        }
    }
#endif
}