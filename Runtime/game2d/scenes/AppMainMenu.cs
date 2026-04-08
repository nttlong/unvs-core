using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using unvs.ext;

namespace unvs.game2d.scenes
{
    public class AppMainMenu : AppUIElemen<AppMainMenu>
    {
        public Canvas canvas;
        public Image panel;
        public Button btnStart;
        public Button btnExit;

       
        

        

        public override void InitRunTime()
        {
            
            this.canvas.FullSize();
            this.panel.DockFull();
            GameObjectExtension.ApplyNavigate<Button>(this.gameObject);
        }
    }
}