using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.ext;
#if UNITY_EDITOR
using unvs.shares.editor;
using unvs.sys;


#endif
using static UnityEngine.InputSystem.InputAction;
namespace unvs.game2d.scenes.actors {
    
    public abstract class UnvsPlayer : UnvsBaseComponent
    {
        public InputAction Look;
        
        Dictionary<string, Action<CallbackContext>> _started = null;
        Dictionary<string,Action<CallbackContext>> _canceled = null;
        Dictionary<string, Action<CallbackContext>> _performed = null;
        private bool _disableEvent;

        public abstract MapAction OnMapConrrol(string name);
       
        public abstract void InitRuntime();
        public virtual void Awake()
        {
            if (Application.isPlaying)
            {
                
                Look = UnvsGlobalInput.Player["Look"];
                InitRuntime();
                _started =new Dictionary<string, Action<CallbackContext>>();
                _canceled = new Dictionary<string, Action<CallbackContext>>();
                _performed = new Dictionary<string, Action<CallbackContext>>();
                foreach (var key in UnvsGlobalInput.Player.Keys)
                {
                    var action = OnMapConrrol(key.ToLower());
                    if(action != null)
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
                            _performed.Add(key, performed); 
                        }

                        action.OwnerInputAction = UnvsGlobalInput.Player[key]; // set owner for other method call such as ReadValue ...
                    }
                    
                }
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