using game2d.ext;
using game2d.scenes;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;

namespace unvs.game2d.scenes
{
    public class UnvsPauseMenu : UnvsUIComponentInstance<UnvsPauseMenu>
    {
        public Canvas canvas;
        public Image panel;
        public Button btnResume;
        public Button btnMainMenu;
        public Button btnExit;
       
        public Action OnMainMenu;
        public Action OnExit;
        public override void InitEvents()
        {
            this.btnExit.onClick.AddListener(() =>
            {
                OnExit?.Invoke();
            });
            this.btnMainMenu.onClick.AddListener(() => { OnMainMenu?.Invoke(); });
            this.btnResume.onClick.AddListener(() => {
                base.Hide();
            });
        }

        public override void InitRunTime()
        {
            this.canvas.FullSize();
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