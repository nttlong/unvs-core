using Cysharp.Threading.Tasks;
using game2d.ext;
using System;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using unvs.actor.player;
using unvs.controllers.inputs;
using unvs.ext;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;
using unvs.shares;

namespace unvs.ui {
    [RequireComponent(typeof(AudioSource))]
    public class UnvsDialog : UnvsUIComponentInstance<UnvsDialog>
    {
        public UniTaskCompletionSource DialogShowingTask;
        public Image panel;
        public Image contentPanel;
        public Image foolterPanel;
        public UnityEngine.UI.Image icon;
        public TextMeshProUGUI text;
        public Button btnOk;
        public Button btnCancel;
        private MapAction CloseAny;
        [SerializeField]
        public Vector2 Size;

        public override bool DisablePlayerInput => true;

        public override bool EnablePlayerInput => true;
        
        public override void InitEvents()
        {
            UnvsGlobalInput.OnUIInputReady += () =>
            {
                this.CloseAny = UnvsGlobalInput.NewMapUIAction(this, "CloseAny", action =>
                {
                    action.started += ctx =>
                    {
                        this.Hide();
                    };
                });
            };
           
        }

        public override void InitRunTime()
        {
            base.InitRunTime();
            canvas.UIFullSize();
        }
        public override void Show()
        {
            DialogShowingTask = new UniTaskCompletionSource();
            var screenSize=Commons.GetUIScreenSize();
            var x = (screenSize - this.panel.GetSize()) / 2;
            this.panel.SetPosition(x,this.Size);
            UnvsActorDialogue.Instance.Hide();
            UnvsInteractUI.Instance.HideInteracIconOfItem();
            base.Show();
        }
        public void HideFooter()
        {
            this.foolterPanel.Hide();
        }
        public override void Hide()
        {
            
            base.Hide();
            this.foolterPanel.Show();
            DialogShowingTask?.TrySetResult();
        }
        public UniTask DoReviewItemAsync(Sprite iconSprite, LocalizedString description, params types.AudioInfo[] feedBack)
        {
            var audio = this.AudioOpen;
            audio = audio.GetBetter(feedBack);
            icon.sprite = iconSprite;
            if(description!=null && !description.IsEmpty)
            {
                this.text.text = description.ToString();
            } else
            {
                this.text.text = "?????";
            }
            audio.Play(this.GetComponent<AudioSource>());
            Show();
            return DialogShowingTask.Task;
        }

#if UNITY_EDITOR
        [UnvsButton()]
        public  void Generate()
        {
            var dialog = this;
            dialog.canvas = dialog.AddChildChildCanvasWithGraphicRaycasterIfNotExist("canvas");
            dialog.panel = dialog.canvas.transform.AddChildComponentIfNotExist<Image>("panel");
            //var mvlg = dialog.panel.AddComponentIfNotExist<VerticalLayoutGroup>();
            //mvlg.FixFullLayoutChildren();
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

        private void OnValidate()
        {
            this.Size=this.panel.GetSize();
        }




#endif
    }
}