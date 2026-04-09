using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using game2d.scenes;
using System;
using Unity.Cinemachine;
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
            this.buffer.gameObject.SetActive(false);
            this.actorContainer= this.AddChildComponentIfNotExist<Transform>("Actor-Container");
        }

       


#endif
    }
}