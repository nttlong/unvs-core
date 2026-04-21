using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class UnvsMultiInteractPoint : UnvsInteractObject
    {
        public UnvsMultiInteractBody owner;
    }
#if UNITY_EDITOR
    public partial class UnvsMultiInteractPoint : UnvsInteractObject
    {
       
    }
#endif
}