
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
        public TextMeshProUGUI txt;
        [SerializeField]
        public Vector2 Size;

        public override void InitRunTime()
        {
            base.InitRunTime();
            //this.canvas.FullSize();
            //this.canvas.enabled = false;
        }
        public UnvsActirDialogue Show(Vector2 pos,string content)
        {
            this.Show();
            pos = pos.ToScreen();
            
            var v = pos - this.Size / 2;
            this.panel.SetPosition(new Vector2(v.x,pos.y));
            this.txt.text = content;
            return this;

        }
        public override void InitEvents()
        {

        }
#if UNITY_EDITOR
        [UnvsButton()]
        public void Generate()
        {
            this.canvas = this.AddChildComponentIfNotExist<Canvas>("canvas");
            this.panel = this.canvas.transform.AddChildComponentIfNotExist<Image>("panel");
            this.panel.AddComponentIfNotExist<VerticalLayoutGroup>().FixFullLayoutChildren();
            this.txt=this.panel.AddChildComponentIfNotExist<TextMeshProUGUI>("txt");
        }
        public void OnDrawGizmos()
        {
            this.Size=this.panel.GetSize();
        }

#endif
    }
}