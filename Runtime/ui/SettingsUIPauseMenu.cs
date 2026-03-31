using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public GameObject firstSelectItem;

        public Action OnResume { get; set ; }
        public Action OnExit { get ; set; }
        public Action OnToMain { get; set ; }

        public Canvas PauseMenuCanvas => pauseMenuCanvas;

        public Image PauseMenuPanel => pauseMenuPanel;

       

        public GameObject FirstSelectItem => firstSelectItem;

        public void Hide()
        {
            Time.timeScale = 1.0f;
            this.pauseMenuCanvas.enabled = false;
            this.pauseMenuCanvas.gameObject.SetActive(false);
            //GlobalApplication.UIEventControllerInstance.PauseStarted -= UIEventControllerInstance_PauseStarted;
        }

        public void Show()
        {
            Time.timeScale =0f;
            this.pauseMenuCanvas.enabled = true;
            this.pauseMenuCanvas.gameObject.SetActive(true);
            //GlobalApplication.UIEventControllerInstance.PauseStarted += UIEventControllerInstance_PauseStarted;
        }

        private void UIEventControllerInstance_PauseStarted()
        {
            this.Toggle();
        }

        public void Toggle()
        {
            if (this.pauseMenuCanvas.enabled)
            {
                Hide();
            } else
            {
                Show();
            }
        }

        private void Awake()
        {
            if(Application.isPlaying)
            {
                if (firstSelectItem == null)
                {
                    throw new Exception("Please set value of firstSelectItem");
                }
            }
            pauseMenuCanvas = this.AddChildComponentIfNotExist<Canvas>("PauseMenuCanvas");
            pauseMenuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
            pauseMenuPanel = pauseMenuCanvas.transform.AddChildComponentIfNotExist<Image>("PauseMenuPanel");
        }
        private void Start()
        {
            if(Application.isPlaying)
            {
                this.pauseMenuCanvas.FullSize();
                GlobalApplication.UIPauseMenu = this as IPauseMenu;
                this.Hide();
            }
        }
        void OnEnable()
        {
            // Xóa lựa chọn cũ (nếu có)
            EventSystem.current.SetSelectedGameObject(null);

            // Đặt lựa chọn mới vào nút mặc định
            EventSystem.current.SetSelectedGameObject(firstSelectItem);
        }
    }
}