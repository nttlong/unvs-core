using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.interfaces;
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
            this.menuCanvas.enabled = false;
            this.menuCanvas.gameObject.SetActive(false);
        }

        public void Show()
        {
            this.menuCanvas.enabled = true;
            this.menuCanvas.gameObject.SetActive(true);
        }
        private void Awake()
        {
           menuCanvas= this.AddChildComponentIfNotExist<Canvas>("CanvasMain");
            menuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
            panel = menuCanvas.transform.AddChildComponentIfNotExist<Image>("PanelMain");
            panel.DockFull();
        }
        private void Start()
        {
            
        }
    }
}