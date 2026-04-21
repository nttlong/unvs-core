using UnityEngine;
using unvs.game2d.transitions;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class UnvsRigidBox : UnvsRigidObject
    {
        public UnvsTransitionDefinitions Transition;
        public float Height;
        public float duration;

        public bool IsOn;

        public override void InitRuntime()
        {
            coll=GetComponent<BoxCollider2D>();
            Height = coll.bounds.size.y;
        }
    }
}