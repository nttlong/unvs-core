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
    public class SettingsUIPauseMenu : MonoBehaviour, IPauseMenu
    {
        public Canvas pauseMenuCanvas;
        public Image pauseMenuPanel;

        public Action OnResume { get; set ; }
        public Action OnExit { get ; set; }
        public Action OnToMain { get; set ; }

        public Canvas PauseMenuCanvas => pauseMenuCanvas;

        public Image PauseMenuPanel => pauseMenuPanel;

        public void Hide()
        {
            this.pauseMenuCanvas.enabled = false;
            this.pauseMenuCanvas.gameObject.SetActive(false);
        }

        public void Show()
        {
            this.pauseMenuCanvas.enabled = true;
            this.pauseMenuCanvas.gameObject.SetActive(true);
        }
        private void Awake()
        {
            pauseMenuCanvas = this.AddChildComponentIfNotExist<Canvas>("PauseMenuCanvas");
            pauseMenuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
            pauseMenuPanel = pauseMenuCanvas.transform.AddChildComponentIfNotExist<Image>("PauseMenuPanel");
        }
        private void Start()
        {
            if(Application.isPlaying)
            {
                GlobalApplication.UIPauseMenu = this as IPauseMenu;
                this.Hide();
            }
        }
    }
}