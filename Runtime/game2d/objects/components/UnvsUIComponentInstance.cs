using UnityEngine;

namespace unvs.game2d.objects.components
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