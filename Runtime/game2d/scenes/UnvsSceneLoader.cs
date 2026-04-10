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
        public UnvsActor currentActor;

        public override void InitEvents()
        {
            this.buffer.gameObject.SetActive(false);
        }
        public async UniTask<UnvsScene> LoadNewAsync(string path)
        {
            await this.clearAsync();
            var ret= await Commons.LoadPrefabsAsync<UnvsScene>(path,this.buffer);
            ret.transform.SetParent(this.chunks.transform,false);

            UnvsCinema.Instance.UpdateWorldBound(ret);
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
                this.currentActor = actor;
            }
            UnvsCinema.Instance.UpdateMainCameraBoxCollider2dSize();
            return ret;
        }
        public async UniTask<UnvsScene> LoadChunkLeftAsync(UnvsScene fromScene,string path)
        {
            this.clearChunkRightIfExeedeAsync().Forget();
            fromScene.triggerLoadSceneLeft.Off();
            fromScene.wallLeft.enabled = false;

            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(path, this.buffer);
            
            ret.triggerLoadSceneRight.Off();
            ret.wallRight.enabled = false;
            this.validateCurrentActor(ret);
            
            ret.triggerLoadSceneRight.Off();
            ret.wallRight.enabled = false;
            var offset = ret.JoinInfo.RightPos - fromScene.JoinInfo.LeftPos;
            
            ret.transform.position -= (Vector3)offset;
            ret.transform.SetParent(this.chunks.transform,true);
            ret.transform.SetAsFirstSibling();
            UnvsCinema.Instance.UpdateWorldBound(ret);
            ret.gameObject.SetActive(true );
            fromScene.leftScene = ret;
            ret.rightScene = fromScene;



            return ret;
        }

        

        public async UniTask<UnvsScene> LoadChunkRightAsync(UnvsScene fromScene, string path)
        {
            this.clearChunkLeftIfExeedeAsync().Forget();
            fromScene.triggerLoadSceneRight.Off();
           
            fromScene.wallRight.enabled = false;
            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(path, this.buffer);
            ret.triggerLoadSceneLeft.Off();
            ret.triggerLoadSceneRight.Off();
            this.validateCurrentActor(ret);
            
            ret.triggerLoadSceneLeft.Off();
            ret.wallLeft.enabled = false;
            var offset = ret.JoinInfo.LeftPos - fromScene.JoinInfo.RightPos;
            ret.transform.position -= (Vector3)offset;
            ret.transform.SetParent(this.chunks.transform, true);
            ret.transform.SetAsLastSibling();
            UnvsCinema.Instance.UpdateWorldBound(ret);
            ret.gameObject.SetActive(true);
            fromScene.rightScene = ret;
            ret.leftScene = fromScene;

            return ret;
        }
        private void validateCurrentActor(UnvsScene scene)
        {
            var actor = scene.GetActiveActor();
            
            if (actor == null || this.currentActor==null) return;
            actor.GetComponent<UnvsPlayer>().enabled = false;
            if (actor.GetType()== this.currentActor.GetType())
            {
                if(actor.name== this.currentActor.name)
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