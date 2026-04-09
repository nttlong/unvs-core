using Cysharp.Threading.Tasks.Triggers;
using game2d.ext;
using game2d.scenes;
using PlasticPipe.PlasticProtocol.Messages;
using System;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using unvs.ext;
using unvs.shares;

namespace unvs.game2d.scenes
{

    public class UnvsInteractUI : UnvsUIComponentInstance<UnvsInteractUI>
    {
        public Texture2D defaultCursorIcon;
        public Image cursor;
        private Vector2 _virtualMousePos;
        private Sprite defautlSprite;
        [SerializeField] float gamepadSensitivity = 1000f;

        public event Action<Vector2, Image, GameObject> OnHoverInteractObject;
        public override void InitEvents()
        {
           
        }
        
        public override void InitRunTime()
        {
            base.InitRunTime();
            InitUI();
        }
        
        private void InitUI()
        {
            canvas.SetMeOnLayer(Constants.Layers.TOP_UI);
            canvas.FullSize();
            canvas.SetMeOnLayer(Constants.Layers.UI);
            canvas.sortingOrder = 1024;
            cursor = canvas.transform.AddChildComponentIfNotExist<Image>("cursor");
            if (defaultCursorIcon != null)
            {
                // Create a new Sprite from the texture
                // Rect defines the area (full texture), Pivot (0.5, 0.5) centers it

                defautlSprite = Sprite.Create(
                     defaultCursorIcon,
                     new Rect(0, 0, defaultCursorIcon.width, defaultCursorIcon.height),
                     new Vector2(0.5f, 0.5f)
                 );
                cursor.sprite = defautlSprite;
                // 4. Set the UI size based on the texture dimensions
                cursor.rectTransform.sizeDelta = new Vector2(defaultCursorIcon.width, defaultCursorIcon.height);
            }

            // 5. Reset position to center of screen initially
            cursor.rectTransform.anchoredPosition = Vector2.zero;
            // Hide the system cursor
            Cursor.visible = false;
        }
        void UpdateCursorPosition()
        {
            if (cursor == null) return;
            Vector2 deltaMouse = Vector2.zero;
            Vector2 stickInput = Vector2.zero;

            // 1. Check Mouse (New System)
            if (Mouse.current != null)
            {
                deltaMouse = Mouse.current.delta.ReadValue();
            }

            // 2. Check Gamepad (New System)
            if (Gamepad.current != null)
            {
                // Right Stick thường là stick phía tay phải
                stickInput = Gamepad.current.rightStick.ReadValue();
            }

            // --- Logic cập nhật vị trí ---

            // Nếu chuột di chuyển (delta khác 0)
            if (deltaMouse.sqrMagnitude > 0.01f)
            {
                // Với hệ thống mới, bạn có thể lấy vị trí chuột trực tiếp
                _virtualMousePos = Mouse.current.position.ReadValue();
            }
            else if (stickInput.sqrMagnitude > 0.1f) // Deadzone
            {
                _virtualMousePos += (Vector2)stickInput * gamepadSensitivity * Time.deltaTime;
            }

            // Clamp và Update UI như cũ
            _virtualMousePos.x = Mathf.Clamp(_virtualMousePos.x, 0, Screen.width);
            _virtualMousePos.y = Mathf.Clamp(_virtualMousePos.y, 0, Screen.height);

            cursor.rectTransform.position = _virtualMousePos;
        }
        private void LateUpdate()
        {
            if (cursor == null) return;
            UpdateCursorPosition();
            var pos = (Vector2)_virtualMousePos;
            var interactObject = pos.GetHitCollider<Transform>(Constants.Layers.INTERACT_OBJECT);
            if (interactObject != null)
            {
                OnHoverInteractObject?.Invoke(pos, this.cursor, interactObject.gameObject);
               
            }
            else
            {
                cursor.sprite = defautlSprite;
            }
        }
#if UNITY_EDITOR
        [UnvsButton]
        public void Generate()
        {
            canvas = this.AddChildComponentIfNotExist<Canvas>("canvas");
           

            
        }

       
#endif
    }
}