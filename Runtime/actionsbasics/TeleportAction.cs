using Cysharp.Threading.Tasks;
using UnityEngine;
using unvs.actions;
using unvs.baseobjects;
using unvs.ext;
using unvs.interfaces;
using unvs.manager;
using unvs.shares;
namespace unvs.actionsbasics
{
    public class TeleportAction : unvs.actions.ActionBase
    {
        [SerializeField]
        public AudioClip OpenSound;
        [SerializeField]
        public AudioClip CloseSound;
        public async override UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            var teleportObject = Sender.Source.GetComponent<ITeleportPrefab>();
            if(teleportObject == null)
            {
                Sender.Cancel();
                return;
            }
            if (string.IsNullOrEmpty(teleportObject.PathToWord))
            {
               await Sender.Target.GetComponent<ISpeakableObject>().SayICanNotDoThatAsync();
                Sender.Target.GetComponent<IActorObject>().OnMoving += TeleportAction_OnMoving1;
                Sender.Cancel();
                return;
            }
            await teleportObject.OpenSound.PlayBetterAudioClipAsync(OpenSound);
            if (teleportObject.IsNew)
            {
                await GlobalApplication.SceneLoaderManagerInstance.LoadNewAsync(teleportObject.PathToWord, teleportObject.TargetName);

            } else
            {
                var fromScene=Sender.Source.GetComponentInParent<IScenePrefab>();
                await GlobalApplication.SceneLoaderManagerInstance.LoadInteriorAsync(teleportObject.PathToWord, teleportObject.TargetName, fromScene);
            }
            await teleportObject.CloseSound.PlayBetterAudioClipAsync(CloseSound);
        }

        private void TeleportAction_OnMoving1(IActorObject obj)
        {
            obj.Speaker.Off();
            obj.OnMoving -= TeleportAction_OnMoving1;
        }

       
    }
}

