using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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
using unvs.types;


namespace unvs.ui
{
    /// <summary>
    /// CAUTION!: Trong input_system cua UI  phai set ActionType cua Look la Pass Throught
    /// CAUTION!: input_system.Look.ActionType mus be Pass Throught
    /// </summary>
    public partial class UnvsInteractUI : UnvsUIComponentInstance<UnvsInteractUI>
    {

        public Image virtualCursor;
        private types.IconInfo _currentCursor;
        private UnvsCursor lastCursorData;
        public Vector2 virtualMousePos;

        //public UnvsCursor DefaultCursor;
        public UnvsInteractObject lastInteractObject;

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
            if (!UnvsApp.Instance.Settings.UseLookNavigator)
            {
                this.virtualCursor.Hide();
                return;
            }
            _currentCursor = UnvsApp.Instance.Settings.DefautCursor.icon;
            this.virtualCursor.ShowImage();
            UnvsGlobalInput.OnUIInputReady += () =>
            {
               
                this.look = UnvsGlobalInput.NewMapUIAction(this, "Look", action =>
                {
                    action.performed += ctx =>
                    {
                        
                        
                        if (ctx.control.device is Mouse)
                        {
                            this.lookPositionWatchSource = this.lookPositionWatchSource.Refresh();
                            virtualMousePos = ctx.ReadValue<Vector2>();
                            updateCursor();
                        } else
                        {
                            
                            this.StartWatch(
                                () => virtualMousePos,
                                (val) => virtualMousePos = val,
                                () => ctx.ReadValue<Vector2>(), // Biến này được cập nhật ở performed/canceled
                                updateCursor,
                                UnvsApp.Instance.Settings.GamepadLookCursorSpeed,
                                this.lookPositionWatchSource.Token
                            ).Forget(); // Chạy và quên nó đi (UniTask sẽ tự quản lý)
                        }

                         
                    };
                    
                });
            };
        }

        public override void InitRunTime()
        {
            base.InitRunTime();
            InitUI();
            lastCursorData = UnvsApp.Instance.Settings.DefautCursor;

        }

        private void InitUI()
        {
            canvas.SetMeOnLayer(Constants.Layers.TOP_UI);
            canvas.UIFullSize();
            canvas.SetMeOnLayer(Constants.Layers.UI);
            canvas.sortingOrder = 1024;
            virtualCursor = canvas.transform.AddChildComponentIfNotExist<Image>("cursor");

            if (!UnvsApp.Instance.Settings.UseLookNavigator)
            {

                virtualCursor.HideImage();
            }
            else
            {
                virtualCursor.ShowImage();
                _currentCursor = UnvsApp.Instance.Settings.DefautCursor.icon;
            }

            UnityEngine.Cursor.visible = false;

        }
        public void ChangeIcon(UnvsCursor cursorData)
        {
            this.Cts = this.Cts.Refresh();
            var c = cursorData ?? UnvsApp.Instance.Settings.DefautCursor;
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
            ChangeIcon(UnvsApp.Instance.Settings.DefautCursor);
        }
        void UpdateCursorPosition()
        {
            if (virtualCursor == null) return;
            unvs.ext.ImageExt.ShowAtUIPosition(virtualCursor, virtualMousePos);



        }
        bool _isInitCursor = false;
        private MapAction playerLook;
        private MapAction interact;
        private MapAction look;
        public CancellationTokenSource Cts = new CancellationTokenSource();
        private CancellationTokenSource lookPositionWatchSource = new CancellationTokenSource();
        private IconInfo _tmpBackupIcon;
        public void SetCurrentCursorIcon(UnvsCursor cursor)
        {
            if(lastCursorData!= cursor)
            {
                this.Cts = this.Cts.Refresh();
                _currentCursor = cursor.icon;
                lastCursorData = cursor;
                _currentCursor.PlayAnimAsync(this.virtualCursor,Cts.Token).Forget();
            }
            
            
        }
        void updateCursor()
        {
            if (Locked) return;
            if (!_isInitCursor)
            {
                _currentCursor = lastCursorData.icon; // DefaultCursor.icon;
                _isInitCursor = true;
            }
            if (UnvsCinema.Instance == null) return;
            //if (UnvsCinema.Instance.IsInUpdateState) return;
            if (virtualCursor == null) return;
            UpdateCursorPosition();

            var pos = (Vector2)virtualMousePos;
            Camera cam = Camera.main; // Đảm bảo lấy đúng camera đang dùng

            if (cam == null) return;

            // Thay vì dùng Screen, hãy dùng pixelRect của chính Camera đó
            Rect safeRect = cam.pixelRect;

            if (!safeRect.Contains(pos))
            {
                ChangeIcon(lastCursorData);
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

                    ChangeIcon(lastCursorData);


                }
            }
            else
            {

                ChangeIcon(lastCursorData);


            }
            //_animTimer += Time.unscaledDeltaTime;
            _currentCursor.PlayAnimAsync(virtualCursor, this.Cts.Token).Forget();

