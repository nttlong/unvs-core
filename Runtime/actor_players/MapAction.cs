using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace unvs.actor.player
{
    public class MapAction
    {
        internal InputAction _ownerInputAction;

        public event Action<CallbackContext> started;
        public event Action<CallbackContext> canceled;
        public event Action<CallbackContext> performed;
        internal void InvokeStarted(CallbackContext ctx) => started?.Invoke(ctx);
        internal void InvokeCanceled(CallbackContext ctx) => canceled?.Invoke(ctx);
        internal void InvokePerformedd(CallbackContext ctx) => performed?.Invoke(ctx);

        public T ReadValue<T>() where T : struct
        {
            return _ownerInputAction.ReadValue<T>();
        }

        internal bool hasStarted => started != null;
        internal bool hasCanceled=>canceled!=null;
        internal bool hasPerformed => performed != null;

    }
}
