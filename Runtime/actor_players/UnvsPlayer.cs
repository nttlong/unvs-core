using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.controllers_input;
using unvs.ext;
using unvs.game2d.actors;
using unvs.game2d.objects.components;
using unvs.game2d.scenes;


#if UNITY_EDITOR

using unvs.sys;


#endif
using static UnityEngine.InputSystem.InputAction;
namespace unvs.actor.player {
    
    public abstract class UnvsPlayer : UnvsBaseComponent
    {
        public InputAction Look;
        
        Dictionary<string, Action<CallbackContext>> _started = null;
        Dictionary<string,Action<CallbackContext>> _canceled = null;
        Dictionary<string, Action<CallbackContext>> _performed = null;
        private bool _disableEvent;
        protected MapAction NewMapAction(string name,Action<MapAction> InitEvents)
        {
            if (!UnvsGlobalInput.Player.Keys.Contains(name,StringComparer.OrdinalIgnoreCase))
            {
                Debug.LogError($"{name} was not found in {string.Join(',', UnvsGlobalInput.Player.Keys)}");
                return null;
            }
            var key= UnvsGlobalInput.Player.Keys.FirstOrDefault(p=>p.Equals(name,StringComparison.OrdinalIgnoreCase));
            var action = new MapAction();
            InitEvents(action);
            applyAction(key, action);
            return action;


        }
        public abstract MapAction OnMapConrrol(string name);
       
        public abstract void InitRuntime();
        public virtual void Awake()
        {
            if (Application.isPlaying)
            {
                
                Look = UnvsGlobalInput.Player["Look"];
                InitRuntime();
                var properties = this.GetType().GetProperties(
                    BindingFlags.Instance |     // Lấy các property của đối tượng (không phải static)
                    BindingFlags.Public |       // Lấy cả Public
                    BindingFlags.NonPublic      // QUAN TRỌNG: Lấy cả Protected và Private
                ).Where(p=>p.PropertyType==typeof(MapAction));
                foreach (var item in properties)
                {
                    item.GetValue(this);
                }
                _started =new Dictionary<string, Action<CallbackContext>>();
                _canceled = new Dictionary<string, Action<CallbackContext>>();
                _performed = new Dictionary<string, Action<CallbackContext>>();

                foreach (var key in UnvsGlobalInput.Player.Keys)
                {
                    var action = OnMapConrrol(key.ToLower());
                    applyAction(key, action);

                }
            }
        }

        private void applyAction(string key, MapAction action)
        {
            if (action != null)
            {
                // kiem tra action co start thi goi

                if (action.hasStarted)
                {
                    Action<CallbackContext> start = ctx =>
                    {
                        if (!_disableEvent)
                            action.InvokeStarted(ctx);
                    };
                    UnvsGlobalInput.Player[key].started += start;
                    if(_started==null) _started=new Dictionary<string, Action<CallbackContext>>();
                    _started.Add(key, start);
                }
                if (action.hasCanceled)
                {
                    Action<CallbackContext> canceled = ctx =>
                    {
                        if (!_disableEvent)
                            action.InvokeCanceled(ctx);
                    };
                    UnvsGlobalInput.Player[key].canceled += canceled;
                    if(_canceled==null) _canceled= new Dictionary<string, Action<CallbackContext>>();
                    _canceled.Add(key, canceled);
                }
                if (action.hasPerformed)
                {
                    Action<CallbackContext> performed = ctx =>
                    {
                        if (!_disableEvent)
                            action.InvokePerformedd(ctx);
                    };
                    UnvsGlobalInput.Player[key].performed += performed;
                    if(_performed==null) _performed = new Dictionary<string, Action<CallbackContext>>();
                    _performed.Add(key, performed);
                }

                action._ownerInputAction = UnvsGlobalInput.Player[key]; // set owner for other method call such as ReadValue ...
            }
        }

        public virtual void OnDestroy()
        {
            ClearAllEvents();
        }

        public virtual void OnDisable()
        {
            ClearAllEvents();
        }

        private void ClearAllEvents()
        {
            // Sử dụng cơ chế an toàn hơn khi truy cập Global Input
            if (_started != null)
            {
                foreach (var item in _started)
                {
                    if (UnvsGlobalInput.Player.TryGetValue(item.Key, out var inputAction))
                        inputAction.started -= item.Value;
                }
                _started.Clear();
            }

            if (_canceled != null)
            {
                foreach (var item in _canceled)
                {
                    if (UnvsGlobalInput.Player.TryGetValue(item.Key, out var inputAction))
                        inputAction.canceled -= item.Value;
                }
                _canceled.Clear();
            }
            if (_performed != null)
            {
                foreach (var item in _performed)
                {
                    if (UnvsGlobalInput.Player.TryGetValue(item.Key, out var inputAction))
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