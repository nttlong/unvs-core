using Cysharp.Threading.Tasks;
using game2d.scenes;
using System;
using UnityEngine;
using unvs.ext;
using unvs.shares;

namespace unvs.game2d.scenes
{
    public class UnvsSceneLoader : UnvsUIComponentInstance<UnvsSceneLoader>
    {
        public Transform chunks;
        public Transform buffer;

        

        
        public override void InitEvents()
        {
            this.buffer.gameObject.SetActive(false);
        }
        public async UniTask<UnvsScene> LoadNewAsync(string path)
        {
            await this.clearAsync();
            var ret= await Commons.LoadPrefabsAsync<UnvsScene>(path,this.buffer);
            ret.transform.SetParent(this.chunks.transform,false);
            ret.gameObject.SetActive(true);
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
        }

       


#endif
    }
}