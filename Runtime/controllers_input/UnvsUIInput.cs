using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.actor.player;
using unvs.components;
using unvs.components;
using static UnityEngine.InputSystem.InputAction;

namespace unvs.controllers.inputs {
    [Serializable]
    public class UnvsUIInput<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        public MonoBehaviour Controller;
        private bool _disableEvent;
        private Dictionary<string, Action<CallbackContext>> _started;
        private Dictionary<string, Action<CallbackContext>> _canceled;
        private Dictionary<string, Action<CallbackContext>> _performed;
       
        public MapAction NewMapAction(string name, Action<MapAction> InitEvents)
        {
            if (!UnvsGlobalInput.UI.Keys.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                Debug.LogError($"{name} was not found in {string.Join(',', UnvsGlobalInput.UI.Keys)}");
                return null;
            }
            var key = UnvsGlobalInput.UI.Keys.FirstOrDefault(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));
            var action = new MapAction();
            InitEvents(action);
            applyAction(key, action);
            return action;


        }
       

       
        public virtual void StartInputController()
        {
            if (Application.isPlaying)
            {

              
               
                var properties = this.GetType().GetProperties(
                    BindingFlags.Instance |     
                    BindingFlags.Public |      
                    BindingFlags.NonPublic     
                ).Where(p => p.PropertyType == typeof(MapAction));
                foreach (var item in properties)
                {
                    item.GetValue(this);
                }
               
            }
        }

        private void applyAction(string key, MapAction action)
        {
            if (action != null)
            {
               

                if (action.hasStarted)
                {
                    Action<CallbackContext> start = ctx =>
                    {
                        if (!_disableEvent)
                            action.InvokeStarted(ctx);
                    };
                    UnvsGlobalInput.UI[key].started += start;
                    if (_started == null) _started = new Dictionary<string, Action<CallbackContext>>();
                    _started.Add(key, start);
                }
                if (action.hasCanceled)
                {
                    Action<CallbackContext> canceled = ctx =>
                    {
                        if (!_disableEvent)
                            action.InvokeCanceled(ctx);
                    };
                    UnvsGlobalInput.UI[key].canceled += canceled;
                    if (_canceled == null) _canceled = new Dictionary<string, Action<CallbackContext>>();
                    _canceled.Add(key, canceled);
                }
                if (action.hasPerformed)
                {
                    Action<CallbackContext> performed = ctx =>
                    {
                        if (!_disableEvent)
                            action.InvokePerformedd(ctx);
                    };
                    UnvsGlobalInput.UI[key].performed += performed;
                    if (_performed == null) _performed = new Dictionary<string, Action<CallbackContext>>();
                    _performed.Add(key, performed);
                }

                action._ownerInputAction = UnvsGlobalInput.UI[key]; // set owner for other method call such as ReadValue ...
            }
        }

        

        public void ClearAllEvents()
        {
           
            if (_started != null)
            {
                foreach (var item in _started)
                {
                    if (UnvsGlobalInput.UI.TryGetValue(item.Key, out var inputAction))
                        inputAction.started -= item.Value;
                }
                _started.Clear();
            }

            if (_canceled != null)
            {
                foreach (var item in _canceled)
                {
                    if (UnvsGlobalInput.UI.TryGetValue(item.Key, out var inputAction))
                        inputAction.canceled -= item.Value;
                }
                _canceled.Clear();
            }
            if (_performed != null)
            {
                foreach (var item in _performed)
                {
                    if (UnvsGlobalInput.UI.TryGetValue(item.Key, out var inputAction))
                        inputAction.performed -= item.Value;
                }
            }
        }

        public void ControlDisable()
        {
            _disableEvent = true;
        }
        public void ControlEnable()
        {
            _disableEvent = false;
        }
    }
}