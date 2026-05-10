using Script.unvs.ext;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using unvs.components;
using unvs.controllers.inputs;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;

namespace unvs.ui
{
    public partial class UnvsUIInventory : UnvsUIComponentInstance<UnvsUIInventory>
    {
        public Image panel;
        public UnvsGrid grid;
        public UnvsBagger lastBagger;
        

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
                lastBagger=bagger;
                lastBagger.OnAddItem += LastBagger_OnAddItem;
                Show();

            }
            else
            {
                Hide();
                lastBagger.OnAddItem -= LastBagger_OnAddItem;
                UnvsApp.SayOff();
            }
            
        }

        private void LastBagger_OnAddItem(GameObject obj)
        {
            LoadItems(lastBagger);
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
        private void LateUpdate()
        {
            var v = UnvsGlobalInput.LookAction.ReadValue<Vector2>();
            var panelRect = this.panel.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            panelRect.GetWorldCorners(corners);
            
            UnvsCollectableItem collectableItem = null;
            var rects=this.GetComponentsInChildren<UnvsDraggableItem>().Select(p=>p.GetComponent<RectTransform>()).ToArray();
            if(v.IsInner(rects,out var item))
            {
                
               
                if (lastBagger != null)
                {
                    var draggItem = item.GetComponent<UnvsDraggableItem>();
                   
                    if (draggItem != null)
                    {
                        collectableItem = lastBagger.FindItem(draggItem.name);
                       
                    }
                }
                if (collectableItem != null)
                {
                    UnvsInteractUI.Instance.Locked=true;
                    UnvsInteractUI.Instance.ChangeIcon(collectableItem.Icon);
                    UnvsGlobalInput.PlayerDisable();
                }
                else
                {
                    UnvsInteractUI.Instance.Locked = false;
                    UnvsInteractUI.Instance.RestoreDefaultIcon();
                    UnvsGlobalInput.PlayerEnable();
                }
            } else
            {
                UnvsInteractUI.Instance.Locked = false;
                UnvsGlobalInput.PlayerEnable();
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