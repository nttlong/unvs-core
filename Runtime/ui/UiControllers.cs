//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections;
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
//namespace unvs.ui {
//    public abstract class UIEventController : MonoBehaviour
//    {
//        public event Action CancelStarted;
//        public event Action PauseStarted;
//        public event Action PauseCanceled;
//        public void ActionUICancelStarted()
//        {
//            CancelStarted?.Invoke();
//        }

//        public void ActionUIPauseCanceled()
//        {
//            PauseCanceled?.Invoke();
//        }

//        public void ActionUIPauseStarted()
//        {
//            PauseStarted?.Invoke();
//        }
//        private void Awake()
//        {
//            OnInitUIController();
//            GlobalApplication.UIEventControllerInstance = this;
//        }

        

//        private void OnEnable()
//        {
//            OnEnableController();
//        }
//        private void OnDisable()
//        {
//            OnDisableController();
//        }

//        public abstract void OnDisableController();


//        public abstract void OnEnableController();
//        public abstract void OnInitUIController();

//        public abstract void SwithUI();
//    }
//}