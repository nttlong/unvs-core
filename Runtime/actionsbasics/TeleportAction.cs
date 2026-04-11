using Cysharp.Threading.Tasks;
using UnityEngine;
using unvs.actions;
using unvs.baseobjects;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.scenes;
using unvs.game2d.scenes.actors;
using unvs.interfaces;

using unvs.shares;
namespace unvs.actionsbasics
{
    public class TeleportAction : unvs.actions.ActionBase
    {
        [SerializeField]
        public AudioInfo OpenSound;
        [SerializeField]
        public AudioInfo CloseSound;
        public async override UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            var teleportObject = Sender.Source.GetComponent<UnvsTeleport>();
            if (teleportObject == null)
            {
                Sender.Cancel();
                return;
            }
            var actor = Sender.GetTargetComponent<UnvsActor>();
            if (actor == null)
            {
                Sender.Cancel();
                return;
            }
            if (string.IsNullOrEmpty(teleportObject.TargetPath))
            {
                actor.speaker.SayIThisDoesNotDoAnything();
                
                Sender.Cancel();
                return;
            }
            UnvsCinema.Instance.audioSource = new AudioSource();

            teleportObject.OpenSound.PlayBetterAudioClipAsync(OpenSound).Forget();
            if (teleportObject.IsNew)
            {
                await UnvsSceneLoader.Instance.LoadNewAsync(teleportObject.TargetPath, teleportObject.SpawnName);
                

            }
            else
            {
                var fromScene = Sender.Source.GetComponentInParent<UnvsScene>();
                await UnvsSceneLoader.Instance.LoadInteriorAsync(teleportObject.TargetPath, teleportObject.SpawnName, fromScene);
            }
            await teleportObject.CloseSound.PlayBetterAudioClipAsync(CloseSound);
        }

       


    }
}

