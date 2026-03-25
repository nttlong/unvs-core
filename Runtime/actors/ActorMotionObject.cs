using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.actors
{
    public class ActorMotionObject : MonoBehaviour, IActorMotion
    {
        public IActorMotion Instance;
        private Vector3 orginalScale;
        public Animator anim;
        
        public Animator Anim => anim;

        public void Flip(float x)
        {
            if (x > 0)
            {
                anim.transform.localScale = orginalScale;
            } else
            {
                anim.transform.localScale = new Vector3(-orginalScale.x,orginalScale.y,orginalScale.z);
            }
        }

        public void Idle()
        {
            Animator anim =Instance.Anim;
            var physical=this.GetComponent<IActorPhysical>();
            if (physical!=null)
            {
                if (physical.CurrentHoldingObject != null)
                {
                    anim.Play("Motions.Idle-Hang-Item-Forward");
                    return;
                }
            }
            anim.Play("Motions.Idle");
        }

        private void Awake()
        {
          
            anim = GetComponentInChildren<Animator>();
            orginalScale = new Vector3( anim.transform.localScale.x, anim.transform.localScale.y, anim.transform.localScale.z );
            
            this.Instance = this;

        }
        private void Start()
        {
           
            Instance.Idle();
        }
#if UNITY_EDITOR
        private void EditorTimeSetup()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) return;
            //var m = GetComponentInChildren<Animator>();
            //if (m == null) return;
            this.SetMeOnLayer(unvs.shares.Constants.Layers.ACTOR);
           
            GetComponentInChildren<Animator>()?.AddComponentIfNotExist<ActorAnimationEvents>();

            UnityEditor.EditorUtility.SetDirty(this);

        }

        

        private void OnValidate()
        {
            if(Application.isPlaying) return;
            EditorTimeSetup();
        }

        public void Walk()
        {//"Motions.Walk"
            anim.Play("Motions.Walk");
        }
#endif
    }
}

