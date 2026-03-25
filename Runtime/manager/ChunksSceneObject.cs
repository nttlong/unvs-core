using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using unvs.ext;
using unvs.gameword;
using unvs.interfaces;
using unvs.interfaces.sys;

namespace unvs.manager
{
    public class ChunksSceneObject : MonoBehaviour, IChunkScenes
    {
        public async UniTask CheckLeftAsync(Transform tempDelete, int chunkSize)
        {
            await UniTask.Yield();
            var scenes=this.GetComponentsInChildren<IScenePrefab>();
            if (scenes.Length >= chunkSize)
            {
                if (scenes[0].Right != null)
                {
                    scenes[0].Right.LeftTriggerZone.On();
                    scenes[0].Right.LeftWall.gameObject.SetActive(true);
                }
                var wb = GlobalWorldBound.Instance.FindByOwner(scenes[0]);
                if (wb != null)
                {
                    (wb as MonoBehaviour).transform.SetParent(scenes[0].GoWorld.transform);
                }
               
                scenes[0].GoWorld.transform.SetParent(tempDelete.transform, false);
                tempDelete.SafeDestroyChildrenAsync().Forget();
            }


        }

        public async UniTask CheckRightAsync(Transform tempDelete, int chunkSize)
        {
            await UniTask.Yield();
            var scenes = this.GetComponentsInChildren<IScenePrefab>();
            if (scenes.Length >= chunkSize)
            {
                if (scenes[scenes.Length-1].Left != null)
                {
                    scenes[scenes.Length - 1].Left.RightTriggerZone.On();
                    scenes[scenes.Length - 1].Left.RightWall.gameObject.SetActive(true);
                }
                var wb = GlobalWorldBound.Instance.FindByOwner(scenes[scenes.Length - 1]);
                if (wb != null)
                {
                    (wb as MonoBehaviour).transform.SetParent(scenes[scenes.Length - 1].GoWorld.transform);
                }
                scenes[scenes.Length - 1].GoWorld.transform.SetParent(tempDelete.transform, false);
                tempDelete.SafeDestroyChildrenAsync().Forget();
            }
        }
    }
}