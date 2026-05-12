using Cysharp.Threading.Tasks;

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.UI;
using unvs.actor.player;
using unvs.controllers.inputs;
using unvs.data;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;
using unvs.shares;


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


        //private float _animTimer;


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
            //if (!UnvsApp.Instance.Settings.UseLookNavigator) return;
            UnvsGlobalInput.OnUIInputReady += () =>
            {
                this.look = UnvsGlobalInput.NewMapUIAction(this, "Look", action =>
                {
                    action.performed += ctx =>
                    {
                        _virtualMousePos = ctx.ReadValue<Vector2>();
                        updateCursor();
                    };
                });
            };
        }

        public override void InitRunTime()
        {
            base.InitRunTime();
            InitUI();

        }

        private void InitUI()
        {
            canvas.SetMeOnLayer(Constants.Layers.TOP_UI);
            canvas.UIFullSize();
            canvas.SetMeOnLayer(Constants.Layers.UI);
            canvas.sortingOrder = 1024;

            virtualCursor = canvas.transform.AddChildComponentIfNotExist<Image>("cursor");
            if (UnvsApp.Instance.Settings.UseLookNavigator)
            {
                virtualCursor.gameObject.SetActive(false);
            }

            Cursor.visible = false;

        }
        public void ChangeIcon(UnvsCursor cursorData)
        {
            this.Cts = this.Cts.Refresh();
            var c = cursorData ?? DefaultCursor;
            if (c == null)
            {
                return;
            }
            if (c.icon.sprites.Length > 0)
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
            unvs.ext.ImageExt.ShowAtUIPosition(virtualCursor, _virtualMousePos);



        }
        bool _isInitCursor = false;
        private MapAction look;
        private CancellationTokenSource Cts = new CancellationTokenSource();
        private UnvsInteractObject _lastCheck;

        void updateCursor()
        {
            if (Locked) return;
            if (!_isInitCursor)
            {
                _currentCursor = DefaultCursor.icon;
                _isInitCursor = true;
            }
            if (UnvsCinema.Instance == null) return;
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
                if (interactObject.CursorData != null)
                {

                    ChangeIcon(interactObject.CursorData);

                }
                else
                {

                    ChangeIcon(DefaultCursor);


                }
            }
            else
            {

                ChangeIcon(DefaultCursor);


            }
            //_animTimer += Time.unscaledDeltaTime;
            _currentCursor.PlayAnimAsync(virtualCursor, this.Cts.Token).Forget();

            UpdateCursorPosition();

        }
        private void LateUpdate()
        {
            if (UnvsApp.Instance == null) return;
            if (UnvsApp.Instance.Settings.UseLookNavigator) return;
            if (UnvsApp.Instance.currentActor == null) return;
            var check = UnvsApp.Instance.currentActor.ScanObject<UnvsInteractObject>();
            if (check != null && check.CursorData != null)
            {
                if (UnvsApp.Instance.InteractingTask.Status != UniTaskStatus.Pending)
                {
                    if (_lastCheck != check)
                    {
                        this.Cts = this.Cts.Refresh();
                        check.CursorData.icon.ShowAtAsync(worldPos: check.GetCenterPos(), virtualCursor, this.Cts.Token).Forget();
                    }
                } else
                {
                    this.Cts = this.Cts.Refresh();
                    virtualCursor.gameObject.SetActive(false);
                }

            }
            else
            {
                this.Cts = this.Cts.Refresh();
                virtualCursor.gameObject.SetActive(false);
            }
            _lastCheck = check;
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