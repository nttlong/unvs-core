using System;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.scenes;

namespace game2d.ext
    {
        public static class AppPauseMenuExt
        {

#if UNITY_EDITOR
        public static void EditorCreate(AppPauseMenu menu)
        {
            menu.canvas = menu.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            menu.panel = menu.canvas.transform.AddChildComponentIfNotExist<UnityEngine.UI.Image>("panel");
            var vlg= menu.panel.AddComponentIfNotExist<VerticalLayoutGroup>();
            menu.btnResume = menu.panel.transform.AddButtonIfNotExist("btnResume", "Resume");
            menu.btnMainMenu = menu.panel.transform.AddButtonIfNotExist("btnMainMenu", "To Title Screen");
            menu.btnExit = menu.panel.transform.AddButtonIfNotExist("btnExit", "Exit");
            vlg.FixLayoutChildren();
            menu.EditorSave();
        }
        
#endif
        }
    }