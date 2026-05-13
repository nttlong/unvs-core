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
    public abstract class UnvsNonUIComponentInstance<T> : UnvsNonUIComponent where T : Component
    {

        public static T Instance;
        public override void InitRunTime()
        {
            Instance = this as T;



        }

    }
}