using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using unvs.game2d.scenes.actors;

namespace unvs.actor.skills{
    [CreateAssetMenu(fileName = "ActorSkill", menuName = "Systems/Actor Skill Data")]
    public class UnvsActorSkills : ScriptableObject
    {
        [SerializeReference, SkillSelector]
        public AbstractActorBaseSkill[] skills;

        // Cache để truy cập cực nhanh
        private Dictionary<Type, AbstractActorBaseSkill> _skillCache;

        public MonoBehaviour Owner { get; internal set; }
        public void Initialize(MonoBehaviour owner)
        {
            this.Owner = owner;
            if (skills == null) return;

            foreach (var skill in skills)
            {
                if (skill != null)
                {
                    skill.Owner = owner;
                    skill.OnBind(); // Cho skill cơ hội cache các component như Animator, Rigidbody
                }
            }
        }
        public T Get<T>(AbstractActionBaseSkill previous=null) where T : AbstractActorBaseSkill
        {
            _skillCache ??= new Dictionary<Type, AbstractActorBaseSkill>();

            var type = typeof(T);
            if (!_skillCache.ContainsKey(type))
            {
                _skillCache[type] = skills.FirstOrDefault(p => p is T);
            }
            (_skillCache[type] as AbstractActorBaseSkill).PreviousSkill=previous;
            return _skillCache[type] as T;
        }
    }
}