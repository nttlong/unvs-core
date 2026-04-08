using Cysharp.Threading.Tasks.Triggers;
using game2d.scenes;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using unvs.ext;

namespace game2d.ext {
    public static class AppDialogExt
    {
        public static void Generate(this AppDialog dialog)
        {
            dialog.canvas = dialog.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            dialog.panel= dialog.canvas.transform.AddChildComponentIfNotExist<Image>("panel");
            var mvlg = dialog.panel.AddComponentIfNotExist<VerticalLayoutGroup>();
            mvlg.FixFullLayoutChildren();
            dialog.contentPanel = dialog.panel.AddChildComponentIfNotExist<Image>("contentPanel");
            dialog.icon= dialog.contentPanel.AddChildIfNotExist<Image>("icon");
            dialog.text = dialog.contentPanel.AddChildIfNotExist<TextMeshProUGUI>("text");
            var ctLg = dialog.contentPanel.AddComponentIfNotExist<VerticalLayoutGroup>();
            ctLg.FixFullLayoutChildren();
            //dialog.contentPanel.DockFull();
            dialog.foolterPanel = dialog.panel.AddChildComponentIfNotExist<Image>("foolterPanel");
            dialog.foolterPanel.DockBottom(120f);
            var hlg=dialog.foolterPanel.AddComponentIfNotExist<HorizontalLayoutGroup>();
            hlg.FixLayoutChildren();
            dialog.btnOk = dialog.foolterPanel.transform.AddButtonIfNotExist("btnOK", "OK");
            dialog.btnCancel = dialog.foolterPanel.transform.AddButtonIfNotExist("btnCandel", "Cancel");
            dialog.EditorSetDirty();
        }
    }
}