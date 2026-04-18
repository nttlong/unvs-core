using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using unvs.actions;
using unvs.game2d.scenes.actors;

namespace unvs.actor.skills
{
    public class SkillSelectorAttribute : PropertyAttribute { }
    [Serializable]
    public abstract class ActorBaseSkill // Chuyển thành abstract
    {


        public string name;
        [SerializeField]
        public AnimatorOverrideController animator;
        private RuntimeAnimatorController _backupAnim;

        public virtual void Active(UnvsActor actor)
        {
            if (animator != null)
            {
                var anim = actor.GetComponentInChildren<Animator>();
                if (anim != null)
                {
                    _backupAnim = anim.runtimeAnimatorController;
                    anim.runtimeAnimatorController = animator;
                }
            }
            
        }
    }
    
}