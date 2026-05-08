using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using unvs.components;
using unvs.ext;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;

namespace unvs.ui
{
    public partial class UnvsUIInventory : UnvsUIComponentInstance<UnvsUIInventory>
    {
        public Image panel;
        public UnvsGrid grid;
        private UnvsBagger lastBagger;

        public override bool DisablePlayerInput => false;

        public override bool EnablePlayerInput => false;

        public override void InitEvents()
        {
           
        }
        public override void Show()
        {
            //_isShow=true;
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
            //_isShow = false;
        }
        //int _count = 0;
        //bool _isShow;
        public void Toggle(UnvsBagger bagger)
        {
            //UnvsApp.Instance.currentActor.SayText($"IsShow={IsShow},_count={_count}");
            //_count++;
            if (!IsShow)
            {
                LoadItems(bagger);
           
                Show();

            }
            else
            {
                Hide();
            }
            
        }

        private void LoadItems(UnvsBagger bagger)
        {
            
            this.grid.ClearAllItems();
            foreach (Transform t in bagger.bagger)
            {
                var sprite = t.GetFirstComponent<SpriteRenderer>(true);
                this.grid.AddSpriteToGrid(sprite.sprite, t.name);

            }
        }
    }
#if UNITY_EDITOR
    public partial class UnvsUIInventory : UnvsUIComponentInstance<UnvsUIInventory>
    {


        [UnvsButton("Generate Element")]
        public void EditorGenerateElements()
        {
            this.canvas = this.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            this.panel = this.canvas.transform.AddChildComponentIfNotExist<UnityEngine.UI.Image>("panel");
            this.grid = this.panel.AddComponentIfNotExist<UnvsGrid>();
            this.panel.AddComponentIfNotExist<UnvsDockPanel>();

        }
    }
#endif
}