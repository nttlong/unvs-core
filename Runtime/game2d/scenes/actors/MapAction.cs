using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace unvs.game2d.scenes.actors
{
    public class MapAction
    {
        public InputAction OwnerInputAction;

        public event Action<CallbackContext> started;
        public event Action<CallbackContext> canceled;
        public event Action<CallbackContext> performed;
        internal void InvokeStarted(CallbackContext ctx) => started?.Invoke(ctx);
        internal void InvokeCanceled(CallbackContext ctx) => canceled?.Invoke(ctx);
        internal void InvokePerformedd(CallbackContext ctx) => performed?.Invoke(ctx);

        internal bool hasStarted => started != null;
        internal bool hasCanceled=>canceled!=null;
        internal bool hasPerformed => performed != null;

    }
}
