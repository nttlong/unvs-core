using UnityEngine;

namespace unvs.game2d.objects.components
{
    public abstract class UnvsComponent : UnvsBaseComponent
    {

        public virtual void Awake()
        {
            if (Application.isPlaying)

                InitRuntime();

        }


        public abstract void InitRuntime();
    }
}