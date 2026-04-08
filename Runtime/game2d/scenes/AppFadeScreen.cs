using System;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.scenes;

namespace game2d.scenes {
    public class AppFadeScreen : AppUIElemen<AppFadeScreen>
    {
        public Canvas canvas;
        public Image panel;
        public override void InitRunTime()
        {
            this.canvas.FullSize();
        }
    }
}