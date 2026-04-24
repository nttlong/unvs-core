

using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using unvs.game2d.objects.components;
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
            if(this.direction==LoadeSceneEnum.Left && !string.IsNullOrEmpty(scene.SceneLeft))
            {
                this.Off();
                UnvsSceneLoader.Instance.LoadChunkLeftAsync(scene,scene.SceneLeft).Forget();
               
            }
            if (this.direction == LoadeSceneEnum.Right && !string.IsNullOrEmpty(scene.SceneRight))
            {
                this.Off();
                UnvsSceneLoader.Instance.LoadChunkRightAsync(scene, scene.SceneRight).Forget();
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