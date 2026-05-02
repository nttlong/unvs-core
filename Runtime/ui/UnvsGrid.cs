using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using unvs.ext;

namespace unvs.ui
{
    public class UnvsGrid : MonoBehaviour
    {
        public int columns = 5;
        public Vector2 cellSize = new Vector2(100, 100);
        public Vector2 spacing = new Vector2(10, 10);
        public string prefxSlotName = "UnvsSlot_";
        public Transform container;
        public int currentIndex=0;
        private void Awake()
        {
            if (Application.isPlaying)
            {
                foreach (Transform t in container.transform)
                {
                    var sprite= t.GetFirstComponent<Sprite>(true);
                    this.AddSpriteToGrid(sprite, currentIndex, t.name);
                    currentIndex++;
                }
            }
        }
        public void AddSpriteToGrid(Sprite sprite, int index,string itemName)
        {
            // 1. Tạo Slot
            GameObject slotObj = new GameObject($"{prefxSlotName}_{index}", typeof(RectTransform));
            slotObj.transform.SetParent(this.transform, false);

            // Setup RectTransform cho Slot (như cũ)
            RectTransform slotRect = slotObj.GetComponent<RectTransform>();
            slotRect.anchorMin = new Vector2(0, 1);
            slotRect.anchorMax = new Vector2(0, 1);
            slotRect.pivot = new Vector2(0, 1);

            int row = index / columns;
            int col = index % columns;
            slotRect.anchoredPosition = new Vector2(col * (cellSize.x + spacing.x), -row * (cellSize.y + spacing.y));
            slotRect.sizeDelta = cellSize;

            // 2. Tạo Item (Dùng Image thay vì SpriteRenderer để hiện lên Panel)
            GameObject itemObj = new GameObject(itemName, typeof(RectTransform), typeof(UnityEngine.UI.Image));
            itemObj.transform.SetParent(slotObj.transform, false);

            // SỬA LỖI TẠI ĐÂY:
            UnityEngine.UI.Image img = itemObj.GetComponent<UnityEngine.UI.Image>();
            img.sprite = sprite;
            // Nếu img.raycastTarget báo lỗi, dùng ép kiểu Graphic:
            ((Graphic)img).raycastTarget = true;

            RectTransform itemRect = itemObj.GetComponent<RectTransform>();
            itemRect.sizeDelta = cellSize;
            itemRect.anchoredPosition = Vector2.zero;

            // 3. Thêm Drag & Drop
            itemObj.AddComponent<UnvsDraggableItem>();
        }
        private void OnValidate()
        {
            this.container = this.AddChildComponentIfNotExist<Transform>("container");
            this.container.gameObject.SetActive(false);

        }
    }
}