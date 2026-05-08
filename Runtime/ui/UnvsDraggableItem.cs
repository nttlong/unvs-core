namespace unvs.ui
{
    using Cysharp.Threading.Tasks.Triggers;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using unvs.ext;
    using unvs.game2d.scenes;
    using unvs.types;

    [RequireComponent(typeof(CanvasGroup))]
    public class UnvsDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler
    {
        public Image image;
        [HideInInspector] public Transform parentAfterDrag; // Biến này để Slot gán giá trị vào
        private Vector2 _originalSize;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Canvas canvas;
        private Transform _originalParent;
        
        internal UnvsGrid owner;
        

        //[SerializeField]
        //public Vector2 size;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
            if (owner.currentDrageInfo == null)
            {
                owner.currentDrageInfo = new UnvsDragContext();
            }
            owner.currentDrageInfo.Item = this;
            owner.currentDrageInfo.OldSlot = this.transform.parent;
            _originalParent = this.transform.parent;
            if (canvas.GetComponent<HorizontalOrVerticalLayoutGroup>() != null)
                canvas.GetComponent<HorizontalOrVerticalLayoutGroup>().enabled = false;
            parentAfterDrag = transform.parent;

            // Sử dụng true để Unity tự tính toán lại Local Scale/Size sao cho 
            // kích thước thực tế ngoài màn hình không thay đổi.
            transform.SetParent(canvas.transform, true);

            canvasGroup.blocksRaycasts = false;
           
           
        }

       

        public void OnDrag(PointerEventData eventData)
        {
            UnvsApp.Instance.IsBeginDragItem = true;
            // Tương thích cả Mouse và Gamepad Virtual Cursor
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            UnvsApp.Instance.currentActor.SayText("OnDrag");

        }

        public void OnEndDrag(PointerEventData eventData)
        {
           
            // 1. Xác định mục tiêu dưới chuột
            GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
            if (dropTarget == null) dropTarget = eventData.pointerEnter;

            // 2. Chuẩn bị thông tin Drag cho Event
            if (owner.currentDrageInfo != null)
            {
                owner.currentDrageInfo.NewSlotOrDropContainer = dropTarget != null ? dropTarget.transform : null;

                // Mặc định là chưa có logic gì xử lý thay đổi
                owner.currentDrageInfo.hasChange = false;

                // 3. RAISE EVENT: Cho phép các hệ thống khác (Inventory, Crafting, NPC...) can thiệp
                owner.RaiseEventOnDrop(owner.currentDrageInfo);

                // 4. KIỂM TRA PHẢN HỒI (hasChange)
                if (owner.currentDrageInfo.hasChange == false)
                {
                    // Nếu KHÔNG CÓ LOGIC NÀO xử lý (thả trượt hoặc sai quy tắc): Trả về Slot cũ
                    RollbackToOriginalSlot();
                }
               
            }

            // 5. Reset các thuộc tính hình ảnh
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            // Bật lại LayoutGroup nếu có
            var layout = canvas.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (layout != null) layout.enabled = true;
            UnvsApp.Instance.IsBeginDragItem = false;
            UnvsApp.Instance.currentActor.SayText("OnEndDrag");
        }

        private void RollbackToOriginalSlot()
        {
            transform.SetParent(_originalParent, false);
            rectTransform.anchoredPosition = Vector2.zero;
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UnvsApp.Instance.IsBeginDragItem = true;
            UnvsApp.Instance.currentActor.SayText("Start drag");
        }
    }
}