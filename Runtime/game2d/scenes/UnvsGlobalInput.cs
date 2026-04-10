
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

namespace unvs.game2d.scenes
{
    public class UnvsGlobalInput
    {
        public static Dictionary<string, InputAction> Player;
        private static object playerValue;
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
            var PlayerPro = typ.GetProperty("UI");
            if (PlayerPro != null)
            {
                UI = new Dictionary<string, InputAction>();
                playerValue = PlayerPro.GetValue(inputIns);
                var pts = PlayerPro.PropertyType.GetProperties().Where(p => p.PropertyType == typeof(InputAction));
                foreach (var p in pts)
                {
                    var val = p.GetValue(playerValue) as InputAction;
                    UI[p.Name] = val;
                   

                }
            }
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
