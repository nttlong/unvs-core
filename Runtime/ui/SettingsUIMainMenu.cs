using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using unvs.ext;
using unvs.gameword;
using unvs.interfaces;
using unvs.shares;
namespace unvs.ui
{
    [ExecuteInEditMode]
    public class SettingsUIMainMenu : MonoBehaviour, IMainMenu
    {
        public Canvas menuCanvas;
        private Image panel;

        public Action OnStartClick { get; set ; }
        public Action OnExitClick { get ; set; }

        public Canvas MenuCanvas => menuCanvas;

        public Image Panel => panel;

        public bool IsShowing { get; internal set; }

        public void DoExitGame()
        {
            OnExitClick?.Invoke();
            
        }

        public void DoStartGame()
        {
            OnStartClick?.Invoke();
        }

        public void Hide()
        {
            this.IsShowing = false;
            this.menuCanvas.DoDeactive();
           
        }

        public void Show()
        {
            //this.menuCanvas.enabled = true;
            this.menuCanvas.DoActive();
            var FirstButton=this.GetComponentInChildren<Button>();
            if(FirstButton != null )
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(FirstButton.gameObject);
            }
            this.IsShowing = true;
        }
        private void Awake()
        {
           menuCanvas= this.AddChildComponentIfNotExist<Canvas>("CanvasMain");
            menuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
            panel = menuCanvas.transform.AddChildComponentIfNotExist<Image>("PanelMain");
            panel.DockFull();
            GlobalApplication.UIMainMenu = this;
        }
        private GameObject _lastSelectedButton;

        void Update()
        {
            if (menuCanvas.enabled)
            {
               
                if (EventSystem.current.currentSelectedGameObject != null &&
                    EventSystem.current.currentSelectedGameObject != _lastSelectedButton)
                {
                    _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
                }

               
                if (EventSystem.current.currentSelectedGameObject == null && _lastSelectedButton != null)
                {
                   
                    EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
                }
            }
        }
    }
}