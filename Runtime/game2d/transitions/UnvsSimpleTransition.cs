using UnityEngine;
using DG.Tweening;
namespace unvs.game2d.transitions
{
    public enum TransitionEnum
    {
        Down,
        Up
    }
    public class UnvsSimpleTransition : UnvsTransitionBase
    {
        public TransitionEnum Transition;
        public float DutaionTime = 5f;
        public override void Execute(MonoBehaviour source)
        {
            var rigBox= source.GetComponentInParent<Rigidbody>();
            if (rigBox == null) return;
            var coll= rigBox.GetComponent<Collider>();
            var distance = rigBox.transform.position.y- coll.bounds.size.y;
            rigBox.transform.parent.DOMoveY(distance, DutaionTime);
        }
    }
}
