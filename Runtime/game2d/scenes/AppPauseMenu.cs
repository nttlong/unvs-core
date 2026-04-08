using System;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;

namespace unvs.game2d.scenes
{
    public class AppPauseMenu : AppUIElemen<AppPauseMenu>
    {
        public Canvas canvas;
        public Image panel;
        public Button btnResume;
        public Button btnMainMenu;
        public Button btnExit;

        
        

        public override void InitRunTime()
        {
            this.canvas.FullSize();
        }
    }
}