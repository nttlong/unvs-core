using UnityEngine;
using unvs.interfaces;
using unvs.ext;
using unvs.shares;
namespace unvs.GameObjects.Characters
{
    [ExecuteInEditMode]
    public class CharacterObject : MonoBehaviour, ICharacter
    {
        [Header("Animation")]
        public Animator anim;
        public AnimationLayerInfo[] layerInfo;
        public AnimationLayerInfo[] LayerInfo => layerInfo;

        public string idleClipName;
        public AnimationClip[] clips;
        public MotionEnum currentMotion;
        private PoseEnum currentPose;

        public Animator Anim => anim;

        public string IdleClipName => idleClipName;

        public AnimationClip[] Clips => clips;

        public MotionEnum CurrentMotion
        {
            get => currentMotion;
            set
            {
                currentMotion= value;
                ((ICharacter)this).UpdateMotion(value);
            }
        }
        public PoseEnum CurrentPose
        {
            get => currentPose;
            set
            {
                currentPose = value;
                ((ICharacter)this).UpdatePose(value);
            }
        }

        void Awake()
        {
            
            anim = GetComponentInChildren<Animator>();
            if (! Application.isPlaying) 
                BeforePlaying();
            if (CurrentMotion == MotionEnum.None)
            {
                CurrentMotion=MotionEnum.Idle;
            }
        }
        
        private void BeforePlaying()
        {
            layerInfo = AnimExt.LayersIndices(anim).ToArray();
            clips = AnimExt.ClipNames(anim).ToArray();
        }

        public void UpdateMotion(MotionEnum value)
        {
            Anim?.Motion(value.ToString());
        }

        public void UpdatePose(PoseEnum value)
        {
            throw new System.NotImplementedException();
        }
    }
}

