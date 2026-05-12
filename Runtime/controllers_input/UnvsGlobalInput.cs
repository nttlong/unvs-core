
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using unvs.actor.player;
using unvs.components;
using static UnityEngine.InputSystem.DefaultInputActions;
using static UnityEngine.InputSystem.InputAction;

namespace unvs.controllers.inputs
{
    public class UnvsEventAction
    {
        public Action<CallbackContext> Started;
        public Action<CallbackContext> Cancel;
    }
    public class UnvsGlobalInput
    {
        public static Dictionary<string, InputAction> Player;
        private static object playerValue;
        private static object uiValue;
        public static Dictionary<string, InputAction> UI;

        public static object inputIns;


        public static InputAction LookAction { get; private set; }

        public static void Disable()
        {
            if (inputIns != null)
            {
                var typ = inputIns.GetType();
                typ.GetMethod("Disable").Invoke(inputIns, null);
            }
        }
        public static void Enable()
        {
            if (inputIns != null)
            {
                var typ = inputIns.GetType();
                typ.GetMethod("Enable").Invoke(inputIns, null);
            }

        }
        public static void PlayerEnable()
        {
            if (playerValue != null)
            {
                var typ = playerValue.GetType();
                typ.GetMethod("Enable").Invoke(playerValue, null);
            }

        }
        public static void PlayerDisable()
        {
            if (playerValue != null)
            {
                var typ = playerValue.GetType();
                typ.GetMethod("Disable").Invoke(playerValue, null);
            }

        }
        public static void UIEnable()
        {
            if (uiValue != null)
            {
                var typ = uiValue.GetType();
                typ.GetMethod("Enable").Invoke(uiValue, null);
            }

        }
        public static void UIDisable()
        {
            if (uiValue != null)
            {
                var typ = uiValue.GetType();
                typ.GetMethod("Disable").Invoke(uiValue, null);
            }

        }
        public static event Action OnPlayerInputReady;
        static event Action _OnUIInputReady;
        public static event Action OnUIInputReady
        {
            add
            {
                if (UI == null)
                {
                    _OnUIInputReady += value;
                } else
                {
                    value?.Invoke();
                }
            }
            remove
            {
                if (UI == null)
                {
                    _OnUIInputReady -= value;
                }
            }
        }
        internal static void MapPlayerEvents()
        {
            var typ = inputIns.GetType();
            var PlayerPro = typ.GetProperty("Player");
            if (PlayerPro != null)
            {
                Player = new Dictionary<string, InputAction>();
                playerValue = PlayerPro.GetValue(inputIns);
                var pts = PlayerPro.PropertyType.GetProperties().Where(p => p.PropertyType == typeof(InputAction));
                foreach (var p in pts)
                {
                    var val = p.GetValue(playerValue) as InputAction;
                    if (p.Name == "Look")
                    {
                        LookAction = val;
                    }
                    Player[p.Name] = val;

                }
                OnPlayerInputReady?.Invoke();
            }
            
        }
        internal static void MapUIEvents()
        {
            var typ = inputIns.GetType();
            var UIProperty = typ.GetProperty("UI");
            if (UIProperty != null)
            {
                UI = new Dictionary<string, InputAction>();
                uiValue = UIProperty.GetValue(inputIns);
                var pts = UIProperty.PropertyType.GetProperties().Where(p => p.PropertyType == typeof(InputAction));
                foreach (var p in pts)
                {
                    var val = p.GetValue(uiValue) as InputAction;
                    UI[p.Name] = val;


                }
                _OnUIInputReady?.Invoke();
            }
            
        }

        internal static void SetActivePlayer(bool v)
        {
            throw new NotImplementedException();
        }

        public static void RegisterPlayer<T>(T component, Func<string, UnvsEventAction> OnRegsietEvent) where T : UnvsComponentEvetns
        {
            var dict = new Dictionary<string, Action<CallbackContext>>();
            foreach (var key in Player.Keys)
            {
                var action = OnRegsietEvent(key);
                if (action != null)
                {

                    if (action.Started != null)
                    {
                        void start(CallbackContext ctx)
                        {
                            action.Started.Invoke(ctx);
                        }
                        dict[$"{key}.start"] = start;
                        Player[key].started += start;
                    }
                    if (action.Cancel != null)
                    {
                        void cancel(CallbackContext ctx)
                        {
                            action.Cancel.Invoke(ctx);
                        }
                        dict[$"{key}.cancel"] = cancel;
                        Player[key].started += cancel;
                    }
                }
            }
            component.onDisable = () =>
            {
                foreach (var key in dict.Keys)
                {
                    var items = key.Split('.');
                    if (items[1] == "start")
                    {
                        Player[key].started -= dict[key];
                    }
                    if (items[1] == "cancel")
                    {
                        Player[key].canceled -= dict[key];
                    }
                }
            };
        }
        public static MapAction NewMapPlayerAction(UnvsBaseComponent component,string name, Action<MapAction> InitEvents)
        {
            if(!Application.isPlaying) return null;
            if (!Player.Keys.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                Debug.LogError($"{name} was not found in {string.Join(',', UnvsGlobalInput.Player.Keys)}");
                return null;
            }
            var key = Player.Keys.FirstOrDefault(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));
            var action = new MapAction();
            InitEvents(action);
            applyAction(key, action, component);
            return action;


        }
        public static MapAction NewMapUIAction(UnvsBaseComponent component, string name, Action<MapAction> InitEvents)
        {
            
            if (!UI.Keys.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                Debug.LogError($"{name} was not found in {string.Join(',', UnvsGlobalInput.Player.Keys)}");
                return null;
            }
            var key = UI.Keys.FirstOrDefault(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));
            var action = new MapAction();
            InitEvents(action);
            applyUIAction(key, action, component);
            return action;


        }

        private static bool InvalidateComponent(UnvsBaseComponent component)
        {

            if (component == null)
            {
                return true;
            }


            if (!component.enabled || !component.gameObject.activeInHierarchy)
            {
                return true;
            }


            return false;
        }
        static void applyAction(string key, MapAction action, UnvsBaseComponent component)
     => BindInput(Player[key], action, component);

        static void applyUIAction(string key, MapAction action, UnvsBaseComponent component)
            => BindInput(UI[key], action, component);

        private static void BindInput(InputAction input, MapAction action, UnvsBaseComponent component)
        {
            if (action == null || input == null) return;

            if (action.hasStarted)
            {
                Action<CallbackContext> start = ctx => {
                    if (InvalidateComponent(component)) return;
                    action.InvokeStarted(ctx);
                };
                input.started += start;
                component.OnDestroying += () => input.started -= start;
            }

            if (action.hasCanceled)
            {
                Action<CallbackContext> canceled = ctx => {
                    if (InvalidateComponent(component)) return;
                    action.InvokeCanceled(ctx);
                };
                input.canceled += canceled;
                component.OnDestroying += () => input.canceled -= canceled;
            }

            if (action.hasPerformed)
            {
                Action<CallbackContext> performed = ctx => {
                    if (InvalidateComponent(component)) return;
                    action.InvokePerformed(ctx); // Đã sửa typo
                };
                input.performed += performed;
                component.OnDestroying += () => input.performed -= performed;
            }
        }

        public static void ExitGame(bool IsEditorMode)
        {
            Application.Quit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();

        }

        public static void EditorExitGame()
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
    public struct ActionSender
    {
        public string Name;

        public InputAction.CallbackContext Context;

        public Vector2 Pos;
        public bool Start;
    }
}
