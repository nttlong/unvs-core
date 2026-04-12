using game2d.ext;
using game2d.scenes;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using unvs.ext;

namespace unvs.game2d.scenes
{
    public class UnvsPauseMenu : UnvsUIComponentInstance<UnvsPauseMenu>
    {
        
        public Image panel;
        public Button btnResume;
        public Button btnMainMenu;
        public Button btnExit;
       
        public Action OnMainMenu;
        public Action OnExit;
        private InputAction actionPause;
        private bool isShow;

        public override void InitRunTime()
        {
           
            base.InitRunTime();
            this.panel.DockFull();
        }
        public override void Show()
        {
            base.Show();
           
        }
        public virtual void Toggle()
        {
            isShow = !isShow;
            if(isShow ) Show();
            else Hide();
        }
        public override void Hide()
        {
            base.Hide();
            //if(actionPause != null)
            //actionPause.started -= ActionPause_started;
        }
        
        public override void InitEvents()
        {
           
            this.btnExit.onClick.AddListener(() =>
            {
                if (OnExit != null)
                    OnExit.Invoke();
                else
                    UnvsApp.Instance.ExitGame();
            });
            this.btnMainMenu.onClick.AddListener(() => {
                if(OnMainMenu!=null) OnMainMenu.Invoke();
                else
                {
                    UnvsSceneLoader.Instance.ClearUpAll();
                    Hide();
                    UnvsApp.Instance.MainMenu.Show();
                }
            });
            this.btnResume.onClick.AddListener(() => {
                base.Hide();
            });
        }

        
#if UNITY_EDITOR
        [UnvsButton]
        public void GenerateUI()
        {
           var  menu = this;
            menu.canvas = menu.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            menu.panel = menu.canvas.transform.AddChildComponentIfNotExist<UnityEngine.UI.Image>("panel");
            var vlg = menu.panel.AddComponentIfNotExist<VerticalLayoutGroup>();
            menu.btnResume = menu.panel.transform.AddButtonIfNotExist("btnResume", "Resume");
            menu.btnMainMenu = menu.panel.transform.AddButtonIfNotExist("btnMainMenu", "To Title Screen");
            menu.btnExit = menu.panel.transform.AddButtonIfNotExist("btnExit", "Exit");
            vlg.FixLayoutChildren();
            menu.EditorSave();
        }

#endif
    }
}