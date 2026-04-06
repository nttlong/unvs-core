using Cysharp.Threading.Tasks.Triggers;
using System;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;

namespace unvs.players{
    [Serializable]
    public class AnimExractor
    {
        [SerializeField]
        public Animator animator;
       
        [SerializeField]
        public BlendTreeInfo[] motions;
        private PlayerBase owner;
        
        public virtual void BaseMotion(string name)
        {
            this.motions.PlayBaseLayer(name);
        }

        public void AddtiveMotion(string name)
        {
            this.motions.PlayAddtiveMotion(name);
        }


#if UNITY_EDITOR
        internal void EditorExtractAllAnim(PlayerBase obj)
        {
           animator = obj.GetComponentInChildren<Animator>();
          
           motions = animator.EditorExtractAllMotions().ToArray();
        }
#endif
    }
}