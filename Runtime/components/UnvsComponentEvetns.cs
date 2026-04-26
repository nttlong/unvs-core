using System;

namespace unvs.components
{
    public abstract class UnvsComponentEvetns: UnvsComponent
    {
        internal Action onDisable;
        private void OnDisable()
        {
            onDisable.Invoke();

        }
    }
}