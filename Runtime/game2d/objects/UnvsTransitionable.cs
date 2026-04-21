using UnityEngine;
using unvs.game2d.scenes;
using unvs.game2d.transitions;

namespace game2d.objects {
    
    public class UnvsTransitionable : UnvsBaseComponent {
        public UnvsTransitionDefinitions Transition;
        public bool IsOn;
    }
}