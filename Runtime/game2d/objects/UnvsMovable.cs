using UnityEngine;
using unvs.shares;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class UnvsMovable:UnvsInteractObject
    {
        
    }
#if UNITY_EDITOR
    public partial class UnvsMovable:UnvsInteractObject
    {
        public override void OnValidate()
        {
            base.OnValidate();
            if (GetComponent<SpriteRenderer>().sprite == null)
                GetComponent<SpriteRenderer>().sprite = Commons.LoadAsset<Sprite>("Packages/com.unvs.core/Runtime/Sprites/Square.png");

        }
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
        }
    }
#endif
}