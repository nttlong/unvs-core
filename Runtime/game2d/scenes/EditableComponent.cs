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
        private Canvas _canvas;

        public abstract void InitEvents();
        public abstract void InitRunTime();

        public virtual void Hide()
        {
            this.enabled = false;
            if(_canvas==null)
            _canvas=this.GetComponentInChildren<Canvas>(true);
            if( _canvas!=null )
            {
                _canvas.enabled = true;
                _canvas.gameObject.SetActive(false);
            }
        }
        public virtual void Show()
        {
            this.enabled = true;
            this.gameObject.SetActive(true);
            if (_canvas == null)
            {
                _canvas = this.GetComponentInChildren<Canvas>(true);
                if (_canvas != null) _canvas.FullSize();
            }
                
            if (_canvas != null)
            {
                _canvas.enabled = true;
                _canvas.gameObject.SetActive(true);
            }
            this.ApplyNavigate<Button>();
        }
        public virtual void Awake()
        {
            Debug.Log($"Awake={this}");
            if (Application.isPlaying)
            {
                InitRunTime();
                Debug.Log($"Awake.isPlaying={this}");
                if (_canvas == null)
                    _canvas = this.GetComponentInChildren<Canvas>(true);
                if (_canvas != null)
                {
                    _canvas.FullSize();
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