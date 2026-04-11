//using Cysharp.Threading.Tasks;
//using System;
//using System.Threading.Tasks;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using unvs.actors;
//using unvs.ui;
//namespace unvs.interfaces
//{
//    public interface IDiscoveryDialog
//    {
//        Button OK { get; }
//        Button Cancel { get; }
//        float Width { get; }
//        float Height { get; }
//        Canvas DiscoveryDialogCanvas {  get; }
//        Image DiscoveryDialogPanel {  get; }
//        Image ConfirmPanel { get; }
//        Image Icon { get;}
//        TextMeshProUGUI Content { get;}

//        AudioSource AudioSource { get; }
//        AudioClip Sound { get; }
//        event Action OnOK;
//        event Action OnCancel;
//        UniTask DoShowDialogAsync(MonoBehaviour owner, MonoBehaviour source);
//        void Hide();
//        void Show();
//        void ShowWithoutConfirm();
//    }
//}

