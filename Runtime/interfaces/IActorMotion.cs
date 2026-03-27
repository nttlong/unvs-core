using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using unvs.shares;

namespace unvs.interfaces
{
    public interface IActorMotion
    {
        
        BlendTreeInfo[] BlendTreeAnim {  get; }
        BlendTreeInfo[] MotionsAnim { get; }
        
        Animator Anim { get; }
        void BendDownAndPickItem();
        void PickItem();
        UniTask BendDownAndPickItemAsync();
        UniTask PickItemAsync();
        void Flip(float x);
        void Idle();
        void Walk();
        void PlayMotion(string motionName);
       
        void PlayAddtiveMotion(string motionName);
        void PlayBlendTreeMotion(string layername,string blendName, string motionName);
        void Sprint();
        
    }
}