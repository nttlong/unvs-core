using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using game2d.scenes;
using System;
using System.IO;
using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes.actors;
using unvs.interfaces;
using unvs.shares;

namespace unvs.game2d.scenes
{
    public partial class UnvsSceneLoader : UnvsUIComponentInstance<UnvsSceneLoader>
    {
        public Transform chunks;
        public Transform buffer;
        public Transform bufferDelete;
        public Transform interior;
        public Transform backupInterior;
        public Transform actorContainer;
        private UnvsScene lastInteriorScene;

        public override void InitEvents()
        {
            this.buffer.gameObject.SetActive(false);
        }
        public async UniTask<UnvsScene> LoadInteriorAsync(string path, string spawnName, UnvsScene fromScene)
        {
            if (fromScene == null) return null;
            lastInteriorScene = fromScene;
            lastInteriorScene.transform.SetParent(this.backupInterior.transform, true);
            lastInteriorScene.TurnOffLeft().TurnOffRight();
            if(lastInteriorScene.leftScene != null)
            {
                lastInteriorScene.leftScene.rightScene = null;
            }
            if (lastInteriorScene.rightScene != null)
            {
                lastInteriorScene.rightScene.leftScene = null;
            }
            lastInteriorScene.leftScene = null;
            lastInteriorScene.rightScene = null;
            UnvsCinema.Instance.ClearWorlds();
            UnvsApp.Instance.CleanUp();
            this.interior.gameObject.SetActive(true);
            this.chunks.gameObject.SetActive(false);
            await this.clearChunsAsync();

            
            UnvsCinema.Instance.ClearWorlds();
            UnvsScene ret;
            ret = this.backupInterior.GetComponentInChildrenByName<UnvsScene>(path);
            if (ret == null)
            {
                ret = await Commons.LoadPrefabsAsync<UnvsScene>(path, this.buffer);
            }
            ret.transform.SetParent(this.interior.transform, true);

            UnvsCinema.Instance.UpdateWorld(ret,true);
            UnvsCinema.Instance.vcam.UpdateByUnvsScene(ret);

            UnvsActor actor = ret.GetActiveActor();
            if (actor != null)
            {
                this.validateCurrentActor(ret);
            }

            UnvsApp.Instance.currentActor.StandBy(ret.GetStartPosition(spawnName));
            UnvsCinema.Instance.vcam.Watch(UnvsApp.Instance.currentActor.camWatcher);

            UnvsCinema.Instance.UpdateMainCameraBoxCollider2dSize();
            UnvsApp.Instance.RaiseEnterScene(ret);
            UnvsCinema.Instance.vcam.UpdateOffset2D(ret.followOffset);
            CenterScene();
            ret.TurnOnLeft().TurnOnRight();
            ret.gameObject.SetActive(true);
           
            lastInteriorScene = ret;
            return ret;
        }
       
        public async UniTask<UnvsScene> LoadNewAsync(string path, string spawnName)
        {
            
            await this.clearAsync();
            this.interior.gameObject.SetActive(false);
            if (UnvsApp.Instance.currentActor != null)
            {
                UnvsApp.Instance.currentActor.gameObject.SafeDestroy();
            }
                UnvsCinema.Instance.ClearWorlds();
            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(path, this.buffer);
            ret.transform.SetParent(this.chunks.transform, true);

            UnvsCinema.Instance.UpdateWorld(ret,true);
            UnvsCinema.Instance.vcam.UpdateByUnvsScene(ret);

            UnvsActor actor = ret.GetActiveActor();
            if (actor != null)
            {
                actor.transform.SetParent(this.actorContainer.transform, true);
            }
           
            if (actor != null)
            {
                actor.StandBy(ret.GetStartPosition(spawnName));
                UnvsCinema.Instance.vcam.Watch(actor.camWatcher);
                UnvsApp.Instance.currentActor = actor;
            }

            UnvsCinema.Instance.UpdateMainCameraBoxCollider2dSize();
            UnvsApp.Instance.RaiseEnterScene(ret, true);
            CenterScene();
            ret.gameObject.SetActive(true);

            return ret;
        }
        public async UniTask<UnvsScene> LoadChunkLeftAsync(UnvsScene fromScene, string path)
        {
            
            this.clearChunkRightIfExeedeAsync().Forget();
            if (fromScene.transform.parent == this.interior)
            {
                fromScene.transform.SetParent(this.chunks.transform, true);
                interior.gameObject.SetActive(false);
                backupInterior.SafeDestroyChildrenAsync().Forget();
                this.chunks.gameObject.SetActive(true);
            }
            if (fromScene.transform.parent == this.interior)
            {
                fromScene.transform.SetParent(this.chunks.transform.parent, true);
            }
            fromScene.TurnOffLeft();


            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(path, this.buffer);
            ret.TurnOffRight();

            this.validateCurrentActor(ret);


            var offset = ret.JoinInfo.RightPos - fromScene.JoinInfo.LeftPos;


            ret.transform.SetParent(this.chunks.transform, false);
            ret.transform.position -= (Vector3)offset;
            ret.JoinInfo.LeftPos -= offset;
            ret.JoinInfo.RightPos -= offset;
            ret.transform.SetAsFirstSibling();
            UnvsCinema.Instance.UpdateWorld(ret,false);
            CenterScene();
            ret.gameObject.SetActive(true);
            fromScene.leftScene = ret;
            ret.rightScene = fromScene;
            //CenterScene();

            return ret;
        }

