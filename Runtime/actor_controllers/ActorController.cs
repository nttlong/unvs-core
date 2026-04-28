using System;
using Unity.VisualScripting;
using UnityEngine;
using unvs.actor.player;
using unvs.components;
using unvs.game2d.actors;
using unvs.game2d.objects.editor;

namespace unvs.controllers
{
    ////unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    //[Serializable]
    //public partial class ActorController<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    //{

    //    //public UnvsPlayer Player;
    //}
#if UNITY_EDITOR
    [Serializable]
    public partial class ActorController<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        [UnvsButton("Create default controller")]
        public void EditorCreateDefaltPlayerController()
        {
            //BasicController
            var control = Owner.GetComponent<UnvsPlayer>();
            if (control!=null)
            {
                unvs.editor.utils.Dialogs.Show($"Please, remove {control.GetType()} before add {typeof(BasicController)}");
                return;
            }
            Owner.AddComponent<BasicController>();
        }
        [UnvsButton("Create mouse controller")]
        public void EditorCreateDefaltMousePlayerController()
        {
            //BasicController
            var control = Owner.GetComponent<UnvsPlayer>();
            if (control != null)
            {
                unvs.editor.utils.Dialogs.Show($"Please, remove {control.GetType()} before add {typeof(BasicController)}");
                return;
            }
            Owner.AddComponent<BasicMouseController>();
        }
    }
#endif
}