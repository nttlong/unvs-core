using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.ui
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    public class SettingsUIDiscoveryDialog : MonoBehaviour, IDiscoveryDialog
    {
        public AudioSource audioSource;
        [SerializeField]
        public AudioClip sound;
        public Canvas discoveryDialogCanvas;
        public Image discoveryDialogPanel;
        public Image icon;
        public TextMeshProUGUI content;

        public AudioSource AudioSource => audioSource;

        public AudioClip Sound => sound;

        public Canvas DiscoveryDialogCanvas => discoveryDialogCanvas;

        public Image DiscoveryDialogPanel => discoveryDialogPanel;

        public Image Icon => icon;

        public TextMeshProUGUI Content => content;

        public UniTask DoShowDialogAsync(MonoBehaviour owner, MonoBehaviour source)
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            this.DiscoveryDialogCanvas.enabled = false;
            this.DiscoveryDialogCanvas.gameObject.SetActive(false);
        }

        public void Show()
        {
            this.DiscoveryDialogCanvas.enabled = true;
            this.DiscoveryDialogCanvas.gameObject.SetActive(true);
        }

        private void Awake()
        {
            audioSource=GetComponent<AudioSource>();
            discoveryDialogCanvas = this.AddChildComponentIfNotExist<Canvas>("DiscoveryDialogCanvas");
            discoveryDialogPanel = discoveryDialogCanvas.transform.AddChildComponentIfNotExist<Image>("DiscoveryDialogPanel");
            icon = this.GetComponentInChildrenByName<Image>("Icon");
            content = this.GetComponentInChildrenByName<TextMeshProUGUI>("content");
        }
        private void Start()
        {
            if (Application.isPlaying)
            {
                icon = this.GetComponentInChildrenByName<Image>("Icon");
                content = this.GetComponentInChildrenByName<TextMeshProUGUI>("content");
                if (icon == null) throw new Exception($"Image Icon was not found in {name}");
                if (content == null) throw new Exception($"Content Icon was not found in {name}");
                GlobalApplication.UIDiscoveryDialog = this as IDiscoveryDialog;
                this.Hide();
            }
        }
    }
}