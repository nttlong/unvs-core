using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using unvs.ext;
using unvs.shares;
namespace unvs.game2d.scenes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnvsButtonAttribute : Attribute
    {
        public string ButtonName { get; }

        public UnvsButtonAttribute(string buttonName = null)
        {
            ButtonName = buttonName;
        }
    }
    public enum UnvsPropertyTypeEnum
    {
        List
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class UnvsPropertyAttribute : Attribute
    {
       
        public UnvsPropertyTypeEnum PType { get; }
        public UnvsPropertyAttribute(UnvsPropertyTypeEnum pType)
        {
            PType = pType;
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EditButtons : Attribute
    {
        private string[] cmds;

        public EditButtons(params string[] Commands)
        {
            this.cmds = Commands;
        }

        public string[] GetCommands()
        {
            return cmds;
        }
    }
    public abstract class UnvsComponent : UnvsBaseComponent
    {

        public virtual void Awake()
        {
            if (Application.isPlaying)

                InitRuntime();

        }


        public abstract void InitRuntime();
    }
    public abstract class UnvsComponentEvetns: UnvsComponent
    {
        internal Action onDisable;
        private void OnDisable()
        {
            onDisable.Invoke();
        }
    }
    public abstract class UnvsList
    {

    }
    public abstract class UnvsBaseComponent : MonoBehaviour
    {


#if UNITY_EDITOR
        public Action<string> OnEditorError;
        public void RaiseEditorError(string error)
        {
            OnEditorError?.Invoke(error);
        }
#endif


    }
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
    public abstract class UnvsUIComponentInstance<T> : UnvsUIComponent where T : Component
    {

        public static T Instance;
        public override void InitRunTime()
        {
            Instance = this as T;
           


        }

    }
    
}