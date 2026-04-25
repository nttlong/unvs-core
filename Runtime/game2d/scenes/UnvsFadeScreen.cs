using Cysharp.Threading.Tasks;
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
        public static UnvsFadeScreen Instance;
        public Image panel;

        public override bool DisablePlayerInput => false;

        public override bool EnablePlayerInput => false;

        public override void InitEvents()
        {
            
        }
        public async UniTask FadeInAsync(float durationTime = 1f)
        {
            if (durationTime == 0f) return;
            panel.transform.position = new Vector3(panel.transform.position.x, panel.transform.position.y, 0);
            panel.enabled = true;
            panel.gameObject.SetActive(true);
            await panel.FadeInAsync(durationTime);
        }
        public async UniTask FadeOutAsync(float durationTime = 1f)
        {
            if (durationTime == 0f) return;
            panel.transform.position = new Vector3(panel.transform.position.x, panel.transform.position.y, 0);
            panel.enabled = true;
            panel.gameObject.SetActive(true);
            await panel.FadeOutAsync(durationTime);
        }
        public override void InitRunTime()
        {
            this.canvas.FullSize();
            this.panel.DockFull();
            this.panel.FadeOutAsync(0.1f).Forget();
            Instance = this;
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