using Cysharp.Threading.Tasks.Triggers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using unvs.ext;

namespace unvs.game2d.scenes.actors
{
    public class UnvsActirDialogue : UnvsUIComponentInstance<UnvsActirDialogue>
    {
        
        public Image panel;
        public TextMeshPro txt;
        public override void InitRunTime()
        {
            base.InitRunTime();
            this.canvas.FullSize();
        }
        public UnvsActirDialogue Show(Vector2 pos,string content)
        {
            this.Show();
            this.panel.SetPosition(pos);
            this.txt.text = content;
            return this;

        }
#if UNITY_EDITOR
        [UnvsButton()]
        public void Generate()
        {
            this.canvas = this.AddChildComponentIfNotExist<Canvas>("canvas");
            this.panel = this.canvas.transform.AddChildComponentIfNotExist<Image>("panel");
            this.panel.AddComponentIfNotExist<VerticalLayoutGroup>().FixFullLayoutChildren();
            this.txt=this.panel.AddChildComponentIfNotExist<TextMeshPro>("txt");
        }

        public override void InitEvents()
        {
            
        }
#endif
    }
}