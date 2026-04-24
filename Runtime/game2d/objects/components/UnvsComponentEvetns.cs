using System;

namespace unvs.game2d.objects.components
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