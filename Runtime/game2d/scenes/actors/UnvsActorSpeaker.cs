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
using unvs.shares;

#if UNITY_EDITOR
using unvs.shares.editor;

#endif
using unvs.sys;
namespace unvs.game2d.scenes.actors
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