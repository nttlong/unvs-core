using System;

namespace unvs.game2d.objects.editor
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
}