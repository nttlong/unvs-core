using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.UI;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.ui
{
    public class InputActions
    {
        public InputAction Look { get; set; }
        public InputAction Move { get; set; }
        public InputAction Interact { get; set; }
        public InputAction Sprint { get; set; }
        public InputAction Next { get; set; }
       
    }
    public class UIInputActions
    {
        public InputAction Click { get; set; }
        public InputAction Pause { get; set; }
        public InputAction Cancel { get; set; }
        public InputAction ScrollWheel { get; set; }
    }
    public abstract class SettingsGlobalEvents : MonoBehaviour
    {
        public InputActions Player;

        public UIInputActions UI;

        private void Awake()
        {
            Player=new InputActions();
            UI = new UIInputActions();
            SetupController();
            GlobalApplication.GlobalInput = this;
        }
        

        

        public abstract void SetupController();

    }
}