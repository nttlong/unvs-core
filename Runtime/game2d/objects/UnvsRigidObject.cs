using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.shares;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(ShadowCaster2D))]
    public partial class UnvsRigidObject:UnvsBaseComponent
    {
        public virtual void Awake()
        {
            if (Application.isPlaying)
            {
                InitRuntime();
            }
        }

        public virtual void InitRuntime()
        {
            coll = this.GetComponent<BoxCollider2D>();
        }
    }
#if UNITY_EDITOR
    public partial class UnvsRigidObject : UnvsBaseComponent
    {
        public BoxCollider2D coll;

        public virtual void OnValidate()
        {
            this.SetMeOnLayer(Constants.Layers.WORLD_GROUND);
        }
        public virtual void OnDrawGizmos()
        {
            var sp = GetComponent<SpriteRenderer>();
            if(sp != null)
            {
                if (sp.sprite == null)
                {
                    sp.ApplyDefaultBox();
                }
            }
            if (coll == null)
                coll = this.GetComponent<BoxCollider2D>();
           
            
            if (sp != null)
            {
                sp.EditorSyncSize(coll, transform);
               
            }
            else
            {
                sp = this.GetComponentInChildren<SpriteRenderer>();
                sp.EditorSyncSize(coll);
                
            }

        }
    }
#endif
}