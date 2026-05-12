using game2d.scenes;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using unvs.ext;
using unvs.components;
using unvs.game2d.objects.editor;
using unvs.shares;
using unvs.ui;
using unvs.game2d.scenes;
using unvs.controllers.inputs;

namespace unvs.ui
{
    [RequireComponent(typeof(AudioSource))]
    public class UnvsMainMenu : UnvsUIComponent
    {
       
        public Image panel;
        public Button btnStart;
        public Button btnExit;
        public Action OnStart;
        public Action OnExit;

        public override bool DisablePlayerInput => true;

        public override bool EnablePlayerInput => true;

        public override void InitEvents()
        {
            //UnvsGlobalInput.OnUIInputReady += () =>
            //{
            //    UnvsGlobalInput.NewMapUIAction(this, "Click", action =>
            //    {
            //        action.started += ctx =>
            //        {
            //            base.ApplyNaviagatorButtons();
            //        };
            //    });
            //};
            btnStart.onClick.AddListener(() =>
            {
                OnStart?.Invoke();
            });
            btnExit.onClick.AddListener(() =>
            {
                OnExit?.Invoke();
            });
        }
        public override void Show()
        {
           
            this.AudioOpen.Play(this.GetComponent<AudioSource>());
            base.Show();
            base.ApplyNaviagatorButtons();

        }
        public override void Hide()
        {
            base.Hide();
            this.AudioClose.Play(this.GetComponent<AudioSource>());
        }
        public override void InitRunTime()
        {
            
            this.canvas.UIFullSize();
            this.panel.DockFull();
            //GameObjectExtension.ApplyNavigate<Button>(this.gameObject);
        }
#if UNITY_EDITOR
        [UnvsButton]
        public void Generate()
        {

            this.canvas = this.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            this.panel = this.canvas.transform.AddChildComponentIfNotExist<Image>("panel");
            var v = this.panel.AddComponentIfNotExist<VerticalLayoutGroup>();
            v.FixLayoutChildren();
            this.btnStart = this.panel.transform.AddButtonIfNotExist("btnStart", "Start");
            this.btnExit = this.panel.transform.AddButtonIfNotExist("btnExit", "Exit");
            this.btnExit.onClick.AddListener(() =>
            {
                UnvsApp.Instance.ExitGame();
            });


        }

        
#endif
    }
}