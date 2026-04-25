using System;
using Unity.VisualScripting;
using UnityEngine;
using unvs.actor.player;
using unvs.game2d.actors;
using unvs.game2d.objects.editor;

namespace unvs.controllers
{
    [Serializable]
    public partial class ActorController : unvs.types.UnvsEditableProperty
    {
        public MonoBehaviour owner;
        public UnvsPlayer Player;
    }
#if UNITY_EDITOR
    public partial class ActorController : unvs.types.UnvsEditableProperty
    {
        [UnvsButton("Create default controller")]
        public void EditorCreateDefaltPlayerController()
        {
            //BasicController
            var control = owner.GetComponent<UnvsPlayer>();
            if (control!=null)
            {
                unvs.editor.utils.Dialogs.Show($"Please, remove {control.GetType()} before add {typeof(BasicController)}");
                return;
            }
            owner.AddComponent<BasicController>();
        }
        [UnvsButton("Create mouse controller")]
        public void EditorCreateDefaltMousePlayerController()
        {
            //BasicController
            var control = owner.GetComponent<UnvsPlayer>();
            if (control != null)
            {
                unvs.editor.utils.Dialogs.Show($"Please, remove {control.GetType()} before add {typeof(BasicController)}");
                return;
            }
            owner.AddComponent<BasicMouseController>();
        }
    }
#endif
}