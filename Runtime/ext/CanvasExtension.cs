using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using unvs.gameword;
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
            // 2. Đặt Reference Resolution (1080x1920 cho portrait mobile chuẩn)
            canvasScaler.referenceResolution = Commons.GetScreenSize();
            rect.anchoredPosition = new Vector2(0f, 0f);
            rect.sizeDelta = Commons.GetScreenSize();
            // 3. Các thuộc tính quan trọng khác (nên set luôn để responsive tốt)
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;  // 0 = match width (landscape tốt),
            UICanvas.SetMeOnLayer(Constants.Layers.UI);
            var check = UICanvasList.FirstOrDefault(p => p == UICanvas);
            if (check==null)
            {
                var lst = new List<Canvas>(UICanvasList);
                lst.Add(UICanvas);
                UICanvasList = lst.ToArray();
            }
        }
        public static void DoActive(this Canvas UICanvas)
        {
            if (!Application.isPlaying) return;
            if (UICanvas == null) return;
            foreach(var p  in UICanvasList)
            {
                if(p != null)
                {
                    if(p!= UICanvas)
                    {
                        p.gameObject.SetActive(false);
                    }
                   
                }
            }
            UICanvas.enabled = true;
            UICanvas.gameObject.SetActive(true);
            GlobalApplication.GlobalInput.Player.enable = false;
            Time.timeScale = 0f;
            SingleScene.Instance.CursorOn();

        }
        public static void DoDeactive(this Canvas UICanvas)
        {
            if (UICanvas == null) return;
            UICanvas.enabled = false;
            UICanvas.gameObject.SetActive(false);
            GlobalApplication.GlobalInput.Player.enable = true;
            Time.timeScale = 1f;
        }
    }
}