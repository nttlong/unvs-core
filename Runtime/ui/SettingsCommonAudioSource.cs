//using Cysharp.Threading.Tasks;
//using Cysharp.Threading.Tasks.Triggers;
//using System;
//using System.Collections;
//using System.Threading;
//using System.Threading.Tasks;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.Localization;
//using UnityEngine.Rendering;
//using UnityEngine.UI;
//using unvs.actions;
//using unvs.ext;
//
//using unvs.shares;
//namespace unvs.ui
//{
//    [RequireComponent(typeof(AudioSource))]
//    public class SettingsCommonAudioSource : MonoBehaviour
//    {
//        private void Awake()
//        {
//            if(Application.isPlaying)
//            {
//                GlobalApplication.CommonAudioSource=GetComponent<AudioSource>();
//            }
//        }
//    }
//}