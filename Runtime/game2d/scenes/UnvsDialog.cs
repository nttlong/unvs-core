using game2d.ext;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.scenes;

namespace game2d.scenes {
    public class UnvsDialog : UnvsUIComponentInstance<UnvsDialog>
    {
      
        public Image panel;
        public Image contentPanel;
        public Image foolterPanel;
        public Image icon;
        public TextMeshProUGUI text;
        public Button btnOk;
        public Button btnCancel;

        public override bool DisablePlayerInput => false;

        public override bool EnablePlayerInput => false;
        
        public override void InitEvents()
        {
            
        }

        public override void InitRunTime()
        {
            
            canvas.FullSize();
        }
#if UNITY_EDITOR
        [UnvsButton()]
        public  void Generate()
        {
            var dialog = this;
            dialog.canvas = dialog.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            dialog.panel = dialog.canvas.transform.AddChildComponentIfNotExist<Image>("panel");
            var mvlg = dialog.panel.AddComponentIfNotExist<VerticalLayoutGroup>();
            mvlg.FixFullLayoutChildren();
            dialog.contentPanel = dialog.panel.AddChildComponentIfNotExist<Image>("contentPanel");
            dialog.icon = dialog.contentPanel.AddChildIfNotExist<Image>("icon");
            dialog.text = dialog.contentPanel.AddChildIfNotExist<TextMeshProUGUI>("text");
            var ctLg = dialog.contentPanel.AddComponentIfNotExist<VerticalLayoutGroup>();
            ctLg.FixFullLayoutChildren();
            //dialog.contentPanel.DockFull();
            dialog.foolterPanel = dialog.panel.AddChildComponentIfNotExist<Image>("foolterPanel");
            dialog.foolterPanel.DockBottom(120f);
            var hlg = dialog.foolterPanel.AddComponentIfNotExist<HorizontalLayoutGroup>();
            hlg.FixLayoutChildren();
            dialog.btnOk = dialog.foolterPanel.transform.AddButtonIfNotExist("btnOK", "OK");
            dialog.btnCancel = dialog.foolterPanel.transform.AddButtonIfNotExist("btnCandel", "Cancel");
            dialog.EditorSetDirty();
        }

        


#endif
    }
}