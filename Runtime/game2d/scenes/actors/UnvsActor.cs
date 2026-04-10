using Cysharp.Threading.Tasks.Triggers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
using unvs.ext;
#if UNITY_EDITOR
using unvs.shares.editor;
using unvs.sys;
#endif

namespace unvs.game2d.scenes.actors
{
    [RequireComponent(typeof(IKBoneMap))]
    [RequireComponent(typeof(AnimMap))]
    [RequireComponent(typeof(UniqueObject))]
    public class UnvsActor : UnvsBaseComponent
    {

        private bool isMoving;
        private Vector2 target;
        private Vector2 direction;
        public float WalkSpeed = 8f;
        public Collider2D coll;
        public Rigidbody2D body;
        public Transform camWatcher;
       
        public GameObject animEle;
        public Animator animator;
        public AnimMap motions;
       
        public void StandBy(Vector2 vector2)
        {
            this.transform.position = vector2;// + new Vector2((coll.bounds.max.x-coll.bounds.min.x)/2,0);
            

        }
        public void SayText(string Content)
        {
            var pos=new Vector2(coll.bounds.center.x,coll.bounds.max.y+2);
            UnvsActirDialogue.Instance.Show(pos,Content);
        }
        public void SayOff()
        {
            UnvsActirDialogue.Instance.Hide();
        }



#if UNITY_EDITOR
        [UnvsButton]
        public void FixLayout()
        {

        }
        [UnvsButton("Anim controller")]
        public void GenerateAnimatorController()
        {
            this.animEle = this.GetComponentInChildren<SpriteSkin>(true).transform.parent.gameObject;
            string folderPath=EditorUtils.EditorGetFolder(this.animEle);
            var controller =EditorUtils.EditorCreateAnimatorController(folderPath, this.animEle.name);
            this.animator= this.animEle.transform.AddComponentIfNotExist<Animator>();
            this.animator.runtimeAnimatorController = controller;
        }
        [UnvsButton]
        public void Generate()
        {
            if(this.CheckComponentIfNotExistCreate<CapsuleCollider2D>(out var _coll))
            {

                _coll.size = new Vector2(8, 20);
                _coll.offset = new Vector2(0, 10);
                this.coll = _coll;
            }
            
            
            body = this.AddComponentIfNotExist<Rigidbody2D>();
            body.freezeRotation = true;
            this.camWatcher = this.AddChildComponentIfNotExist<Transform>("cam-wacther");
            this.camWatcher.position = new Vector3(this.coll.bounds.center.x, this.coll.bounds.max.y, -10);
        }

        


#endif
    }
}
