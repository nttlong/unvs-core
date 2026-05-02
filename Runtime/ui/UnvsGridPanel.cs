using System.Collections.Generic;
using UnityEngine;

namespace unvs.ui
{
    [ExecuteInEditMode]
    public class UnvsInventoryGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        public int columns = 5;
        public Vector2 cellSize = new Vector2(100, 100);
        public Vector2 spacing = new Vector2(10, 10);

        // Lưu trữ danh sách các Object đã tạo để quản lý
        private List<GameObject> createdSlots = new List<GameObject>();

        /// <summary>
        /// Hàm chia ô dùng chung cho mọi loại Component (Image, SpriteRenderer, v.v.)
        /// </summary>
        public void BuildGrid<T>(int totalItems, System.Action<T, int> onInitialize) where T : Component
        {
            // 1. Dọn dẹp sạch sẽ trước khi tạo mới
            ClearGrid();

            for (int i = 0; i < totalItems; i++)
            {
                // 2. Tính toán vị trí dựa trên Index
                int row = i / columns;
                int col = i % columns;

                float x = col * (cellSize.x + spacing.x);
                float y = -row * (cellSize.y + spacing.y); // Đi xuống nên y âm

                // 3. Tạo GameObject mới hoàn toàn (Không cần Prefab)
                GameObject go = new GameObject($"Slot_{i}", typeof(RectTransform));
                go.transform.SetParent(this.transform, false);

                RectTransform rect = go.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(x, y);
                rect.sizeDelta = cellSize;

                // 4. Thêm Component yêu cầu và gọi Callback để bên ngoài tự xử lý logic
                T component = go.AddComponent<T>();
                createdSlots.Add(go);

                onInitialize?.Invoke(component, i);
            }
        }

        public void ClearGrid()
        {
            foreach (var slot in createdSlots)
            {
                if (slot != null) DestroyImmediate(slot);
            }
            createdSlots.Clear();
        }
    }
}