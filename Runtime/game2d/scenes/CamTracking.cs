

using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using unvs.components;
using unvs.components;
using unvs.shares;


namespace  unvs.game2d.scenes
{

    public enum LoadeSceneEnum
    {
        Left, Right, Top, Bottom
    }
    public class LoadSceneTracking:UnvsBaseComponent
    {
        public BoxCollider2D coll;
        public LoadeSceneEnum direction;

        private void Awake()
        {
            coll = GetComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {


            //if (collision.gameObject != Camera.main.gameObject) return;
            if (collision.gameObject.tag != Constants.Tags.TRIGGER_LOAD_SCENE) return;
            var scene = this.GetComponentInParent<UnvsScene>();
            if(scene == null) return;
            if(this.direction==LoadeSceneEnum.Left && scene.Links.LeftScene!=null && scene.Links.LeftScene.AssetGUID!="")
            {
                this.Off();
                UnvsSceneLoader.Instance.LoadChunkLeftAsync(scene, scene.Links.LeftScene).Forget();
               
            }
            if (this.direction == LoadeSceneEnum.Right && scene.Links.RightScene != null && scene.Links.RightScene.AssetGUID != "")
            {
                this.Off();
                UnvsSceneLoader.Instance.LoadChunkRightAsync(scene, scene.Links.RightScene).Forget();
            }
        }

        public void Off()
        {
            this.enabled = false;
            this.gameObject.SetActive(false);
            this.GetComponent<Collider2D>().enabled = false;

        }
        public void On()
        {
            this.GetComponent<Collider2D>().enabled = true;
            this.gameObject.SetActive(true);
            this.enabled = true;
            
           
        }
    }
}