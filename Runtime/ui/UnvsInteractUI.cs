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
using unvs.components;
using unvs.data;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;
using unvs.shares;
using unvs.types;
using static PlasticPipe.Server.MonitorStats;

namespace unvs.ui
{

    public partial class UnvsInteractUI : UnvsUIComponentInstance<UnvsInteractUI>
    {
        //public Texture2D defaultCursorIcon;
        public Image virtualCursor;
        private types.IconInfo _currentCursor;
        private Vector2 _virtualMousePos;
        //[SerializeField]
        //public types.IconInfo DefaultIcon;
        public UnvsCursor DefaultCursor;
       
        [SerializeField] float gamepadSensitivity = 1000f;
        private float _animTimer;
        

        /// <summary>
        /// THis UI allow showing when game playing, so no need to hide or show player
        /// </summary>
        public override bool DisablePlayerInput => false;
        /// <summary>
        /// THis UI allow showing when game playing
        /// </summary>
        public override bool EnablePlayerInput => false;

        public bool Locked { get; internal set; }

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
            virtualCursor = canvas.transform.AddChildComponentIfNotExist<Image>("cursor");
            ChangeIcon(this.DefaultCursor);

            //cursor.sprite = DefaultIcon.srpite;
            //// 5. Reset position to center of screen initially
            //cursor.rectTransform.anchoredPosition = DefaultIcon.Pivot;
            //cursor.rectTransform.sizeDelta = DefaultIcon.size;
            // Hide the system cursor
            Cursor.visible = false;
            //Cursor.SetCursor()
        }
        public void ChangeIcon(UnvsCursor cursorData)
        {
            var c = cursorData ?? DefaultCursor;
            if(c == null)
            {
                return;
            }
            if (c.icon.sprites.Length>0)
            {
                _currentCursor = cursorData.icon;
                if (cursorData.icon.size != Vector2.zero)
                    virtualCursor.rectTransform.sizeDelta = cursorData.icon.size;
                virtualCursor.rectTransform.anchoredPosition = cursorData.icon.Pivot;
                UpdateCursorPosition();
            }
          
        }
        public void RestoreDefaultIcon()
        {
            ChangeIcon(DefaultCursor);
        }
        void UpdateCursorPosition()
        {
            if (virtualCursor == null) return;
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

            virtualCursor.rectTransform.position = _virtualMousePos;
        }
        private void LateUpdate()
        {
            if(Locked) return;

            if(UnvsCinema.Instance==null) return;
            //if (UnvsCinema.Instance.IsInUpdateState) return;
            if (virtualCursor == null) return;
            UpdateCursorPosition();

            var pos = (Vector2)_virtualMousePos;
            Camera cam = Camera.main; // Đảm bảo lấy đúng camera đang dùng

            if (cam == null) return;

            // Thay vì dùng Screen, hãy dùng pixelRect của chính Camera đó
            Rect safeRect = cam.pixelRect;

            if (!safeRect.Contains(pos))
            {
                ChangeIcon(DefaultCursor);
                //_currentCursor = DefaultIcon.;
                //cursor.sprite = this.DefaultIcon.srpite;
                return;
            }
            var interactObject = pos.GetHitCollider<UnvsInteractObject>(Constants.Layers.INTERACT_OBJECT);

            if (interactObject != null)
            {
                if (interactObject.CursorData!=null)
                {
                  
                    ChangeIcon(interactObject.CursorData);
                   
                } else
                {
                  
                    ChangeIcon(DefaultCursor);
                   
                   
                }
            }
            else
            {
                
                ChangeIcon(DefaultCursor);


            }
            _animTimer += Time.unscaledDeltaTime;

            virtualCursor.sprite = _currentCursor.GetFrame(_animTimer);
            UpdateCursorPosition();
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