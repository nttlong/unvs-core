//using Cysharp.Threading.Tasks.Triggers;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using unvs.ext;
//using unvs.gameword;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.ui
//{
//    [ExecuteInEditMode]
//    public class SettingsUIMainMenu : MonoBehaviour, IMainMenu
//    {
//        public Canvas menuCanvas;
//        private Image panel;

//        public Action OnStartClick { get; set ; }
//        public Action OnExitClick { get ; set; }

//        public Canvas MenuCanvas => menuCanvas;

//        public Image Panel => panel;

//        public bool IsShowing { get; internal set; }

//        public Button BtnStart => btnStart;

//        public Button BtnExit => btnExit;

//        public void DoExitGame()
//        {
//            OnExitClick?.Invoke();
            
//        }

//        public void DoStartGame()
//        {
//            OnStartClick?.Invoke();
//        }
       
//        public void Hide()
//        {
//            this.IsShowing = false;
//            this.menuCanvas.DoDeactive();
           
//        }
//        private void Reset()
//        {
//            menuCanvas = this.AddChildComponentIfNotExist<Canvas>("CanvasMain");
//            menuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
//            panel = menuCanvas.transform.AddChildComponentIfNotExist<Image>("PanelMain");
            
//            panel.AddComponentIfNotExist<VerticalLayoutGroup>().FixLayoutChildren();
//            panel.DockFull();
//            btnStart = panel.transform.AddButtonIfNotExist("btnStart", "Start");
//            btnExit = panel.transform.AddButtonIfNotExist("btnExit", "Exit");
//        }
//        public void Show()
//        {
//            //this.menuCanvas.enabled = true;
//            this.menuCanvas.DoActive();
//            var FirstButton=this.GetComponentInChildren<Button>();
//            if(FirstButton != null )
//            {
//                EventSystem.current.SetSelectedGameObject(null);
//                EventSystem.current.SetSelectedGameObject(FirstButton.gameObject);
//            }
//            this.IsShowing = true;
//        }
//        private void Awake()
//        {
//            if (!Application.isPlaying) return;

//           menuCanvas= this.AddChildComponentIfNotExist<Canvas>("CanvasMain");
//            menuCanvas.FullSize();
//            menuCanvas.AddComponentIfNotExist<GraphicRaycaster>();
//            panel = menuCanvas.transform.AddChildComponentIfNotExist<Image>("PanelMain");
//            panel.DockFull();
//            GlobalApplication.UIMainMenu = this;
//            btnStart = this.GetComponentInChildrenByName<Button>("btnStart");
//            btnExit = this.GetComponentInChildrenByName<Button>("btnExit");
//            btnStart.onClick.AddListener(() =>
//            {
//                this.OnStartClick?.Invoke();
//            });
//            btnExit.onClick.AddListener(() =>
//            {
//                OnStartClick?.Invoke();
//            });
//        }
//        private GameObject _lastSelectedButton;
//        public Button btnStart;
//        public Button btnExit;

//        void Update()
//        {
//            if (!Application.isPlaying) return;
//            if (menuCanvas.enabled)
//            {
               
//                if (EventSystem.current.currentSelectedGameObject != null &&
//                    EventSystem.current.currentSelectedGameObject != _lastSelectedButton)
//                {
//                    _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
//                }

               
//                if (EventSystem.current.currentSelectedGameObject == null && _lastSelectedButton != null)
//                {
                   
//                    EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
//                }
//            }
//        }
//    }
//}