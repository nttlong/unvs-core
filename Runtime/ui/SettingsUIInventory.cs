//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using Unity.VisualScripting;

//using UnityEngine;
//using UnityEngine.Localization;
//using UnityEngine.Rendering;
//using UnityEngine.UI;
//using unvs.actions;
//using unvs.ext;
//using unvs.interfaces;
//using unvs.shares;
//using static Unity.VisualScripting.Member;
//namespace unvs.ui
//{
//    [ExecuteInEditMode]
//    [RequireComponent(typeof(SpriteRenderer))]
//    [RequireComponent(typeof(SortingGroup))]
//    [RequireComponent(typeof(BoxCollider2D))]
//    public class SettingsUIInventory : MonoBehaviour, IInventoryController
//    {
        
//        public Image container;
//        public Canvas containerCanvas;
      
//        public DockDirection dockDirection=DockDirection.Bottom;
//        public float size=0;
//        private Dictionary<string, GameObject> storage=new Dictionary<string, GameObject>();
//        private HybridDragSystem dragSystem;
//        public Image bagger;
//        private Image interactContainer;

//        public GameObject Owner { get; set ; }

//        public Image Container => container;

//        public Image InteractContainer => interactContainer;

//        public Canvas ContainerCanvas => containerCanvas;

//        public DockDirection DockDirection => dockDirection;

//        public float Size => size;

//        public Dictionary<string, GameObject> Storage => storage;

//        public Image Bagger => bagger;

//        public HybridDragSystem DragSystem => dragSystem;

//        public bool Add(MonoBehaviour source)
//        {
//            var storagableItem = source.GetComponent<IStoragableObject>();
//            if(storagableItem ==null)  return false;
//            var img= interactContainer.transform.AddChildComponentIfNotExist<Image>(source.name);
//            img.sprite = storagableItem.Icon;
//            source.transform.SetParent(bagger.transform, false);
//            var actor=Owner.GetComponent<IActorObject>();
           
//            var item = source.GetComponent<IConsumableItem>();
//            if(item != null)
//            {
//                item.Owner=actor;
//            }
           
//            //dItem.enabled = true;
//            //source.transform.SetParent(this.container.transform);
//            return true;
//        }

//        public void Delete()
//        {
//            throw new System.NotImplementedException();
//        }
//        private void Awake()
//        {
//            containerCanvas=this.AddChildComponentIfNotExist<Canvas>(Constants.ObjectsConst.INVENTORY_CANVAS);
//            containerCanvas.AddComponentIfNotExist<GraphicRaycaster>();
//            // containerCanvas.AddComponent<HorizontalLayoutGroup>();
//            container =containerCanvas.transform.AddChildComponentIfNotExist<Image>(Constants.ObjectsConst.INVENTORY_PANEL);

//           interactContainer= container.transform.AddChildComponentIfNotExist<Image>(Constants.ObjectsConst.INVENTORY_PANEL_INTERACT);
//            interactContainer.raycastTarget = false;
//            container.raycastTarget = false;
//            //interactContainer la noi de cac image cho phep drag 
//            dragSystem=interactContainer.AddComponentIfNotExist<HybridDragSystem>();
//            bagger = transform.AddChildComponentIfNotExist<Image>(Constants.ObjectsConst.INVENTORY_PANEL_BAGGER);
//            bagger.gameObject.SetActive(false);
//            if (Application.isPlaying)
//            {
//                InitAtRunTime();
//                InitDragSystem();
//            }
//        }

//        private void InitDragSystem()
//        {
//            dragSystem.OnDragBegin = p =>
//            {
//                //var storageableItem = bagger.GetComponentInChildrenByName<IStoragableObject>(p.Source.name);
//                //if (storageableItem == null) return;
//                //p.CloneObject(transform, storageableItem.Icon, storageableItem.Name,120);
               
//            };
//            dragSystem.OnDragging = p =>
//            {
//                if(p.DraggingVisual==null)
//                {
//                    var storageableItem = bagger.GetComponentInChildrenByName<IStoragableObject>(p.Source.name);
//                    if (storageableItem == null) return;
//                    p.CloneObject(this.containerCanvas, storageableItem.Icon, storageableItem.Name, 120);
//                }
//                p.Move();
               


               
//            };
//            dragSystem.OnDrop = p =>
//            {
//                var target = Vector2dExtesion.GetHitCollider<IConsumerObject>(p.Pos);

//                if (target != null)
//                {

                    
                    
//                    var actor = Owner.GetComponent<IActorObject>();
//                    var consumeItem = bagger.GetComponentInChildrenByName<IConsumableItem>(p.Source.name);
//                    consumeItem.Owner.Cts = consumeItem.Owner.Cts.Refresh();
//                    target.ConsumeDefinintion.ObjectsExecAsync(target, consumeItem, consumeItem.Owner.Cts).Forget();
//                    return;
//                }
//            };
//        }

//        private void InitAtRunTime()
//        {
//            if(size==0)
//            {
//                if (dockDirection == DockDirection.Bottom || dockDirection == DockDirection.Top)
//                {
//                    size = Commons.GetScreenSize().y / 8;
//                    container.AddComponentIfNotExist<VerticalLayoutGroup>();
//                }
//                if (dockDirection == DockDirection.Left || dockDirection == DockDirection.Right)
//                {
//                    size = Commons.GetScreenSize().x / 8;
//                    container.AddComponentIfNotExist<HorizontalLayoutGroup>();
                    



//                }
//            }
//            containerCanvas.FullSize();
//            container.Dock(this.DockDirection, size);
//            GetComponent<BoxCollider2D>().isTrigger=true;

//        }
//    }
//}