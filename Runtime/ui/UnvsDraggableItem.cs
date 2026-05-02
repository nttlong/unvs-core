namespace unvs.ui{
    using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnvsDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image; // Hoặc SpriteRenderer
    [HideInInspector] public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root); // Đưa lên lớp trên cùng để không bị che
        transform.SetAsLastSibling();
        image.raycastTarget = false; // Tắt raycast để khi thả, nó "xuyên" qua được chính nó để chạm vào Slot
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Hoạt động cho cả Mouse và Gamepad (nếu EventSystem đã setup)
        transform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        transform.localPosition = Vector3.zero; // Reset về giữa ô
    }
}
}