using UnityEngine;

namespace unvs.ui
{
    public abstract class UnvsUIComponentInstance<T> : UnvsUIComponent where T : Component
    {

        public static T Instance;
        public override void InitRunTime()
        {
            Instance = this as T;
           


        }

    }
}