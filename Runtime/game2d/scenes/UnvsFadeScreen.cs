using System;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;

namespace game2d.scenes {
    public class UnvsFadeScreen : UnvsUIComponent
    {
        
        public Image panel;

        public override bool DisablePlayerInput => false;

        public override bool EnablePlayerInput => false;

        public override void InitEvents()
        {
            
        }

        public override void InitRunTime()
        {
            this.canvas.FullSize();
        }
#if UNITY_EDITOR
        [UnvsButton]
        public void Generate()
        {
            this.canvas = this.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            this.panel = this.canvas.transform.AddChildComponentIfNotExist<Image>("Panel");
        }

        
#endif
    }
}