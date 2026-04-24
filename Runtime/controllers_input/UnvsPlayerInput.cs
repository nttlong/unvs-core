

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using static UnityEngine.InputSystem.DefaultInputActions;

namespace unvs.controllers_input
{
    
    public abstract class UnvsPlayerInputMap : UnvsBaseComponent
    {
        public static UnvsPlayerInputMap Instance { get; private set; }
        public InputAction LookAction { get; private set; }
        public string[] UI;

        public string[] Players;
      
       
        
        public abstract object OnNew();
        private void Awake()
        {
            if (Application.isPlaying)
            {
                Instance = this;
                UnvsGlobalInput.inputIns = OnNew();
                UnvsGlobalInput.MapPlayerEvents();
                UnvsGlobalInput.MapUIEvents();
                UnvsGlobalInput.Enable();
            }
        }
        public void OnEnable()
        {
            UnvsGlobalInput.Enable();
        }
        public void OnDisable()
        {
            UnvsGlobalInput.Disable();
        }
#if UNITY_EDITOR
        [UnvsButton]
        public void DoMappingPlayerEvents()
        {
           var inputIns = OnNew();
            if (inputIns == null)
            {
                Debug.LogError($"Create new Monobehavior inherit {typeof(UnvsPlayerInputMap)} then add");
            }
            var typ=inputIns.GetType();
            var PlayerPro = typ.GetProperty("Player");
            var lst=new List<string>();
            if (PlayerPro != null)
            {
                var pts = PlayerPro.PropertyType.GetProperties().Where(p => p.PropertyType == typeof(InputAction));
                foreach ( var p in pts )
                {
                    lst.Add(p.Name);
                }
            }
            this.Players = lst.ToArray();
            var UIPro = typ.GetProperty("UI");
            var lstUI = new List<string>();
            if (lstUI != null)
            {
                var pts = UIPro.PropertyType.GetProperties().Where(p => p.PropertyType == typeof(InputAction));
                foreach (var p in pts)
                {
                    lstUI.Add(p.Name);
                }
            }
            this.UI = lstUI.ToArray();
        }
#endif
    }
    public class UnvsPlayerInput : UnvsBaseComponent
    {
        public UnvsPlayerInputMap InputMap;

        public virtual void Awake()
        {
            if (Application.isPlaying)
            {
                InputMap=GetComponent<UnvsPlayerInputMap>(); 
                if(InputMap == null)
                {
                    Debug.LogError($"Create new Monobehavior inherit {typeof(UnvsPlayerInputMap)} then add");
                }
            }
        }

    }
}