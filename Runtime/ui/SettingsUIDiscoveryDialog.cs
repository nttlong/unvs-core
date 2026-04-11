//using Cysharp.Threading.Tasks;
//using Cysharp.Threading.Tasks.Triggers;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using unvs.ext;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.ui
//{
//    [ExecuteInEditMode]
    
//    public class SettingsUIDiscoveryDialog : MonoBehaviour, IDiscoveryDialog
//    {
//        public AudioSource audioSource;
//        [SerializeField]
//        public AudioClip sound;
//        public Canvas discoveryDialogCanvas;
//        public Image discoveryDialogPanel;
//        public Image icon;
//        public TextMeshProUGUI content;
//        public float width = 600;
//        public float height = 400;
//        public Button btnOk;
//        public Button btnCancel;
//        private bool isConfirm;
//        private Image confirmPanel;

//        public AudioSource AudioSource => audioSource;

//        public AudioClip Sound => sound;

//        public Canvas DiscoveryDialogCanvas => discoveryDialogCanvas;

//        public Image DiscoveryDialogPanel => discoveryDialogPanel;

//        public Image Icon => icon;

//        public TextMeshProUGUI Content => content;

//        public float Width => width;

//        public float Height => height;

//        public Button OK => btnOk;

//        public Button Cancel => btnCancel;

//        public Image ConfirmPanel => confirmPanel;

//        public event Action OnOK;
//        public event Action OnCancel;

//        public UniTask DoShowDialogAsync(MonoBehaviour owner, MonoBehaviour source)
//        {
//            throw new NotImplementedException();
//        }

//        public void Hide()
//        {
            
//            isConfirm = true;
         
            
           
//            this.DiscoveryDialogCanvas.DoDeactive();
//        }

//        public void Show()
//        {
//            this.DiscoveryDialogCanvas.DoActive();
//            confirmPanel.gameObject.SetActive(true);
//            this.DiscoveryDialogCanvas.enabled = true;
//            this.DiscoveryDialogCanvas.gameObject.SetActive(true);
            
//            this.DiscoveryDialogPanel.ShowAtCenter(width,height);
            
//            EventSystem.current.SetSelectedGameObject(btnOk.gameObject);
//            if(this.sound!=null) this.audioSource.PlayOneShot(this.sound);
//            Time.timeScale = 0f;
            
//        }
//        private void Reset()
//        {
//            buildDefaultUITempalte();
//        }
//        void buildDefaultUITempalte()
//        {
//            discoveryDialogCanvas = this.AddChildChildCanvasWithGraphicRaycasterIfNotExist("DiscoveryDialogCanvas");
//            discoveryDialogPanel = discoveryDialogCanvas.transform.AddChildComponentIfNotExist<Image>("DiscoveryDialogPanel");
//            var topPanel = discoveryDialogPanel.AddChildComponentIfNotExist<Image>("top-panel");
//            topPanel.DockTop(300);
//            topPanel.AddComponentIfNotExist<VerticalLayoutGroup>();
//            topPanel.AddChildIfNotExist<Image>("Icon");
//            topPanel.AddChildIfNotExist<TextMeshProUGUI>("content");
//            confirmPanel = discoveryDialogPanel.AddChildComponentIfNotExist<Image>("ConfirmPanel");
//            confirmPanel.AddComponentIfNotExist<HorizontalLayoutGroup>().FixLayoutChildren();

//            btnOk = confirmPanel.transform.AddButtonIfNotExist("btnOK","OK");
            
//            btnCancel = confirmPanel.transform.AddButtonIfNotExist("btnCancel", "Cancel"); 
            
//        }
//        private void Awake()
//        {
//            audioSource=GetComponent<AudioSource>();
//            discoveryDialogCanvas = this.AddChildChildCanvasWithGraphicRaycasterIfNotExist("DiscoveryDialogCanvas");
//            discoveryDialogPanel = discoveryDialogCanvas.transform.AddChildComponentIfNotExist<Image>("DiscoveryDialogPanel");
//            icon = this.GetComponentInChildrenByName<Image>("Icon");
//            content = this.GetComponentInChildrenByName<TextMeshProUGUI>("content");
//            btnOk=this.GetComponentInChildrenByName<Button>("btnOK");
//            btnCancel = this.GetComponentInChildrenByName<Button>("btnCancel");
//            confirmPanel = this.GetComponentInChildrenByName<Image>("ConfirmPanel");
            
//        }
//        private void Start()
//        {
//            if (Application.isPlaying)
//            {
//                icon = this.GetComponentInChildrenByName<Image>("Icon");
//                content = this.GetComponentInChildrenByName<TextMeshProUGUI>("content");
//                if (icon == null) throw new Exception($"Image Icon was not found in {name}");
//                if (content == null) throw new Exception($"Content Icon was not found in {name}");
//                if (btnOk == null) throw new Exception($"Button OK was not found in {name}, please create button name btnOK");
//                if (btnCancel == null) throw new Exception($"Button Cancel was not found in {name}, please create button name btnCancel");
//                if(confirmPanel==null) throw new Exception($"Panel  'ConfirmPanel' was not found in {name}, please create Panel (name is 'confirmPanel') and place 2 buttons there");
//                GlobalApplication.UIDiscoveryDialog = this as IDiscoveryDialog;
//                this.DiscoveryDialogCanvas.FullSize();
//                initEvents();
//                this.Hide();
//            }
//        }

//        private void initEvents()
//        {
//            btnOk.onClick.AddListener(() => {
//                OnOK?.Invoke();
//                Hide();
//            });

//            btnCancel.onClick.AddListener(() => {
//                OnCancel?.Invoke();
//                Hide();
//            });
//        }

//        public void ShowWithoutConfirm()
//        {
//            confirmPanel.gameObject.SetActive(false); ;
//            isConfirm=false;
//            this.DiscoveryDialogCanvas.enabled = true;
//            this.DiscoveryDialogCanvas.gameObject.SetActive(true);
//            this.DiscoveryDialogCanvas.FullSize();
//            this.DiscoveryDialogPanel.ShowAtCenter(width, height);
//            GlobalApplication.GlobalInput.Player.enable = false;
//            EventSystem.current.SetSelectedGameObject(btnOk.gameObject);
//            if (this.sound != null) this.audioSource.PlayOneShot(this.sound);
//            Time.timeScale = 0f;
//        }

//        private void Update()
//        {
//            if (Application.isPlaying && discoveryDialogCanvas.enabled && !isConfirm)
//            {

//                if (GlobalApplication.GlobalInput.UI.Click.triggered || GlobalApplication.GlobalInput.UI.Submit.triggered)
//                {
//                    OnOK?.Invoke();
//                    Hide();
//                }
//                if (GlobalApplication.GlobalInput.UI.Cancel.triggered)
//                {
//                    OnCancel?.Invoke();
//                    Hide();
//                }
//            }
//        }
//    }
//}