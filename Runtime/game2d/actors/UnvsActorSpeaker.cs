using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
using unvs.ext;
using unvs.game2d.objects.components;
using unvs.shares;

#if UNITY_EDITOR


#endif
using unvs.sys;
namespace unvs.game2d.actors
{
    public class UnvsActorSpeaker : UnvsComponent
    {
        public override void InitRuntime()
        {
            
        }

        public void SayIThisDoesNotDoAnything()
        {
            Debug.LogWarning($"{typeof(UnvsActorSpeaker)}.{nameof(SayIThisDoesNotDoAnything)} was not implement");
        }
    }

}