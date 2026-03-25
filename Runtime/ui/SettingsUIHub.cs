using Cysharp.Threading.Tasks.Triggers;
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
    public class SettingsUIHub : MonoBehaviour, IUIHub
    {
        private Canvas hubCanvas;
        private Image hubPanel;

        public Canvas HubCanvas => hubCanvas;

        public Image HubPanel => hubPanel;

        public void Hide()
        {
            if(this.hubCanvas != null)
            {
                this.hubCanvas.enabled = false;
                this.hubCanvas.gameObject.SetActive(false);
            } 
        }

        public void Show()
        {
            if (this.hubCanvas != null)
            {
                this.hubCanvas.enabled = true;
                this.hubCanvas.gameObject.SetActive(true);
            }
        }

        private void Awake()
        {
            hubCanvas = this.AddChildComponentIfNotExist<Canvas>("HubCanvas");
            hubPanel = hubCanvas.transform.AddChildComponentIfNotExist<Image>("HubPanel");

        }
        private void Start()
        {
            if(Application.isPlaying)
            GlobalApplication.UIHub = this as IUIHub;
            this.Hide();
        }
    }
}