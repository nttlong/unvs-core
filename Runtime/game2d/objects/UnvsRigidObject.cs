using log4net.Util;
using System;
using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.shares;

namespace unvs.game2d.objects
{
    
    public  abstract partial class UnvsRigidObject:UnvsBaseComponent
    {
        
        public Collider2D coll;
        public virtual void Awake()
        {
            if (Application.isPlaying)
            {
                InitRuntime();
            }
        }

        public abstract void InitRuntime();
    }
#if UNITY_EDITOR
    public  abstract partial class UnvsRigidObject : UnvsBaseComponent
    {
        

        public virtual void OnValidate()
        {
            this.SetMeOnLayer(Constants.Layers.WORLD_GROUND);
        }
        public virtual void OnDrawGizmos()
        {
            if (coll == null)
                coll = this.GetComponent<BoxCollider2D>();
           
            var sp = this.GetComponent<SpriteRenderer>();
           
            if(coll is BoxCollider2D boxColl)
            {
                if (sp != null)
                {
                    SpriteRendererExtension.FixCollider2DSize(sp, boxColl);
                    float width = sp.sprite.rect.width / sp.sprite.pixelsPerUnit;
                    float height = sp.sprite.rect.height / sp.sprite.pixelsPerUnit;
                    boxColl.size = new Vector2(width, height);// sp.transform.localScale;
                    boxColl.offset = Vector2.zero;
                    // coll.offset =new ( sp.transform.position.x/2,sp.transform.position.y);//= coll.offset;
                    coll.transform.rotation = transform.rotation;// = coll.transform.rotation;
                }
                else
                {
                    sp = this.GetComponentInChildren<SpriteRenderer>();
                    if (sp != null && coll != null)
                    {
                        sp.transform.localScale = boxColl.size;
                        sp.transform.localPosition = coll.offset;
                        // coll.offset =new ( sp.transform.position.x/2,sp.transform.position.y);//= coll.offset;
                        coll.transform.rotation = sp.transform.rotation;// = coll.transform.rotation;
                    }
                }
            }
            

        }
    }
#endif
}