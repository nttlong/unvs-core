using UnityEngine;
using unvs.shares;

namespace unvs.interfaces
{
    public interface ICharacter
    {
        MotionEnum CurrentMotion { get; set; }
        PoseEnum CurrentPose { get; set; }
        Animator Anim { get; }
        string IdleClipName { get; }
        AnimationLayerInfo[] LayerInfo { get; }
        AnimationClip[] Clips { get; }

        void UpdateMotion(MotionEnum value);
        void UpdatePose(PoseEnum value);
    }
}

