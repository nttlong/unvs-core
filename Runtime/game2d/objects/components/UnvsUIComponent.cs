using UnityEngine;
using UnityEngine.UI;
using unvs.controllers_input;
using unvs.ext;
using unvs.game2d.scenes;

namespace unvs.game2d.objects.components
{
     public abstract class UnvsUIComponent : UnvsBaseComponent
    {
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
            IsShow = false;
            if(UnvsGlobalInput.Player != null)
            {
                if (EnablePlayerInput)
                {
                    UnvsGlobalInput.PlayerEnable();
                }

            }
            //UnvsSceneLoader.GameShow();
        }

       

        public virtual void Show()
        {
            this.enabled = true;
            this.gameObject.SetActive(true);
            if (canvas == null)
            {
                canvas = this.GetComponentInChildren<Canvas>(true);
                if (canvas != null) canvas.FullSize();
            }

            if (canvas != null)
            {
                canvas.enabled = true;
                canvas.gameObject.SetActive(true);
            }
           
            this.ApplyNavigate<Button>();
            IsShow=true;
            if (UnvsGlobalInput.Player != null)
            {
                if (DisablePlayerInput)
                {
                    UnvsGlobalInput.PlayerDisable();
                }
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
                    canvas.FullSize();

                }
                InitEvents();
            }
        }
    }
}