using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;
namespace unvs.actors
{
    public  class  ActorMotionObject : MonoBehaviour, IActorMotion
    {
        public IActorMotion Instance;
        private Vector3 orginalScale;
        public Animator anim;
        [SerializeField]
        public BlendTreeInfo[] blendTreeAnim=new BlendTreeInfo[] {};
       
        
        [SerializeField]
        public BlendTreeInfo[] motionsAnim = new BlendTreeInfo[] { };
        
        

        public Animator Anim => anim;
        
        public BlendTreeInfo[] BlendTreeAnim => blendTreeAnim;

        

        public BlendTreeInfo[] MotionsAnim => motionsAnim;

        

       

        public void PlayMotion(string motionName)
        {
            this.motionsAnim?.PlayMotion(motionName);
            
        }

        public virtual void PlayBaselayerMotion(string motionName)
        {

            this.blendTreeAnim?.PlayBaseLayer(motionName);
        }
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

        public virtual void Idle()
        {
            Animator anim =Instance.Anim;
            var physical=this.GetComponent<IActorPhysical>();
            if (physical!=null)
            {
                if (physical.CurrentHoldingObject != null)
                {
                    throw new System.NotImplementedException();
                    return;
                }
            }
            this.PlayBaselayerMotion("Idle");


        }
        public virtual void PlayBlendTreeMotion(string layername, string blendName, string motionName)
        {
            this.blendTreeAnim.PlayBlendTree(layername, blendName, motionName);

        }
        private void Awake()
        {
          
            anim = GetComponentInChildren<Animator>();
            orginalScale = new Vector3( anim.transform.localScale.x, anim.transform.localScale.y, anim.transform.localScale.z );
            
            this.Instance = this;

        }
        public async UniTask BendDownAndPickItemAsync()
        {
            await UniTask.Yield();
            BendDownAndPickItem();
        }

        public virtual void BendDownAndPickItem()
        {
           // this.motionsAnim.PlayCrossFadeMotion("BendDownAndPickItem",0);

            this.motionsAnim.PlayMotion("BendDownAndPickItem");
        }

        public virtual void Sprint()
        {
            this.PlayBaselayerMotion("Sprint");
        }


        public void PlayAddtiveMotion(string motionName)
        {
            this.motionsAnim.PlayAddtiveMotion(motionName);
        }
        public virtual async UniTask PickItemAsync()
        {
            await UniTask.Yield();
            PickItem();
        }
        public virtual void Walk()
        {//"Motions.Walk"
            this.PlayBaselayerMotion("Walk");
            
        }
        public virtual void PickItem()
        {
            this.motionsAnim.PlayCrossFadeMotion("PickItem");
        }

        private void Start()
        {
           
            Idle();
        }
#if UNITY_EDITOR
        private void EditorTimeSetup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (Application.isPlaying) return;
            GetComponentInChildren<Animator>()?.AddComponentIfNotExist<ActorAnimationEvents>();
            if(anim==null) anim = GetComponentInChildren<Animator>();
            if (anim != null)
            {
                blendTreeAnim = anim.EditorExtractBaseLayer().ToArray();
                
                
                
                motionsAnim = anim.EditorExtractAllMotions().ToArray();
                
            }
            UnityEditor.EditorUtility.SetDirty(this);

        }

        

        private void OnValidate()
        {
            if(Application.isPlaying) return;
            EditorTimeSetup();
        }

        
#endif
        
        



    }
}

