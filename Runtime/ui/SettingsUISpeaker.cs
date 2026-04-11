//using Cysharp.Threading.Tasks.Triggers;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using unvs.ext;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.ui
//{
//    [ExecuteInEditMode]
//    public class SettingsUISpeaker : MonoBehaviour, IUISpeakerController
//    {
//        public Image speakerPanel;
//        public Canvas speakerCanvas;
//        public TextMeshProUGUI text;
//        public float width = 400f;
//        public float height=300f;
        

//        public Image SpeakerPanel => speakerPanel;

//        public Canvas SpeakerCanvas => speakerCanvas;

//        public TextMeshProUGUI Text => text;

//        public float Width => width;

//        public float Height => height;

      

//        public void Hide()
//        {
//            speakerCanvas.enabled=false;
//            speakerCanvas.gameObject.SetActive(false);

//        }

//        public void Show(Vector2 pos, string txt)
//        {
//            speakerCanvas.enabled = true;
//            speakerCanvas.gameObject.SetActive(true);
            
//            text.text = txt;
//            var xPos = pos.ToScreen();
//            speakerPanel.SetPosition(xPos.x- width/2, xPos.y,width,height);
//        }
//        private void Awake()
//        {
//            speakerCanvas = this.AddChildComponentIfNotExist<Canvas>("SpeadkerCanvas");
//            speakerCanvas.AddComponentIfNotExist<GraphicRaycaster>();
//            speakerPanel = speakerCanvas.transform.AddChildComponentIfNotExist<Image>("speakerPanel");
//            text= speakerPanel.transform.AddChildComponentIfNotExist<TextMeshProUGUI>("Text");
//        }
//        private void Start()
//        {
//            if(Application.isPlaying)
//            {
//                GlobalApplication.SpeakerController = this;
//                this.SpeakerCanvas.FullSize();
//                this.speakerPanel.SetPosition(0,0,width,height);
//            }
            
//        }
//    }
//}