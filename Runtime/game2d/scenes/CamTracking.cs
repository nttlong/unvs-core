
using System;
using UnityEngine;


namespace  unvs.game2d.scenes
{

    
    public class LoadeSceneTracking:UnvsBaseComponent
    {
        public BoxCollider2D coll;

        private void Awake()
        {
            coll = GetComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var other = collision.otherCollider;
            if (other == Camera.main)
            {
                Debug.LogError($"{name}");
            }
        }
    }
}