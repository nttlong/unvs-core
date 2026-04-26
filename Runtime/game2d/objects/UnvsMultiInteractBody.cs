using unvs.game2d.scenes;
using UnityEngine;
using unvs.components;
namespace unvs.game2d.objects
{
    [RequireComponent(typeof(UnvsRigidBox))]
    public partial class UnvsMultiInteractBody:UnvsBaseComponent
    {
        
    }
#if UNITY_EDITOR
    public partial class UnvsMultiInteractBody : UnvsBaseComponent
    {
         
    }
#endif
}