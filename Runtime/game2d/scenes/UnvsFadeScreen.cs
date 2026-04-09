using System;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.scenes;

namespace game2d.scenes {
    public class UnvsFadeScreen : UnvsUIComponent
    {
        public Canvas canvas;
        public Image panel;

        public override void InitEvents()
        {
            
        }

        public override void InitRunTime()
        {
            this.canvas.FullSize();
        }
    }
}