            UpdateCursorPosition();

        }
        public async UniTask DoShowIconOfInteractableItemAsync(UnvsInteractObject check)
        {
            if (UnvsApp.Instance == null) return;
            if (UnvsApp.Instance.Settings.UseLookNavigator) return;
            if (UnvsApp.Instance.currentActor == null) return;

            if (check != null && check.CursorData != null)
            {
                if (UnvsApp.Instance.InteractingTask.Status != UniTaskStatus.Pending)
                {
                    if (lastInteractObject != check)
                    {
                        this.Cts = this.Cts.Refresh();
                        await check.CursorData.icon.ShowAtAsync(worldPos: check.GetCenterPos(), virtualCursor, this.Cts.Token);
                    }
                }
                else
                {
                    this.Cts = this.Cts.Refresh();
                    virtualCursor.gameObject.SetActive(false);
                }

            }
            else
            {
                if (lastInteractObject != check)
                {
                    this.Cts = this.Cts.Refresh();

                }
                virtualCursor.Hide();
            }
            lastInteractObject = check;
        }
        private void FixedUpdateDelete()
        {
            if (UnvsApp.Instance == null) return;
            if (UnvsApp.Instance.Settings.UseLookNavigator) return;
            if (UnvsApp.Instance.currentActor == null) return;
            var check = UnvsApp.Instance.currentActor.ScanObject<UnvsInteractObject>();
            DoShowIconOfInteractableItemAsync(check).Forget();
        }

        public void SwitchDefautIcon()
        {
            if (!UnvsApp.Instance.Settings.UseLookNavigator) return;
            _tmpBackupIcon = _currentCursor;
            _currentCursor = UnvsApp.Instance.Settings.DefautCursor.icon;
           
            this.Cts= this.Cts.Refresh();
            _currentCursor.PlayAnimAsync(virtualCursor, this.Cts.Token).Forget();
            UpdateCursorPosition();
        }

        public void RestorePreviousIcon()
        {
            if (!UnvsApp.Instance.Settings.UseLookNavigator) return;
            _currentCursor = _tmpBackupIcon;
          
            this.Cts = this.Cts.Refresh();
            _currentCursor.PlayAnimAsync(virtualCursor, this.Cts.Token).Forget();
            UpdateCursorPosition();
        }

        public void RestoreLastCursor()
        {
            if(_cursorStack.Count>0)
            lastCursorData= _cursorStack.Pop();
        }
        Stack<UnvsCursor> _cursorStack=new Stack<UnvsCursor>();
        public void BackupLastCursor()
        {
            _cursorStack.Push(lastCursorData);
        }
    }
    public partial class UnvsInteractUI : UnvsUIComponentInstance<UnvsInteractUI>
    {
        private List<UnvsInteractObject> _lstItem;
        public void AddInteractItem(UnvsInteractObject item)
        {
            if (item == null || item.IsDestroyed()) return;

            // Ensure list is initialized
            _lstItem ??= new List<UnvsInteractObject>();

            // Prevent adding the same item twice (and subscribing twice)
            if (_lstItem.Contains(item)) return;

            // Use a local function or lambda, but we must be careful with the reference
            Action handler = null;
            handler = () =>
            {
                if (_lstItem != null)
                {
                    _lstItem.Remove(item);
                }
                // Unsubscribe to avoid memory leaks
                item.OnDestroying -= handler;
            };

            item.OnDestroying += handler;
            _lstItem.Add(item);
        }

        public void RemoveInteractItem(UnvsInteractObject item)
        {
            if (item == null || item.IsDestroyed()) return;
            _lstItem ??= new List<UnvsInteractObject>();
            this._lstItem.Remove(item);

        }
        public void HideInteracIconOfItem()
        {
            Cts = Cts.Refresh();
            virtualCursor.HideImage();
        }
        public async UniTask ShowIconOfInteracItem(Vector2 centerPoint)
        {
            if(UnvsDialog.Instance.DialogShowingTask!=null && UnvsDialog.Instance.DialogShowingTask.Task.Status == UniTaskStatus.Pending)
            {
                virtualCursor.HideImage();
                return;
            }
            if (_lstItem == null || _lstItem.Count == 0)
            {
               virtualCursor.HideImage();
                return;
            }

            
            UnvsInteractObject nearsetItem = null;
            float minSqrDistance = float.MaxValue;

            foreach (var item in _lstItem)
            {
                if (item == null || item.IsDestroyed()) continue;

                var itemPos = item.GetCenterPos();
                // Calculate squared distance for performance optimization
                float sqrDist = (new Vector2(itemPos.x, itemPos.y) - (Vector2)centerPoint).sqrMagnitude;

                if (sqrDist < minSqrDistance)
                {
                    minSqrDistance = sqrDist;
                    nearsetItem = item;
                }
            }

            await DoShowIconOfInteractableItemAsync(nearsetItem);

        }

        public int GetTotalItem()
        {
            return _lstItem != null ? _lstItem.Count : 0;
        }

        public void ClearInteractItemList()
        {
            if(_lstItem==null ) _lstItem = new List<UnvsInteractObject> ();
            _lstItem.Clear();
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