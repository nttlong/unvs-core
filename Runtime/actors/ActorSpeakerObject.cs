//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.Localization;
//using UnityEngine.Localization.Settings;
//using UnityEngine.Localization.Tables;
//using unvs.ext;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.actors
//{
//    [ExecuteInEditMode]
//    public class ActorSpeakerObject : MonoBehaviour, ISpeakableObject
//    {
//        public LocalizedString msgThisDoesNotDoAnything;
//        public LocalizedString msgICanNotDoThat;

//        public LocalizedString MsgThisDoesNotDoAnything => msgThisDoesNotDoAnything;

       

//        public LocalizedString MsgICanNotDoThat => msgICanNotDoThat;

//        public Vector2 GetPosition()
//        {
//            var coll = GetComponent<Collider2D>();
//            if (coll == null) return Vector2.zero;
//            return new Vector2(coll.bounds.center.x, coll.bounds.max.y);
//        }

//        public void Off()
//        {
//            GlobalApplication.SpeakerController.Hide();
           
//        }

//        // Start is called before the first frame update
//        public void Say(LocalizedString message)
//        {

//            //var txt = "??????????????????????";
//            //if (message != null && !message.IsEmpty)
//             var   txt = message.GetText();
//            GlobalApplication.SpeakerController.Show(this.GetPosition(), txt);
//        }
//        public async UniTask SayAsync(LocalizedString msg, params LocalizedString[] p)
//        {
//            await UniTask.Yield();
          
//            Say(msg.GetFirstValid(p));
//        }

        
//        #region Default saying
//        public async UniTask SayIThisDoesNotDoAnythingAsync()
//        {
//            await UniTask.Yield();
//            this.Say(this.MsgThisDoesNotDoAnything);

//        }
//        public async UniTask SayICanNotDoThatAsync()
//        {
//            await UniTask.Yield();
//            this.Say(this.MsgICanNotDoThat);
//        }
        
//        #endregion
//        public void SayText(string v)
//        {
//            if(GlobalApplication.SpeakerController==null) return;
//            GlobalApplication.SpeakerController.Show(this.GetPosition(), v);
//        }
//        private void Start()
//        {
//#if UNITY_EDITOR
//            if (!msgThisDoesNotDoAnything.IsValid())
//            {
//                msgThisDoesNotDoAnything = new LocalizedString("Global", "ThisDoesNotDoAnything");

               
//            }
//            if (!msgICanNotDoThat.IsValid())
//            {
//                msgICanNotDoThat = new LocalizedString("Global", "ICanNotDoThat");


//            }
//#endif
//        }
//        private void OnValidate()
//        {
//            if (Application.isPlaying) return;
//            msgICanNotDoThat = new LocalizedString("Global", "ICanNotDoThat");
//            if (!msgThisDoesNotDoAnything.IsValid())
//            {
//                msgThisDoesNotDoAnything = new LocalizedString("Global", "ThisDoesNotDoAnything");
               
//                if (msgThisDoesNotDoAnything==null||msgThisDoesNotDoAnything.IsEmpty)
//                {
//                    throw new Exception  ($"key='ThisDoesNotDoAnything' was not found in 'Global'");
//                }

//#if UNITY_EDITOR
//                // Báo cho Unity biết là Object này đã bị thay đổi để nó hiện dấu * (cần Save)
//                UnityEditor.EditorUtility.SetDirty(this);
//#endif
//            }
//        }

        
//    }
//}