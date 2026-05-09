using Cysharp.Threading.Tasks;
using game2d.objects;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using unvs.actions;

using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.scenes;
using unvs.game2d.actors;


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

            if (Sender.Source.IsDestroyed())
            {
                Sender.Cancel();
                return;
            }
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
                actor.speaker?.SayIThisDoesNotDoAnything();

                Sender.Cancel();
                return;
            }
            UnvsCinema.Instance.audioSource = new AudioSource();

            teleportObject.OpenSound.PlayBetterAudioClipAsync(OpenSound).Forget();
            if (teleportObject.TeleportType == types.TeleportType.NewScene)
            {
                await UnvsSceneLoader.Instance.LoadNewAsync(teleportObject.Target, teleportObject.SpawnName, false);


            }
            else if (teleportObject.TeleportType == types.TeleportType.Interior)
            {
                var fromScene = Sender.Source.GetComponentInParent<UnvsScene>();
                await UnvsSceneLoader.Instance.LoadInteriorAsync(teleportObject.Target, teleportObject.SpawnName, fromScene);
            }
            else if (teleportObject.TeleportType == types.TeleportType.TempScene)
            {
                await UnvsSceneLoader.Instance.LoadTempSceneAsync(teleportObject.Target, teleportObject, teleportObject.SpawnName);
            }
            else if (teleportObject.TeleportType == types.TeleportType.ReturnToScene)
            {
                await UnvsSceneLoader.Instance.ReturnFromTempScene(teleportObject,teleportObject.SpawnName);
            }

            await teleportObject.CloseSound.PlayBetterAudioClipAsync(CloseSound);
        }




    }
    public class WaitingCameraFinish : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            if (UnvsCinema.Instance != null)
            {
                if (UnvsCinema.Instance.CamChangeFollowOffsetTask.Status == UniTaskStatus.Pending)
                {
                    await UnvsCinema.Instance.CamChangeFollowOffsetTask;
                }
            }
        }
    }
    public class TransitionAction : ActionBase
    {
        public override async UniTask ExecuteAsync(ActionBaseSender Sender)
        {
            await UniTask.Yield();
            var tras = Sender.GetSourceComponent<UnvsTransitionable>();
            if (tras == null)
            {
                var pro = Sender.Source.GetType().GetFields().FirstOrDefault(p => p.FieldType == typeof(UnvsTransitionable));
                if (pro == null)
                {
                    Sender.Cancel();
                    return;
                }
                tras = pro.GetValue(Sender.Source) as UnvsTransitionable;
            }
            if (tras == null)
            {
                Sender.Cancel();
                return;
            }
            UnvsTransitionDefinitionsExt.PlayTransition(tras.Transition, tras);

        }
    }
}

