using UnityEngine;

namespace unvs.game2d.objects
{
    public  partial class UnvsPickableObject : UnvsInteractObject
    {
        public Texture2D inventoryIcon;
        public override void InitRuntime()
        {
            base.InitRuntime();
            coll.isTrigger = true;
        }
        
    }
    #if UNITY_EDITOR
    public partial class UnvsPickableObject : UnvsInteractObject
    {
       public override void OnValidate()
        {
            base.OnValidate();
            coll=this.GetComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }
        public override void OnDrawGizmos()
        {
            
        }
    }
    #endif
}