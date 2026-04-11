//using Cysharp.Threading.Tasks.Triggers;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using unvs.ext;
//using unvs.gameword;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.ui
//{
//    [ExecuteInEditMode]
//    public class SettingsUIHub : MonoBehaviour, IUIHub
//    {
//        private Canvas hubCanvas;
//        private Image hubPanel;
//        public float size=-1;
//        public DockDirection dock= DockDirection.Bottom;

//        public Canvas HubCanvas => hubCanvas;

//        public Image HubPanel => hubPanel;

//        public float Size => size;

//        public DockDirection Dock => dock;

//        public void Hide()
//        {
//            if(this.hubCanvas != null)
//            {
//                this.hubCanvas.enabled = false;
//                this.hubCanvas.gameObject.SetActive(false);
//            } 
//        }

//        public void Show()
//        {
//            if (this.hubCanvas != null)
//            {
//                this.hubCanvas.enabled = true;
//                this.hubCanvas.gameObject.SetActive(true);
//            }
//        }

//        private void Awake()
//        {
//            hubCanvas = this.AddChildComponentIfNotExist<Canvas>(Constants.ObjectsConst.HUB_CANVAS);
//            hubPanel = hubCanvas.transform.AddChildComponentIfNotExist<Image>(Constants.ObjectsConst.HUB_PANEL);

//        }
//        private void Start()
//        {
//            if (Application.isPlaying) {
//                this.Hide();
//                this.HubCanvas.FullSize();
//                GlobalApplication.UIHub = this as IUIHub;

//                if (Commons.IsAndroid())
//                {
//                    if (this.size == -1)
//                    {
//                        this.size = Commons.GetScreenSize().y / 6;
//                    }
//                    this.HubPanel.DockTop(this.size);
//                } else
//                { 
//                    if (this.size == -1)
//                    {
//                        this.size = Commons.GetScreenSize().y / 9;
//                    }
//                    this.HubPanel.Dock(dock, this.size);
//                }
//            }
//        }
//    }
//}