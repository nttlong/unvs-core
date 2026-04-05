
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Bắt buộc để dùng TextMeshPro
namespace unvs.ui
{
    public class UIHandler : MonoBehaviour
{
    public Canvas targetCanvas; // Kéo Canvas vào đây

    void Start()
    {
        CreateCustomButton();
    }

    void CreateCustomButton()
    {
        // 1. Tạo GameObject cho Button
        GameObject buttonObj = new GameObject("MyDynamicButton");
        buttonObj.transform.SetParent(targetCanvas.transform, false);

        // 2. Thêm các Component cần thiết
        Image btnImage = buttonObj.AddComponent<Image>();
        Button btn = buttonObj.AddComponent<Button>();
        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();

        // Thiết lập kích thước nút
        btnRect.sizeDelta = new Vector2(200, 50);
        btnRect.anchoredPosition = Vector3.zero; // Nằm giữa Canvas

        // 3. Tạo GameObject con cho Text
        GameObject textObj = new GameObject("ButtonText");
        textObj.transform.SetParent(buttonObj.transform, false);

        // 4. Thêm TextMeshProUGUI
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = "CLICK ME";
        txt.fontSize = 24;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;

        // Căn chỉnh Text tràn đầy diện tích nút
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        // 5. Gán sự kiện Click bằng code
        btn.onClick.AddListener(() => {
            Debug.Log("Button được nhấn bằng code!");
            OnButtonClicked();
        });
    }

    void OnButtonClicked()
    {
        // Logic xử lý khi nhấn nút
    }
}
}