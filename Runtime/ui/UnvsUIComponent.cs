using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using unvs.components;
using unvs.components;
using unvs.controllers.inputs;
using unvs.data;
using unvs.ext;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;

namespace unvs.ui
{
    public abstract class UnvsNonUIComponent : UnvsBaseComponent
    {
        public virtual void Awake()
        {

            if (Application.isPlaying)
            {
                InitRunTime();
            }
        }

        public abstract void InitRunTime();
    }
    public abstract class UnvsUIComponent : UnvsBaseComponent
    {

        [Header("Feedback audio")]
        public types.AudioInfo AudioOpen;
        public types.AudioInfo AudioClose;

        [Header("Visualize")]
       
        public UnvsCursor UICursor;
        [SerializeField]
        public types.UINavigateSettings navigateSettings;
        public Canvas canvas;
        public bool IsShow;
        public abstract void InitEvents();
        public abstract void InitRunTime();
        /// <summary>
        /// Disable Player Input when this UI is showing
        /// </summary>
        public abstract bool DisablePlayerInput
        {
            get;
        }
        /// <summary>
        /// Enale PlayerInput when this UI is hidding
        /// </summary>
        public abstract bool EnablePlayerInput
        {
            get;
        }
        public virtual void Hide()
        {
            this.enabled = false;
            if (canvas == null)
                canvas = this.GetComponentInChildren<Canvas>(true);
            if (canvas != null)
            {
                canvas.enabled = false;
                canvas.gameObject.SetActive(false);
            }
           
            if (UnvsGlobalInput.Player != null)
            {
                if (EnablePlayerInput)
                {
                    UnvsGlobalInput.PlayerEnable();
                }

            }
            IsShow = false;
            if(DisablePlayerInput && UnvsInteractUI.Instance != null)
            {
                UnvsInteractUI.Instance.RestoreLastCursor();
            }
        }

        public virtual void ApplyNaviagatorButtons()
        {
            
            foreach (var b in navigateSettings.items)
            {
                if(b.gameObject!=null && !b.gameObject.IsDestroyed() && b.defaultSelected)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(b.gameObject);
                }
            }
        }
        public virtual void Toggle()
        {
            if(!IsShow)
            {
                Show();
            } else
            {
                Hide();
            }
        }
        public virtual void Show()
        {
            this.enabled = true;
            this.gameObject.SetActive(true);
            if (canvas == null)
            {
                canvas = this.GetComponentInChildren<Canvas>(true);
                if (canvas != null) canvas.UIFullSize();
            }

            if (canvas != null)
            {
                canvas.enabled = true;
                canvas.gameObject.SetActive(true);
            }

          
           
            if (UnvsGlobalInput.Player != null)
            {
                if (DisablePlayerInput)
                {
                    UnvsGlobalInput.PlayerDisable();
                }
            }
            IsShow = true;
            if (DisablePlayerInput && UnvsInteractUI.Instance != null)
            {
                var cursor = this.UICursor ?? UnvsApp.Instance.Settings.DefautUICursor;
                UnvsInteractUI.Instance.BackupLastCursor();
                UnvsInteractUI.Instance.SetCurrentCursorIcon(cursor);
            }
        }
        public virtual void Activate()
        {
            this.enabled = true;
            this.gameObject.SetActive(true);
        }
        public virtual void Deactive()
        {
            this.enabled = false;
            this.gameObject.SetActive(false);
        }
        public virtual void Awake()
        {

            if (Application.isPlaying)
            {
                InitRunTime();


                //if (canvas == null)
                //    canvas = this.GetComponentInChildren<Canvas>(true);
                if (canvas != null)
                {
                    canvas.UIFullSize();
                    if (IsShow)
                    {
                        this.Show();
                    }
                    else
                    {
                        this.Hide();
                    }
                    //canvas.enabled = IsShow;

                }

                InitEvents();
            }
        }
#if UNITY_EDITOR
        [UnvsButton("navigate settings")]
        public void InitnavigateSettings()
        {
            this.navigateSettings.items = this.GetComponentsInChildren<Button>().Select(p => new types.UINavigateItem
            {
                gameObject=p.gameObject,
                defaultSelected=false
            }).ToArray();
        }

        
#endif
    }
}