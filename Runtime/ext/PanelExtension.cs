using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using unvs.shares;

namespace unvs.ext
{
    public static class PanelExtension
    {
        public static void SetPosition(
        this Image panel,
        float xPixel,
        float yPixel,
        float widthInPixels,
        float heightInPixels)
        {
            var r = panel.GetComponent<RectTransform>();
            r.SetPosition(xPixel, yPixel, widthInPixels, heightInPixels);
        }
        public static void Hide(this UnityEngine.UI.Image panel)
        {
            // Kiểm tra null để tránh lỗi Reference
            if (panel == null) return;

            var cg = panel.AddComponentIfNotExist<CanvasGroup>();
            //if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();

            // 1. Ẩn hoàn toàn về mặt thị giác
            cg.alpha = 0f;

            // 2. Chặn tương tác chuột/touch
            cg.interactable = false;

            // 3. Chặn việc Raycast (không cho click xuyên qua hoặc nhận click)
            cg.blocksRaycasts = false;
        }
        public static void Show(this UnityEngine.UI.Image panel)
        {
            // 1. Kiểm tra an toàn để tránh NullReferenceException
            if (panel == null) return;

            // 2. Lấy hoặc thêm CanvasGroup nếu chưa có
            var cg = panel.AddComponentIfNotExist<CanvasGroup>();
            //var cg = panel.GetComponent<CanvasGroup>();
            //if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();

            // 3. Hiện thị hoàn toàn về mặt thị giác
            cg.alpha = 1f;

            // 4. Bật lại khả năng tương tác chuột/touch
            cg.interactable = true;

            // 5. Cho phép Raycast bắt được click trên panel này
            cg.blocksRaycasts = true;
        }
        public static void DockTop(this UnityEngine.UI.Image panel, float heightInPx)
        {

            var y = Commons.GetScreenSize().y - heightInPx;
            var w = Commons.GetScreenSize().x;
            panel.SetPosition(0f, y, w, heightInPx);
            var rect = panel.rectTransform;
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(w, heightInPx);
            rect.anchoredPosition = Vector2.zero;
            //panel.Dock(DockDirection.Top, heightInPx);
        }
        public static void DockBottom(this UnityEngine.UI.Image panel, float heightInPx)
        {

            var y = Commons.GetScreenSize().y-heightInPx;
            var w = Commons.GetScreenSize().x;
            panel.SetPosition(0f, 100, heightInPx, heightInPx);
            var rect = panel.rectTransform;
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(w, heightInPx);
            rect.anchoredPosition = Vector2.zero;
           
        }
        public static void DockLeft(this UnityEngine.UI.Image panel, float widthInPx)
        {
            //// Anchor dính lề trái, trải dài từ trên xuống dưới
            //rect.anchorMin = new Vector2(0, 0);
            //rect.anchorMax = new Vector2(0, 1);
            //rect.pivot = new Vector2(0, 0.5f);
            //rect.sizeDelta = new Vector2(heightOrWidthInPx, 0);
            //rect.anchoredPosition = Vector2.zero; // Đưa sát về lề
            var y = Commons.GetScreenSize().y ;
            var w = widthInPx;
            panel.SetPosition(0f, y, w, y);
            var rect = panel.rectTransform;
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 0.5f);
            rect.sizeDelta = new Vector2(widthInPx, 0);
            rect.anchoredPosition = Vector2.zero;
            //panel.Dock(DockDirection.Top, heightInPx);
        }
        public static void DockRight(this UnityEngine.UI.Image panel, float widthInPx)
        {
            // Anchor dính lề phải, trải dài từ trên xuống dưới
            //rect.anchorMin = new Vector2(1, 0);
            //rect.anchorMax = new Vector2(1, 1);
            //rect.pivot = new Vector2(1, 0.5f);
            //rect.sizeDelta = new Vector2(heightOrWidthInPx, 0);
            //rect.anchoredPosition = Vector2.zero;
            var y = Commons.GetScreenSize().y;
            var w = widthInPx;
            panel.SetPosition(0f, y, w, y);
            var rect = panel.rectTransform;
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 0.5f);
            rect.sizeDelta = new Vector2(widthInPx, 0);
            rect.anchoredPosition = Vector2.zero;
            //panel.Dock(DockDirection.Top, heightInPx);
        }
        public static void DockFull(this UnityEngine.UI.Image panel)
        {
            
            if (panel == null) return;

            // Lấy RectTransform của image
            RectTransform rect = panel.rectTransform;

            // Thiết lập Anchor về 4 góc (Min: 0,0 | Max: 1,1)
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);

            // Reset các giá trị offset về 0 để lấp đầy hoàn toàn
            rect.offsetMin = Vector2.zero; // Tương đương Left, Bottom
            rect.offsetMax = Vector2.zero; // Tương đương Right, Top

            // Đảm bảo vị trí trung tâm
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }
        public static void Dock(this UnityEngine.UI.Image panel, DockDirection direction, float size)
        {
            var rect = panel.rectTransform; // Cách viết tắt của GetComponent<RectTransform>()

            switch (direction)
            {
                case DockDirection.Left:

                    panel.DockLeft(size);
                    break;

                case DockDirection.Right:
                    panel.DockRight(size);
                    break;

                case DockDirection.Top:
                    panel.DockTop(size);
                    break;

                case DockDirection.Bottom:
                    panel.DockBottom(size);
                    break;
                case DockDirection.Full:
                    panel.DockFull();
                    break;
            }
        }

        /// <summary>
        /// Fades the screen from Transparent to Black (Alpha: 0 -> 1)
        /// </summary>
        public static async UniTask FadeInAsync(this UnityEngine.UI.Image panel, CanvasGroup canvasGroup, float fadingTime = 0.5f)
        {
            if (canvasGroup) return;

            panel.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true; // Block UI interactions early

            float elapsedTime = 0;
            while (elapsedTime < fadingTime)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadingTime);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            canvasGroup.alpha = 1f;
        }
        public static async UniTask FadeInAsync(this UnityEngine.UI.Image panel, float fadingTime = 0.5f)
        {
            var canvasGroup = panel.AddComponentIfNotExist<CanvasGroup>();
            await panel.FadeInAsync(canvasGroup, fadingTime);
        }
        /// <summary>
        /// Fades the screen from Black to Transparent (Alpha: 1 -> 0)
        /// </summary>
        public static async UniTask FadeOutAsync(this UnityEngine.UI.Image panel, CanvasGroup canvasGroup, float fadingTime = 0.5f)
        {
            if (canvasGroup == null) return;

            float elapsedTime = 0;
            while (elapsedTime < fadingTime)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadingTime));
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false; // Allow UI interactions
            panel.gameObject.SetActive(false);
        }
        public static async UniTask FadeOutAsync(this UnityEngine.UI.Image panel, float fadingTime = 0.5f)
        {
            var canvasGroup = panel.AddComponentIfNotExist<CanvasGroup>();

            await panel.FadeOutAsync(canvasGroup, fadingTime);
        }

        public static Vector2 GetSize(this Image container)
        {
            if (container == null) return Vector2.zero;

            // Image luôn đi kèm với RectTransform trên UI
            RectTransform rectTransform = container.rectTransform;

            // Trả về size (x là width, y là height)
            return rectTransform.rect.size;
        }
    }
}