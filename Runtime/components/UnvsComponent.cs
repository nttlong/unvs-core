using UnityEngine;
using unvs.components;

namespace unvs.components
{
    public abstract class UnvsComponent : UnvsBaseComponent
    {

        public virtual void Awake()
        {
            //InitProperties();
            if (Application.isPlaying)

                InitRuntime();

        }


        public abstract void InitRuntime();
    }
}