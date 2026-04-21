using game2d.objects;
using UnityEngine;
using unvs.ext;
using unvs.game2d.transitions;
using unvs.shares;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(UnvsTransitionable))]
    public partial class UnvsRigidBox : UnvsRigidObject
    {
        
       

        public override void InitRuntime()
        {
            coll=GetComponent<BoxCollider2D>();
           
        }
    }
#if UNITY_EDITOR
    public partial class UnvsRigidBox : UnvsRigidObject
    {
        public override void OnDrawGizmos()
        {
            this.SetMeOnTag(Constants.Tags.RIGID_BOX);
            base.OnDrawGizmos();
        }
    }
#endif
}