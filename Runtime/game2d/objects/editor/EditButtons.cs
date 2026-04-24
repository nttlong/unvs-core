using System;

namespace unvs.game2d.objects.editor
{
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
}