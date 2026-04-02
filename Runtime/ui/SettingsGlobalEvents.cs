using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
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
        private bool _enable;
        public Action OnEnable;
        public Action OnDisable;
        public bool enable
        {
            get
            {
                return _enable;
            }
            set
            {
                _enable = value;
                if(value)
                {
                    OnEnable?.Invoke();
                } else
                {
                    OnDisable?.Invoke();
                }
            }
        }

            
        

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
        public InputAction Submit { get; set; }
        
    }
    public abstract class SettingsGlobalEvents : MonoBehaviour
    {
        public InputActions Player { get; set; }

        public UIInputActions UI { get; set; }

        private void Awake()
        {
            Player=new InputActions();
            UI = new UIInputActions();
            var inputs=OnMapInputSystem();
            Commons.DoMapPalerAndUIAction(inputs, this);
            GlobalApplication.GlobalInput = this;
            SetupController();
            GlobalApplication.GlobalInput = this;
        }

        public abstract object OnMapInputSystem();
       

        public abstract void SetupController();

        public abstract void RaiseOnExitGame();
       
    }
}