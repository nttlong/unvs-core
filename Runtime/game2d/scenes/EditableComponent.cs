using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
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
            else
                InitDesignTime();
        }

        public abstract void InitDesignTime();

        public abstract void InitRuntime();
    }
    public abstract class UnvsBaseComponent : MonoBehaviour
    {
        
       


    }
    public abstract class UnvsUIComponent: UnvsBaseComponent
    {
        public Canvas canvas;

        public abstract void InitEvents();
        public abstract void InitRunTime();

        public virtual void Hide()
        {
            this.enabled = false;
            if(canvas == null)
                canvas = this.GetComponentInChildren<Canvas>(true);
            if(canvas != null )
            {
                canvas.enabled = false;
                canvas.gameObject.SetActive(false);
            }
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

            }
        }
    }
    public abstract class UnvsUIComponentInstance<T>: UnvsUIComponent where T : Component
    {
        
        public static T Instance;
        public override void InitRunTime()
        {
            Instance = this as T;

            
        }

    }
}