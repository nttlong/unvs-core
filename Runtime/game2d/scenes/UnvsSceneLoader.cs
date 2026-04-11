using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using game2d.scenes;
using System;
using System.IO;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes.actors;
using unvs.interfaces;
using unvs.shares;

namespace unvs.game2d.scenes
{
    public class UnvsSceneLoader : UnvsUIComponentInstance<UnvsSceneLoader>
    {
        public Transform chunks;
        public Transform buffer;
        public Transform bufferDelete;
        private Transform interior;
        public Transform actorContainer;
        

        public override void InitEvents()
        {
            this.buffer.gameObject.SetActive(false);
        }
        public async UniTask<UnvsScene> LoadNewAsync(string path)
        {
            await this.clearAsync();
            var ret= await Commons.LoadPrefabsAsync<UnvsScene>(path,this.buffer);
            ret.transform.SetParent(this.chunks.transform,true);

            UnvsCinema.Instance.UpdateWorld(ret);
            UnvsCinema.Instance.vcam.UpdateByUnvsScene(ret);
            
            UnvsActor actor = ret.GetActiveActor();
            if(actor != null)
            {
                actor.transform.SetParent(this.actorContainer.transform,true);
            }
            ret.gameObject.SetActive(true);
            if(actor != null)
            {
                actor.StandBy(ret.GetStartPosition());
                UnvsCinema.Instance.vcam.Watch(actor.camWatcher);
                UnvsApp.Instance.currentActor = actor;
            }
          
            UnvsCinema.Instance.UpdateMainCameraBoxCollider2dSize();
            UnvsApp.Instance.RaiseEnterScene(ret);
           
            return ret;
        }
        public async UniTask<UnvsScene> LoadChunkLeftAsync(UnvsScene fromScene,string path)
        {
            this.clearChunkRightIfExeedeAsync().Forget();
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
            UnvsCinema.Instance.UpdateWorld(ret);
            ret.gameObject.SetActive(true);
            fromScene.leftScene = ret;
            ret.rightScene = fromScene;
            CenterScene();

            return ret;
        }

        private void CenterScene()
        {
            if (chunks.GetComponentsInChildren<UnvsScene>().Length > 1)
            {
                var center = chunks.GetComponentsInChildren<UnvsScene>()[1];
                var of = Vector2.zero - (Vector2)center.transform.position;
                this.chunks.transform.position = of;
                this.actorContainer.transform.position = of;
            }
        }


        public async UniTask<UnvsScene> LoadChunkRightAsync(UnvsScene fromScene, string path)
        {
            
            this.clearChunkLeftIfExeedeAsync().Forget();
            fromScene.TurnOffRight();

            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(path, this.buffer);

            ret.TurnOffLeft();
            this.validateCurrentActor(ret);
            
            
            var offset = ret.JoinInfo.LeftPos - fromScene.JoinInfo.RightPos;
            
            ret.transform.SetParent(this.chunks.transform, false);
            ret.transform.position -= (Vector3)offset;
            ret.JoinInfo.LeftPos -= offset;
            ret.JoinInfo.RightPos -= offset;
            ret.transform.SetAsLastSibling();
            UnvsCinema.Instance.UpdateWorld(ret);
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
            if (len >= 3)
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
            if (len >= 3)
            {
                var deleteScene = this.chunks.GetComponentsInChildren<UnvsScene>()[len - 1];
                if (deleteScene.leftScene != null)
                {
                    deleteScene.leftScene.TurnOnRight();
                }
                deleteScene.transform.SetParent(this.bufferDelete.transform);
               
                await this.bufferDelete.SafeDestroyChildrenAsync();
            }
        }

        private async UniTask clearAsync()
        {
           await  this.chunks.SafeDestroyChildrenAsync();
        }
        
#if UNITY_EDITOR
        [UnvsButton("Generate")]
        public void Generate()
        {
            this.chunks = this.AddChildComponentIfNotExist<Transform>("chunks");
            this.buffer = this.AddChildComponentIfNotExist<Transform>("buffer");
            this.bufferDelete = this.AddChildComponentIfNotExist<Transform>("buffer-delete");
            this.bufferDelete.gameObject.SetActive(false);
            this.interior = this.AddChildComponentIfNotExist<Transform>("interior");
            this.buffer.gameObject.SetActive(false);
            this.actorContainer= this.AddChildComponentIfNotExist<Transform>("Actor-Container");
            
        }

        




#endif
    }
}