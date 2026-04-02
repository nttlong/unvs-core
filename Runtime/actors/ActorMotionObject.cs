using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
           
            this.PlayBaselayerMotion("Idle");

            var physical = this.GetComponent<IActorPhysical>();
            if (physical != null)
            {
                if (physical.CurrentHoldingObject != null)
                {
                    this.PlayAddtiveMotion("Idle-Hangging-Item");
                    physical.CurrentHoldingObject.transform.AttachToParent(this.GetComponent<IActorPhysical>().SocketBack.transform);
                    
                }
                  
            }
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
        public async UniTask BendDownAndPickItemAsync(Action OnFinish, CancellationToken ct)
        {
            await UniTask.Yield();
            await this.motionsAnim.PlayMotionAsync("BendDownAndPickItem", OnFinish, ct);
        }
        public async UniTask PickItemAsync(Action OnFinish, CancellationToken ct)
        {
            await UniTask.Yield();
            await this.motionsAnim.PlayMotionAsync("PickItem", OnFinish, ct);
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
            var physical = this.GetComponent<IActorPhysical>();
            if (physical != null)
            {
                if (physical.CurrentHoldingObject != null)
                {
                    this.PlayAddtiveMotion("Walk-Hangging-Item");
                    physical.CurrentHoldingObject.transform.AttachToParent(this.GetComponent<IActorPhysical>().SocketBack.transform);
                   
                }
            }
            this.GetComponent<ISpeakableObject>()?.Off();
            
        }
        public virtual void PickItem()
        {
            this.motionsAnim.PlayMotion("PickItem");
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

