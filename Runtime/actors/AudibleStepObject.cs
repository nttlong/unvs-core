using UnityEngine;
using unvs.interfaces;
namespace unvs.actors
{
    public class AudibleStepObject : MonoBehaviour, IAudibleStep
    {
        [SerializeField]
        public AudioClip stepSound;
        [Range(0, 1)]
        public float volume = 0.3f;

        public AudioClip StepSound => stepSound;

        public float Volume => volume;
    }
}