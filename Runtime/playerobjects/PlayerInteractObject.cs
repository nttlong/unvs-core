using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;

namespace unvs.playerobjects
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class PlayerInteractObject : MonoBehaviour
    {
        public Collider2D coll;
        public ExploringType exploring;
        public InteractionDefinition data; 
      


        public virtual async UniTask<bool> ExecAsync(MonoBehaviour target, CancellationTokenSource token)
        {
            
            return await data.MonoBehaviourExecAsync( this,target, token.Token);
        }
        public virtual Vector2 GetPosition()
        {
            return coll.bounds.center;
        }
        public virtual void Awake()
        {
            if(Application.isPlaying)
            {
                InitRuntime();
            } else
            {
                InitDesignTime();
            }
        }

        public virtual void InitDesignTime()
        {
            this.coll=GetComponent<BoxCollider2D>();
            this.coll.isTrigger = true;
        }

        public virtual void EditorResizeCollider2D()
        {
            this.coll = GetComponent<BoxCollider2D>();
            if (this.coll is BoxCollider2D box)
            {
                box.ResizeByTransform();
                this.coll.isTrigger = true;
            }

        }

        public virtual void InitRuntime()
        {
            this.SetMeOnLayer(Constants.Layers.INTERACT_OBJECT);
        }
    }
}