using System;
using System.Collections.Generic;
using UnityEngine;
namespace game2d.scenes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InspectorButtonAttribute : Attribute
    {
        public string ButtonName { get; }

        public InspectorButtonAttribute(string buttonName = null)
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

    public abstract class UnvsComponent : MonoBehaviour
    {
        public virtual void Awake()
        {
            InitRuntime();
        }

        public abstract void InitRuntime();


    }
}