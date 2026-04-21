using UnityEngine;

namespace unvs.game2d.transitions
{
    [CreateAssetMenu(fileName = "transition-data", menuName = "Systems/transition2d data")]
    public class UnvsTransitionDefinitions : ScriptableObject
    {
        [SerializeReference]
        public UnvsTransitionBase[] actions;

    }
    
    
}