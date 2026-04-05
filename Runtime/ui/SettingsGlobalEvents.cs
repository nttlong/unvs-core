using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows;
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
                if (value)
                {
                    OnEnable?.Invoke();
                }
                else
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
    /// <summary>
    /// <code>
    /// public override object OnRequest_Input_System()
    ///{
    ///    inputs=new InputSystem_Actions();
    ///    return inputs;
    ///}
    ///public override void RaiseOnExitGame()
    ///{
    //    this.inputs.Disable();
    ///}
    ///public override void SetupController()
    ///{
    ///}
    /// </code>
    /// </summary>
    public abstract class SettingsGlobalEvents : MonoBehaviour
    {
        public InputActions Player { get; set; }

        public UIInputActions UI { get; set; }

        private object inputs;
        private object playerObj;
        private MethodInfo playerObjDisableMethod;
        private MethodInfo playerObjEnableMethod;
        private object uiObj;
        private MethodInfo uiObjEnableMethod;
        private MethodInfo inputsDisableMethod;

        private void Awake()
        {
            if (!Application.isPlaying) return;
            Player = new InputActions();
            UI = new UIInputActions();
            inputs = OnRequest_Input_System();
            playerObj = inputs.GetType().GetProperty("Player").GetValue(inputs, null);
            playerObjDisableMethod = playerObj.GetType().GetMethod("Disable");
            playerObjEnableMethod = playerObj.GetType().GetMethod("Enable");
            uiObj = inputs.GetType().GetProperty("UI").GetValue(inputs, null);
            uiObjEnableMethod = uiObj.GetType().GetMethod("Enable");
            inputsDisableMethod = inputs.GetType().GetMethod("Enable");
            Commons.DoMapPalerAndUIAction(inputs, this);
            GlobalApplication.GlobalInput = this;
            SetupController();
            GlobalApplication.GlobalInput = this;
            
        }

        public abstract object OnRequest_Input_System();

        ///// <summary>
        ///// Create new Input_system then return in this function
        ///// </summary>
        ///// <returns></returns>
        //public abstract object OnMapInputSystem();

      
        public virtual void SetupController()
        {
            if (!Application.isPlaying) return;

            UI.Pause.started += ctx =>
            {
                var scene = GetComponent<ISingleScene>();
                scene.PauseMenu.Toggle();
            };


            Player.OnDisable = () =>
            {
                this.playerObjDisableMethod.Invoke(this.playerObj, new object[] { });
            };
            Player.OnEnable = () => {
                this.playerObjEnableMethod.Invoke(this.playerObj, new object[] { });
            };

            this.playerObjEnableMethod.Invoke(this.playerObj, new object[] { });
            this.uiObjEnableMethod.Invoke(this.uiObj, new object[] { });

        }

        internal void RaiseOnExitGame()
        {
            inputsDisableMethod.Invoke(this.inputs, new object[] { });
        }
    }
}