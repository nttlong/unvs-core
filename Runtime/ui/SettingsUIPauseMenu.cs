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
       

        public Action OnResume { get; set ; }
        public Action OnExit { get ; set; }
        public Action OnToMain { get; set ; }

        public Canvas PauseMenuCanvas => pauseMenuCanvas;

        public Image PauseMenuPanel => pauseMenuPanel;

       

       

        public void Hide()
        {
            
            
            this.pauseMenuCanvas.DoDeactive();
        }

        public void Show()
        {
            if (GlobalApplication.UIMainMenu.IsShowing) return;
            this.pauseMenuCanvas.DoActive();
            EventSystem.current.SetSelectedGameObject(null);

            var firstSelectItem= this.GetComponentInChildren<Button>();
            if(firstSelectItem != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectItem.gameObject);
                _lastSelectedButton= firstSelectItem.gameObject;
            }


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
            
            pauseMenuCanvas = this.AddChildComponentIfNotExist<Canvas>("PauseMenuCanvas");
            pauseMenuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
            pauseMenuPanel = pauseMenuCanvas.transform.AddChildComponentIfNotExist<Image>("PauseMenuPanel");
            pauseMenuCanvas.FullSize();
            pauseMenuPanel.DockFull();
        }
        private void Start()
        {
            if(Application.isPlaying)
            {
                
                GlobalApplication.UIPauseMenu = this as IPauseMenu;
                this.Hide();
            }
        }
        public void DoResum()
        {
            this.OnResume?.Invoke();
        }
        public void DoExitToMain()
        {
            this.OnToMain?.Invoke();
        }
        public void DoExit()
        {
            this.OnExit?.Invoke();
        }
        private GameObject _lastSelectedButton;

        void Update()
        {
            if (pauseMenuCanvas.enabled)
            {
                // 1. Nếu có một Object đang được chọn, hãy lưu nó lại
                if (EventSystem.current.currentSelectedGameObject != null &&
                    EventSystem.current.currentSelectedGameObject != _lastSelectedButton)
                {
                    _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
                }

                // 2. Nếu bỗng nhiên mất Focus (do click trượt hoặc lỗi logic)
                if (EventSystem.current.currentSelectedGameObject == null && _lastSelectedButton != null)
                {
                    // Trả Focus về nút cũ để người dùng Gamepad có thể tiếp tục chơi
                    EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
                }
            }
        }
    }
}