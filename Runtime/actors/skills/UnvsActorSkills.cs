using UnityEngine;

namespace unvs.actor.skills{
    [CreateAssetMenu(fileName = "ActorSkill", menuName = "Systems/Actor Skill Data")]
    public class UnvsActorSkills : ScriptableObject
    {
        [SerializeReference, SkillSelector]
        public ActorBaseSkill[] skills;

    }
}