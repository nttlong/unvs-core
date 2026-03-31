using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using UnityEngine.UI;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.ui
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SettingsUIInventory : MonoBehaviour, IInventoryController
    {
        
        public Image container;
        public Canvas containerCanvas;
      
        public DockDirection dockDirection=DockDirection.Bottom;
        public float size=0;

        public GameObject Owner { get; set ; }

        public Image Container => container;

        public IDragAndDropContainer InteractContainer => throw new System.NotImplementedException();

        public Canvas ContainerCanvas => containerCanvas;

        public DockDirection DockDirection => dockDirection;

        public float Size => size;

        

        public bool Add(MonoBehaviour source)
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }
        private void Awake()
        {
            containerCanvas=this.AddChildComponentIfNotExist<Canvas>(Constants.ObjectsConst.INVENTORY_CANVAS);
           // containerCanvas.AddComponent<HorizontalLayoutGroup>();
            container =containerCanvas.transform.AddChildComponentIfNotExist<Image>(Constants.ObjectsConst.INVENTORY_PANEL);

            container.transform.AddChildComponentIfNotExist<DragDropContainer>(Constants.ObjectsConst.INVENTORY_PANEL_INTERACT);
            if (Application.isPlaying)
            {
                InitAtRunTime();
            }
        }

        private void InitAtRunTime()
        {
            if(size==0)
            {
                if (dockDirection == DockDirection.Bottom || dockDirection == DockDirection.Top)
                {
                    size = Commons.GetScreenSize().y / 8;
                    container.AddComponentIfNotExist<VerticalLayoutGroup>();
                }
                if (dockDirection == DockDirection.Left || dockDirection == DockDirection.Right)
                {
                    size = Commons.GetScreenSize().x / 8;
                    container.AddComponentIfNotExist<HorizontalLayoutGroup>();
                    



                }
            }
            containerCanvas.FullSize();
            container.Dock(this.DockDirection, size);
            GetComponent<BoxCollider2D>().isTrigger=true;

        }
    }
}