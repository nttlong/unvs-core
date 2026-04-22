//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using unvs.ext;
//
//using unvs.shares;
//namespace unvs.ui
//{
//    [ExecuteInEditMode]
//    public class SettingsUIPauseMenu : MonoBehaviour, IPauseMenu
//    {
//        public Canvas pauseMenuCanvas;
//        public Image pauseMenuPanel;
       

//        public Action OnResume { get; set ; }
//        public Action OnExit { get ; set; }
//        public Action OnToMain { get; set ; }

//        public Canvas PauseMenuCanvas => pauseMenuCanvas;

//        public Image PauseMenuPanel => pauseMenuPanel;

       

       

//        public void Hide()
//        {
            
            
//            this.pauseMenuCanvas.DoDeactive();
//        }
//        private void Reset()
//        {
//            pauseMenuCanvas = this.AddChildComponentIfNotExist<Canvas>("PauseMenuCanvas");
//            pauseMenuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
//            pauseMenuPanel = pauseMenuCanvas.transform.AddChildComponentIfNotExist<Image>("PauseMenuPanel");
            
//            var hr= pauseMenuPanel.AddComponentIfNotExist<VerticalLayoutGroup>();
//            LayoutGroupExt.FixLayoutChildren(hr);
           
//            btnResume = pauseMenuPanel.transform.AddButtonIfNotExist("btnResume", "Resume");
//            btnToTitleMenu = pauseMenuPanel.transform.AddButtonIfNotExist("btnToTitleMenu", "To title menu");
//            btnExit = pauseMenuPanel.transform.AddButtonIfNotExist("btnExit", "Exit");
//        }
//        public void Show()
//        {
//            if (GlobalApplication.UIMainMenu.IsShowing) return;
//            this.pauseMenuCanvas.DoActive();
//            EventSystem.current.SetSelectedGameObject(null);

//            var firstSelectItem= this.GetComponentInChildren<Button>();
//            if(firstSelectItem != null)
//            {
//                EventSystem.current.SetSelectedGameObject(firstSelectItem.gameObject);
//                _lastSelectedButton= firstSelectItem.gameObject;
//            }


//        }

       
//        public void Toggle()
//        { 

//            if (this.pauseMenuCanvas.enabled)
//            {
//                Hide();
//            } else
//            {
//                Show();
//            }
//        }

//        private void Awake()
//        {
//            if (!Application.isPlaying) return;
//            pauseMenuCanvas = this.AddChildComponentIfNotExist<Canvas>("PauseMenuCanvas");
//            pauseMenuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
//            pauseMenuPanel = pauseMenuCanvas.transform.AddChildComponentIfNotExist<Image>("PauseMenuPanel");
//            pauseMenuCanvas.FullSize();
//            pauseMenuPanel.DockFull();
//            btnExit = this.GetComponentInChildrenByName<Button>("btnExit");
//            btnExit.onClick.AddListener(() =>
//            {
//                this.OnExit?.Invoke();
//            });
//            btnResume = this.GetComponentInChildrenByName<Button>("btnResume");
//            btnResume.onClick.AddListener(() =>
//            {
//                this.OnResume?.Invoke();
//            });
//            btnToTitleMenu = this.GetComponentInChildrenByName<Button>("btnToTitleMenu");
//            btnToTitleMenu.onClick.AddListener(() =>
//            {
//                this.OnToMain?.Invoke();
//            });
//        }
//        private void Start()
//        {
//            if(Application.isPlaying)
//            {
                
//                GlobalApplication.UIPauseMenu = this as IPauseMenu;
//                this.Hide();
//            }
//        }
//        public void DoResum()
//        {
//            this.OnResume?.Invoke();
//        }
//        public void DoExitToMain()
//        {
//            this.OnToMain?.Invoke();
//        }
//        public void DoExit()
//        {
//            this.OnExit?.Invoke();
//        }
//        private GameObject _lastSelectedButton;
//        private Button btnResume;
//        private Button btnToTitleMenu;
//        private Button btnExit;

//        void Update()
//        {
//            if (pauseMenuCanvas.enabled)
//            {
//                // 1. Nếu có một Object đang được chọn, hãy lưu nó lại
//                if (EventSystem.current.currentSelectedGameObject != null &&
//                    EventSystem.current.currentSelectedGameObject != _lastSelectedButton)
//                {
//                    _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
//                }

//                // 2. Nếu bỗng nhiên mất Focus (do click trượt hoặc lỗi logic)
//                if (EventSystem.current.currentSelectedGameObject == null && _lastSelectedButton != null)
//                {
//                    // Trả Focus về nút cũ để người dùng Gamepad có thể tiếp tục chơi
//                    EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
//                }
//            }
//        }
//    }
//}