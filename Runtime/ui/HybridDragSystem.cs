
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.controllers_input;
using unvs.game2d.scenes;
using unvs.shares;
namespace unvs.ui
{
    // Change from struct to class to ensure reference consistency
    public class DragDropInfo
    {
        public GameObject Source;
        public GameObject Target;
        public Vector2 Pos;
        public GameObject DraggingVisual { get; internal set; }

        public void Empty()
        {
            Source = null;
            Target = null;
            Pos = Vector2.zero;
            if (DraggingVisual != null) UnityEngine.Object.Destroy(DraggingVisual);
        }

        public T GetSource<T>() where T : class
        {
            return Source?.GetComponent<T>();
        }

        public void CloneObject(Canvas canvas, Sprite icon, string name, float iconSize)
        {
            // Use a descriptive name for debugging
            GameObject dragIconObj = new GameObject($"{name}");

            // Ensure it's treated as a UI element by adding RectTransform first
            RectTransform rect = dragIconObj.AddComponent<RectTransform>();

            // Set parent before configuring UI properties
            dragIconObj.transform.SetParent(canvas.transform, false);

            var image = dragIconObj.AddComponent<UnityEngine.UI.Image>();
            image.sprite = icon;
            image.raycastTarget = false; // Prevent blocking raycasts for drop detection

            rect.sizeDelta = new Vector2(iconSize, iconSize);

            // Use position (World Space) for hybrid pointer logic
            rect.position = Pos;

            // Move to the front of the UI hierarchy
            rect.SetAsLastSibling();

            DraggingVisual = dragIconObj;
        }

        public void Move()
        {
            if (DraggingVisual == null) return;

            // Update position every frame during drag
            DraggingVisual.transform.position = Pos;
        }
    }
    public class HybridDragSystem : MonoBehaviour
    {
        [SerializeField] private RectTransform interactContainer;
        private RectTransform _draggingItem;
        private Vector2 _pointerPosition;
        private Vector2 _dragOffset;
        public Action<DragDropInfo> OnDragBegin;
        public Action<DragDropInfo> OnDragging;
        public Action<DragDropInfo> OnDrop;
        private DragDropInfo dragInfo=new DragDropInfo();

        private void Start()
        {
            interactContainer = GetComponent<RectTransform>();
            // Khởi tạo vị trí trỏ ở giữa màn hình
            _pointerPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        }

        private void Update()
        {
            if(UnvsGlobalInput.UI==null|| UnvsGlobalInput.UI.Count==0) return;
            var ui = UnvsGlobalInput.UI;

            // 1. CẬP NHẬT TỌA ĐỘ (HYBRID LOGIC)
            if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().magnitude > 0.1f)
            {
                // Nếu dùng Gamepad: Cộng dồn delta
                Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
                _pointerPosition += stickInput * Time.unscaledDeltaTime * 800f; // 800 là tốc độ trỏ
            }
            else if (Mouse.current != null && Mouse.current.delta.ReadValue().magnitude > 0.1f)
            {
                // Nếu dùng Mouse: Lấy tọa độ trực tiếp
                _pointerPosition = Mouse.current.position.ReadValue();
            }

            // Giới hạn trong màn hình
            _pointerPosition.x = Mathf.Clamp(_pointerPosition.x, 0, Screen.width);
            _pointerPosition.y = Mathf.Clamp(_pointerPosition.y, 0, Screen.height);

            // 2. XỬ LÝ CLICK (BẤM NÚT)
            // Click.started (Gamepad Button South hoặc Mouse Left Click)
            if (ui["Click"].WasPressedThisFrame())
            {
                _draggingItem = FindItemUnderPointer(_pointerPosition);
                if (_draggingItem != null)
                {
                    dragInfo = new DragDropInfo();
                    dragInfo.Source = _draggingItem.gameObject;
                    dragInfo.Pos = _pointerPosition;
                    // Tính Offset để tâm vật thể không bị nhảy về tâm trỏ
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _draggingItem, _pointerPosition, null, out _dragOffset);
                    UnvsGlobalInput.SetActivePlayer(false);
                    OnDragBegin?.Invoke( dragInfo );
                    //_draggingItem.SetAsLastSibling(); // Đưa lên trên cùng
                }
            }

            // 3. XỬ LÝ DRAG (KÉO)
            if (_draggingItem != null && ui["Click"].IsPressed())
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    interactContainer, _pointerPosition, null, out Vector2 localPos);
                dragInfo.Pos = _pointerPosition;
                OnDragging?.Invoke(dragInfo );
                //_draggingItem.anchoredPosition = localPos - _dragOffset;
            }

            // 4. XỬ LÝ DROP (THẢ)
            if (ui["Click"].WasReleasedThisFrame())
            {
                if (_draggingItem != null)
                {
                   
                    dragInfo.Pos = _pointerPosition;
                    OnDrop?.Invoke(dragInfo);
                    _draggingItem = null;
                    dragInfo.Empty();
                    UnvsGlobalInput.SetActivePlayer(true);
                }
            }
        }

        private RectTransform FindItemUnderPointer(Vector2 screenPos)
        {
            // Quét tất cả các icon trong interactContainer
            foreach (RectTransform child in interactContainer)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(child, screenPos))
                    return child;
            }
            return null;
        }

        
    }
}