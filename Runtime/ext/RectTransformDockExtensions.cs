using UnityEngine;
using UnityEngine.UI;
namespace unvs.ext
{
    public static class RectTransformDockExtensions
    {



        /// <summary>
        /// Lấy chiều cao hiện tại của RectTransform theo pixel (đã tính scale).
        /// </summary>
        /// <param name="rect">RectTransform cần lấy</param>
        /// <returns>Chiều cao theo pixel</returns>
        public static float GetHeightInPixels(this RectTransform rect)
        {
            if (rect == null) return 0f;

            Canvas canvas = rect.GetComponentInParent<Canvas>();
            float scaleFactor = (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
                ? canvas.scaleFactor
                : 1f;

            return rect.rect.height * scaleFactor;
        }




        /// <summary>
        /// Dock một RectTransform vào đáy Canvas (hoặc parent RectTransform).
        /// Panel sẽ dính sát đáy, phần thân hướng lên trên, không bị tụt xuống dưới.
        /// </summary>
        /// <param name="panel">RectTransform của panel cần dock (thường là GetComponent<RectTransform>())</param>
        /// <param name="parent">RectTransform của Canvas hoặc container cha</param>
        /// <param name="heightInPixels">Chiều cao mong muốn theo pixel (ví dụ Screen.height / 6). Nếu <=0 thì giữ height hiện tại.</param>
        /// <param name="offset">Khoảng cách từ đáy màn hình lên (padding, thường 0 hoặc dương nhỏ)</param>
        /// <param name="respectSafeArea">Có điều chỉnh theo safe area bottom inset (notch/status bar) không? Default true cho mobile.</param>

        public static void SetPosition(
        this RectTransform panel,
        float xPixel,
        float yPixel,
        float widthInPixels,
        float heightInPixels,
        Vector2? pivot = null)
        {
            //SetWidthInPixels(panel, widthInPixels);

            //SetHeightInPixels(panel, heightInPixels, true);
            if (panel == null) return;

            // Lấy scale factor từ CanvasScaler
            Canvas canvas = panel.GetComponentInParent<Canvas>();
            float scaleFactor = (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
                ? canvas.scaleFactor
                : 1f;

            // Chuyển pixel thành anchored unit
            float anchoredX = xPixel / scaleFactor;
            float anchoredY = yPixel / scaleFactor;

            // Cấu hình anchor & pivot theo gốc dưới trái
            panel.anchorMin = new Vector2(0f, 0f);
            panel.anchorMax = new Vector2(0f, 0f);
            panel.pivot = pivot ?? new Vector2(0f, 0f); // mặc định bottom-left

            // Đặt vị trí
            panel.anchoredPosition = new Vector2(anchoredX, anchoredY);
            panel.sizeDelta = new Vector2(widthInPixels / scaleFactor, heightInPixels / scaleFactor);
            // Force update layout ngay lập tức
            LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
        }

    }
}