using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.ui
{
    [ExecuteInEditMode]
    public class SettingsUIFadeScreen : MonoBehaviour, IFadeScreen
    {
        private Canvas uiCanvas;
        private Image panel;

        public Canvas UICanvas => uiCanvas;

        public Image Panel => panel;

        public async UniTask FadeInAsync(float fadingTime = 0.5F)
        {
            Time.timeScale = 1;
            this.uiCanvas.enabled = true;
            this.uiCanvas.gameObject.SetActive(true);
            await panel.FadeInAsync(fadingTime);
        }

        public async UniTask FadeOutAsync(float fadingTime = 0.5F)
        {
            Time.timeScale = 1;
            await panel.FadeOutAsync(fadingTime);
            this.uiCanvas.enabled = false;
            this.uiCanvas.gameObject.SetActive(false);
        }

        public void Hide()
        {
            if(this.uiCanvas != null)
            {
                this.uiCanvas.enabled=false;
                this.uiCanvas.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            uiCanvas= this.AddChildComponentIfNotExist<Canvas>("FadeScreenCanvas");
            uiCanvas.AddComponentIfNotExist<GraphicRaycaster>();
            panel = uiCanvas.transform.AddChildComponentIfNotExist<Image>("FadeScreenPanel");
        }
        private void Start()
        {
            uiCanvas.FullSize();
            panel.DockFull();
            if (Application.isPlaying)
            {
                GlobalApplication.FadeScreenController = this;
                this.Hide();
            }
            
        }
    }
}