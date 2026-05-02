namespace unvs.ui
{

    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UnvsGridSlot : MonoBehaviour, IDropHandler
    {
        public SpriteRenderer iconRenderer; // Hoặc Image nếu dùng UI
        public string slotID;

        public void OnDrop(PointerEventData eventData)
        {
            // Logic khi thả một item vào ô này
            GameObject dropped = eventData.pointerDrag;
            UnvsDraggableItem item = dropped.GetComponent<UnvsDraggableItem>();
            if (item != null)
            {
                item.parentAfterDrag = transform; // Chuyển "hộ khẩu" về ô mới
            }
        }
    }
}