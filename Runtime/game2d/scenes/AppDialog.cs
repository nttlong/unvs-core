using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.scenes;

namespace game2d.scenes {
    public class AppDialog : AppUIElemen<AppDialog>
    {
        public Canvas canvas;
        public Image panel;
        public Image contentPanel;
        public Image foolterPanel;
        public Image icon;
        public TextMeshProUGUI text;
        public Button btnOk;
        public Button btnCancel;

        

        public override void InitRunTime()
        {
            Instance = this.GetComponent<AppDialog>();
            canvas.FullSize();
        }
    }
}