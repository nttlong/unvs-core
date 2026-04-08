using game2d.scenes;
using System;
using UnityEngine.UI;
using unvs.ext;

namespace game2d.ext
{
    public static class AppFadeScreenExt
    {
        public static void EditorGenerate(this AppFadeScreen cinema)
        {
            cinema.canvas=cinema.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            cinema.panel = cinema.canvas.transform.AddChildComponentIfNotExist<Image>("canvas");
            cinema.EditorSetDirty();
        }
    }
}