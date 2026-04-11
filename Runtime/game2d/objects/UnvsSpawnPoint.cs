using UnityEngine;
using unvs.game2d.scenes;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    public partial class UnvsSpawnPoint : UnvsBaseComponent
    {

    }
#if UNITY_EDITOR
    public partial class UnvsSpawnPoint : UnvsBaseComponent
    {

        private void OnValidate()
        {
            var coll = GetComponent<BoxCollider2D>();
            if (coll != null)
            {
                coll.isTrigger = true;
                coll.offset = Vector3.zero;
                
            }
        }

    }
#endif

}