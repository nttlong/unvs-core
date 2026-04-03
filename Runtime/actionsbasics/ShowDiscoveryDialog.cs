using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;

namespace unvs.actionsbasics
{
    public class ShowDiscoveryDialog : ActionBase
    {

        public AudioInfo AlertSound;
        private UniTaskCompletionSource<bool> utcs;
        public bool ShowConform;
        
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await UniTask.Yield();
            var actor = Sender.GetTargetComponent<IActorObject>();
            var item = Sender.GetSourceComponent<IStoragableObject>();
            var alertSound=this.AlertSound;
            if (item != null)
            {
                // Setup UI ...
                GlobalApplication.UIDiscoveryDialog.Icon.sprite = item.Icon;
                GlobalApplication.UIDiscoveryDialog.Content.text = item.Description.IsValid()
                    ? item.Description.GetLocalizedString()
                    : $"{item.Name}.Description is null";

                // 1. initialize UTCS
                utcs = new UniTaskCompletionSource<bool>();

                // 2. Create local action
                Action onOk = () => utcs.TrySetResult(true);
                Action onCancel = () => utcs.TrySetResult(false);

                // 3. Event register
                GlobalApplication.UIDiscoveryDialog.OnOK += onOk;
                GlobalApplication.UIDiscoveryDialog.OnCancel += onCancel;

                try
                {
                    
                    if (ShowConform)
                    GlobalApplication.UIDiscoveryDialog.Show();
                    else 
                        GlobalApplication.UIDiscoveryDialog.ShowWithoutConfirm();

                    var audibleObject = Sender.GetSourceComponent<IAudiableObject>();
                    if (audibleObject != null && !audibleObject.DiscoverySound.IsEmpty())
                        alertSound = audibleObject.DiscoverySound;
                    alertSound.PlayBetterAudioClipAsync().Forget();
                    
                    // 4. IMPORTANT: wait until utcs return result
                    bool result = await utcs.Task;

                    if (!result)
                    {
                        Sender.Cancel();
                    }
                }
                finally
                {
                    // 5. Always cleanup all events
                    GlobalApplication.UIDiscoveryDialog.OnOK -= onOk;
                    GlobalApplication.UIDiscoveryDialog.OnCancel -= onCancel;
                    GlobalApplication.UIDiscoveryDialog.Hide();
                }
            }
        }

        
    }
}