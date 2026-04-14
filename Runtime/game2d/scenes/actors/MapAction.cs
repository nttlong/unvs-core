using System;
using static UnityEngine.InputSystem.InputAction;

namespace unvs.game2d.scenes.actors
    {
        public class MapAction
        {
            public event Action<CallbackContext> started;
            public event Action<CallbackContext> canceled;
            internal void InvokeStarted(CallbackContext ctx) => started?.Invoke(ctx);
            internal void InvokeCanceled(CallbackContext ctx) => canceled?.Invoke(ctx);
        }
    }