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

using static UnityEngine.InputSystem.InputAction;
#endif

namespace unvs.game2d.scenes.actors {
    
    public abstract class UnvsPlayer : UnvsBaseComponent
    {
        public InputAction Look;
        public class MapAction
        {
            public event Action<CallbackContext> started;
            public event Action<CallbackContext> canceled;
            internal void InvokeStarted(CallbackContext ctx) => started?.Invoke(ctx);
            internal void InvokeCanceled(CallbackContext ctx) => canceled?.Invoke(ctx);
        }
        Dictionary<string, Action<CallbackContext>> _started = null;
        Dictionary<string,Action<CallbackContext>> _canceled = null;
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
                foreach (var key in UnvsGlobalInput.Player.Keys)
                {
                    var action = OnMapConrrol(key.ToLower());
                    if(action != null)
                    {
                        // kiem tra action co start thi goi

                        Action<CallbackContext> start = ctx =>
                        {
                            if(!_disableEvent)
                            action.InvokeStarted(ctx);
                        };
                        UnvsGlobalInput.Player[key].started += start;
                        _started.Add(key, start);
                        Action<CallbackContext> canceled = ctx =>
                        {
                            if (!_disableEvent)
                                action.InvokeCanceled(ctx);
                        };
                        UnvsGlobalInput.Player[key].canceled += canceled;
                        _canceled.Add(key, canceled);
                    }
                    
                }
            }
        }



        public virtual void OnDisable()
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