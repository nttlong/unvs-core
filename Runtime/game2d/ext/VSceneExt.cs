//using UnityEngine.UI;
//using unvs.ext;
//using unvs.game2d.scenes;

//namespace unvs.game2d.ext
//{
//    public static class VSceneExt
//    {
//        #if UNITY_EDITOR
//        public static void EditorCreateScene(this VScene vscene)
//        {
            
//        }
//        #endif
//    }
//    public static class AppMainMenuExt
//    {
//#if UNITY_EDITOR
//        public static void EditorCreateMenuLayout(this AppMainMenu menu)
//        {


//            menu.canvas = menu.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
//            menu.panel = menu.canvas.transform.AddChildComponentIfNotExist<Image>("panel");
//            var vlg = menu.panel.AddComponentIfNotExist<VerticalLayoutGroup>();
//            vlg.FixLayoutChildren();
//            menu.btnStart = menu.panel.transform.AddButtonIfNotExist("btnStart", "Start");
//            menu.btnExit = menu.panel.transform.AddButtonIfNotExist("btnExit", "Exit");
//        }
//#endif
//    }

//}