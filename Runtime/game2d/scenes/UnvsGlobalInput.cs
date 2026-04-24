
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.game2d.objects.components;
using static UnityEngine.InputSystem.DefaultInputActions;
using static UnityEngine.InputSystem.InputAction;

namespace unvs.game2d.scenes
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
                var action= OnRegsietEvent(key);
                if(action != null)
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
                    if (items[1]=="start")
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
    }
    public struct ActionSender
    {
        public string Name;

        public InputAction.CallbackContext Context;

        public Vector2 Pos;
        public bool Start;
    }
}
