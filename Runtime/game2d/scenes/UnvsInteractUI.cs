using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using game2d.ext;
using game2d.scenes;

using System;
using Unity.Burst.Intrinsics;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using unvs.shares;

namespace unvs.game2d.scenes
{

    public partial class UnvsInteractUI : UnvsUIComponentInstance<UnvsInteractUI>
    {
        public Texture2D defaultCursorIcon;
        public Image cursor;
        private Vector2 _virtualMousePos;
        private Sprite defautlSprite;
        [SerializeField] float gamepadSensitivity = 1000f;
        /// <summary>
        /// THis UI allow showing when game playing, so no need to hide or show player
        /// </summary>
        public override bool DisablePlayerInput => false;
        /// <summary>
        /// THis UI allow showing when game playing
        /// </summary>
        public override bool EnablePlayerInput => false;

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

            if(UnvsCinema.Instance==null) return;
            //if (UnvsCinema.Instance.IsInUpdateState) return;
            if (cursor == null) return;
            UpdateCursorPosition();

            var pos = (Vector2)_virtualMousePos;
            Camera cam = Camera.main; // Đảm bảo lấy đúng camera đang dùng

            if (cam == null) return;

            // Thay vì dùng Screen, hãy dùng pixelRect của chính Camera đó
            Rect safeRect = cam.pixelRect;

            if (!safeRect.Contains(pos))
            {
                cursor.sprite = defautlSprite;
                return;
            }
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

        

       


    }
#if UNITY_EDITOR
    public partial class UnvsInteractUI : UnvsUIComponentInstance<UnvsInteractUI>
    {
        [UnvsButton]
        public void Generate()
        {
            canvas = this.AddChildComponentIfNotExist<Canvas>("canvas");
            


        }
        [UnvsButton("Create psd app file")]
        public async UniTask EditorCreateIConPSDFile()
        {
            if (UnityEditor.Selection.activeGameObject != this.gameObject)
            {
                unvs.editor.utils.Dialogs.Show($"Please ,select {this.GetType()},instead of {UnityEditor.Selection.activeGameObject.GetType()}");
                return;
            }
            var folder = unvs.editor.utils.UnvsEditorUtils.GetAbsFolderPathOfGameObject(UnityEditor.Selection.activeGameObject);
            var file_path = System.IO.Path.Join(folder, "icons.psd");
            if (!unvs.editor.utils.Dialogs.Confirm($"Do you want to create new psd file at \n{file_path}"))
            {
                return;
            }
            if (!await unvs.editor.utils.UnvsPythonCall.HealthCheck()) return;
            var dataLayer = unvs.editor.utils.PsdFile.Createlayers(file_path);
            dataLayer.AddBox("default-icon", 64, 64);
           
          
            await unvs.editor.utils.UnvsPythonCall.Call("UnvsPsd", "create_dumny_actor_psd", dataLayer);

        }
    }
#endif
}