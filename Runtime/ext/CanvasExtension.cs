using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using unvs.shares;

namespace unvs.ext
{
    public static class CanvasExtension
    {
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
        }
    }
}