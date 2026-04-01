using UnityEngine;
using UnityEngine.InputSystem;
using unvs.shares;
namespace unvs.ui {
public class DraggableItem : MonoBehaviour
{
    private Camera _mainCamera;
    private bool _isDragging = false;
    private Vector3 _offset;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        // Đăng ký sự kiện từ hệ thống GlobalInput của bạn
        var ui = GlobalApplication.GlobalInput.UI;
        ui.Click.started += OnClickStarted;
        ui.Click.canceled += OnClickCanceled;
    }

    private void OnDisable()
    {
        var ui = GlobalApplication.GlobalInput.UI;
        ui.Click.started -= OnClickStarted;
        ui.Click.canceled -= OnClickCanceled;
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        Vector2 mousePos = GlobalApplication.GlobalInput.UI.ScrollWheel.ReadValue<Vector2>(); 
        // Lưu ý: Thay ScrollWheel bằng Action "Point" hoặc dùng Mouse.current.position.ReadValue()
        
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _mainCamera.nearClipPlane));

        // Kiểm tra xem có click trúng Object này không (Dùng Raycast 2D hoặc 3D)
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            _isDragging = true;
            _offset = transform.position - worldPos;
        }
    }

    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        _isDragging = false;
    }

    private void Update()
    {
        if (_isDragging)
        {
            // Lấy vị trí chuột hiện tại
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f)); // 10f là khoảng cách từ cam
            
            // Cập nhật vị trí vật thể
            transform.position = new Vector3(worldPos.x + _offset.x, worldPos.y + _offset.y, transform.position.z);
        }
    }
}
}