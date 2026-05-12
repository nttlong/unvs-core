using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using unvs.game2d.scenes;
using unvs.shares;

namespace unvs.ext
{
    public static class CanvasExtension
    {
        public static Canvas[] UICanvasList = new Canvas[0];
        public static void FullSize(this Canvas UICanvas)
        {
            if (UICanvas == null) return;
            var rect = UICanvas.GetComponent<RectTransform>();
            UICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler canvasScaler = UICanvas.AddComponentIfNotExist<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            UICanvas.worldCamera = Camera.main;
            canvasScaler.referenceResolution = Commons.GetScreenSize();
            rect.anchoredPosition = new Vector2(0f, 0f);
            rect.sizeDelta = Commons.GetScreenSize();
            // 3. Các thuộc tính quan trọng khác (nên set luôn để responsive tốt)
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;  // 0 = match width (landscape tốt),
            UICanvas.SetMeOnLayer(Constants.Layers.UI);

        }
        public static void UIFullSize(this Canvas UICanvas, CanvasScaler.ScaleMode mode = CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            if (UICanvas == null) return;
            var rect = UICanvas.GetComponent<RectTransform>();
            
            CanvasScaler canvasScaler = UICanvas.AddComponentIfNotExist<CanvasScaler>();
           
            UICanvas.worldCamera = Camera.main;

            rect.anchoredPosition = new Vector2(0f, 0f);
            UICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (Commons.IsMobile())
            {
                
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 0.5f;  // 0 = match width (landscape tốt),
                rect.sizeDelta = Commons.GetScreenSize();
                canvasScaler.referenceResolution = Commons.GetScreenSize();
            }
            else
            {
                
              
                canvasScaler.uiScaleMode = mode;
                rect.sizeDelta = Commons.GetUIScreenSize();
                canvasScaler.referenceResolution = Commons.GetUIScreenSize();
            }

            UICanvas.SetMeOnLayer(Constants.Layers.UI);

        }
        public static void DoActive(this Canvas UICanvas)
        {
            if (!Application.isPlaying) return;
            if (UICanvas == null) return;
            foreach (var p in UICanvasList)
            {
                if (p != null)
                {
                    if (p != UICanvas)
                    {
                        p.gameObject.SetActive(false);
                    }

                }
            }
            UICanvas.enabled = true;
            UICanvas.gameObject.SetActive(true);
            //GlobalApplication.GlobalInput.Player.enable = false;
            Time.timeScale = 0f;
            //SettingsSingleScene.Instance.CursorOn();

        }
        public static void DoDeactive(this Canvas UICanvas)
        {
            if (UICanvas == null) return;
            UICanvas.enabled = false;
            UICanvas.gameObject.SetActive(false);
            //GlobalApplication.GlobalInput.Player.enable = true;
            Time.timeScale = 1f;
        }
    }
}