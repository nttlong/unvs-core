using UnityEngine;

namespace unvs.game2d.scenes.actors {
    public enum ActorType {
        None,
        Player,
        Enemy,
        NPC
    }
    public abstract class UnvsActorSkill:UnvsBaseComponent
    {
        public Animator baseAnim;
       
        internal bool _hasActiveSkill;
        internal RuntimeAnimatorController _backupAnim;

        public AnimatorOverrideController animSkill;
        public virtual void ActiveSkill()
        {
            if (_hasActiveSkill) return;
            this.enabled = true;
            _backupAnim = GetComponentInChildren<Animator>().runtimeAnimatorController;
            GetComponentInChildren<Animator>().runtimeAnimatorController = animSkill;
            _hasActiveSkill = true;
        }
        public virtual void DeactiveSkill()
        {
            if(_backupAnim==null) return;
            this.enabled = false;
            GetComponentInChildren<Animator>().runtimeAnimatorController = _backupAnim;
            _hasActiveSkill = false;
        }
       
     

    }
}