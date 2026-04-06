using System;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;

namespace unvs.players{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent (typeof(CapsuleCollider2D))]
    public class PlayerBase:MonoBehaviour{
        private IScenePrefab _currentScene;
        private Collider2D coll;
        [SerializeField]
        public AnimExractor anims;
       

        private void Reset()
        {
            var col=this.AddComponentIfNotExist<CapsuleCollider2D>();
            col.size = new Vector2(8, 20);
            col.offset= new Vector2(0, 10);
        }
        public virtual void Awake()
        {
            
            this.RuntimeInit();
        }

        public virtual void RuntimeInit()
        {
            if (!Application.isPlaying) return;
            this.RunTimeSetPosition();
            this.anims.BaseMotion("Walk");
            this.anims.AddtiveMotion("hanging-item");
        }

        public virtual void RunTimeSetPosition()
        {
            _currentScene=this.GetComponentInParent<IScenePrefab>();
            coll=this.GetComponent<Collider2D>();
            if(_currentScene != null )
            {
                _currentScene.StartPos.MoveOtherToMe(this);
                
            }
        }
#if UNITY_EDITOR
        public void EditorExtractAllAnim()
        {
            this.anims.EditorExtractAllAnim(this);
        }
#endif
    }
}