        private void CenterScene()
        {
          //  var center = UnvsCinema.Instance.worldBoundCollider2d.bounds.center;
           // UnvsApp.Instance.container.transform.position -= center;
            //if (chunks.GetComponentsInChildren<UnvsScene>().Length > 1)
            //{
            //var center = chunks.GetComponentsInChildren<UnvsScene>()[1];
            //var of = Vector2.zero - center;
            //UnvsCinema.Instance.vcam.PreviousStateIsValid = false;
            //UnvsApp.Instance.transform.position += center;
            //this.chunks.transform.position -= center;
            //this.actorContainer.transform.position -= center;
            //this.interior.transform.position -= center;
            //UnvsCinema.Instance.worldBoundCollider2d.transform.position -= center;
            //}
        }


        public async UniTask<UnvsScene> LoadChunkRightAsync(UnvsScene fromScene, string path)
        {

            this.clearChunkLeftIfExeedeAsync().Forget();
            if (fromScene.transform.parent == this.interior)
            {
                fromScene.transform.SetParent(this.chunks.transform, true);
                interior.gameObject.SetActive(false);
                backupInterior.SafeDestroyChildrenAsync().Forget();
                this.chunks.gameObject.SetActive(true);
            }
            fromScene.TurnOffRight();

            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(path, this.buffer);

            ret.TurnOffLeft();
            this.validateCurrentActor(ret);


            var offset = ret.JoinInfo.LeftPos - fromScene.JoinInfo.RightPos;

            ret.transform.SetParent(this.chunks.transform, false);
            ret.transform.position -= (Vector3)offset;
            ret.JoinInfo.LeftPos -= offset;
            ret.JoinInfo.RightPos -= offset;
          //  ret.transform.SetAsLastSibling();
            UnvsCinema.Instance.UpdateWorld(ret,false);
            ret.gameObject.SetActive(true);
            fromScene.rightScene = ret;
            ret.leftScene = fromScene;

            return ret;
        }
        private void validateCurrentActor(UnvsScene scene)
        {
            var actor = scene.GetActiveActor();

            if (actor == null || UnvsApp.Instance.currentActor == null) return;

            // Safe disable player logic if it exists
            var player = actor.GetComponent<UnvsPlayer>();
            if (player != null) player.enabled = false;

            if (actor.GetType() == UnvsApp.Instance.currentActor.GetType())
            {
                if (actor.name == UnvsApp.Instance.currentActor.name)
                {
                    (actor as MonoBehaviour).enabled = false;
                    (actor as MonoBehaviour).gameObject.SetActive(false);
                    (actor as MonoBehaviour).gameObject.SafeDestroyAsync().Forget();
                    return;
                }
            }
        }
        private async UniTask clearChunkLeftIfExeedeAsync()
        {
            var len = this.chunks.GetComponentsInChildren<UnvsScene>().Length;
            if (len >= UnvsApp.Instance.ChunLenght)
            {
                var deleteScene = this.chunks.GetComponentsInChildren<UnvsScene>()[0];
                if (deleteScene.rightScene != null)
                {
                    deleteScene.rightScene.TurnOnLeft();
                }
                deleteScene.transform.SetParent(this.bufferDelete.transform);
                await this.bufferDelete.SafeDestroyChildrenAsync();
            }
        }
        private async UniTask clearChunkRightIfExeedeAsync()
        {
            var len = this.chunks.GetComponentsInChildren<UnvsScene>().Length;
            if (len >= UnvsApp.Instance.ChunLenght)
            {
                var deleteScene = this.chunks.GetComponentsInChildren<UnvsScene>()[UnvsApp.Instance.ChunLenght - 1];
                if (deleteScene.leftScene != null)
                {
                    deleteScene.leftScene.TurnOnRight();
                }
                deleteScene.transform.SetParent(this.bufferDelete.transform);

                await this.bufferDelete.SafeDestroyChildrenAsync();
            }
        }
        private async UniTask clearChunsAsync()
        {
            
            for (var i = 0; i < this.chunks.GetComponentsInChildren<UnvsScene>().Length; i++)
            {
                this.chunks.GetComponentsInChildren<UnvsScene>()[i].transform.SetParent(this.bufferDelete.transform);
            }
            await this.bufferDelete.SafeDestroyChildrenAsync();
        }
        private async UniTask clearAsync()
        {
            if (lastInteriorScene != null)
            {
                lastInteriorScene.transform.SetParent(this.bufferDelete.transform);
            }
            for (var i = 0; i < this.chunks.GetComponentsInChildren<UnvsScene>().Length; i++)
            {
                this.chunks.GetComponentsInChildren<UnvsScene>()[i].transform.SetParent(this.bufferDelete.transform);
            }
            await this.bufferDelete.SafeDestroyChildrenAsync();
        }
    }
#if UNITY_EDITOR
    public partial class UnvsSceneLoader : UnvsUIComponentInstance<UnvsSceneLoader>
    {

        [UnvsButton("Generate")]
        public void Generate()
        {
            this.chunks = this.AddChildComponentIfNotExist<Transform>("chunks");
            this.buffer = this.AddChildComponentIfNotExist<Transform>("buffer");
            this.bufferDelete = this.AddChildComponentIfNotExist<Transform>("buffer-delete");
            this.bufferDelete.gameObject.SetActive(false);
            this.interior = this.AddChildComponentIfNotExist<Transform>("interior");
            this.buffer.gameObject.SetActive(false);
            this.actorContainer = this.AddChildComponentIfNotExist<Transform>("Actor-Container");
            this.backupInterior = this.AddChildComponentIfNotExist<Transform>("backup-Interior");
            this.backupInterior.gameObject.SetActive(false);

        }


    }





#endif
}
