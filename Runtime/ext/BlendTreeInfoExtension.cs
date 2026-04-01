using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Unity.Burst;
using Unity.Cinemachine;
using UnityEngine;
using unvs.shares;

namespace unvs.ext 
{
    public static class BlendTreeInfoExtension
    {
        public static void PlayBaseLayer(this BlendTreeInfo[] blendTreeAnim, string motionName)
        {
            var item = blendTreeAnim.FirstOrDefault(p => 
             p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase) );

            item.animationController.ResetAllAddtiveLayers();
            item.animationController.SetLayerWeight(item.layerIndex, 1f);
            item.animationController.SetFloat( item.paramName, item.value);
        }
        public static void PlayBlendTree(this BlendTreeInfo[] blendTreeAnim, string layerName,string blenderName, string motionName)
        {
            
            var item= blendTreeAnim.FirstOrDefault(p=>p.layerName.Equals(layerName,StringComparison.OrdinalIgnoreCase)
            && p.blendName.Equals(blenderName,StringComparison.OrdinalIgnoreCase)
            &&p.motionName.Equals(motionName,StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new Exception($"Can not find {layerName}.{motionName}");
            }
            if(item.animationController==null)
            {
                throw new NullReferenceException($"animationController is null, refer {layerName}.{motionName}");
            }
            item.animationController.ResetAllAddtiveLayers();
            item.animationController.SetLayerWeight(0,1f);
            item.animationController.SetFloat( item.paramName, item.value);
        }
        public static void PlayMotion(this BlendTreeInfo[] blendTreeAnim, string motionName)
        {
            var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new Exception($"Can not find {motionName}");
            }
            if (item.animationController == null)
            {
                throw new NullReferenceException($"animationController is null, refer {motionName}");
            }
            item.animationController.ResetAllAddtiveLayers(true);
            item.animationController.SetLayerWeight(item.layerIndex, 1);
            item.animationController.PlayInFixedTime(motionName, item.layerIndex, 1f);

           
        }
        public static async UniTask PlayMotionAsync(this BlendTreeInfo[] blendTreeAnim, string motionName,Action OnFinish,CancellationToken ct)
        {
            var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new Exception($"Can not find {motionName}");
            }
            if (item.animationController == null)
            {
                throw new NullReferenceException($"animationController is null, refer {motionName}");
            }
            item.animationController.ResetAllAddtiveLayers(true);
            item.animationController.SetLayerWeight(item.layerIndex, 1);
            await item.animationController.PlayAnimationAsync(motionName, item.layerIndex, OnFinish,ct);


        }
        public static void PlayCrossFadeMotion(this BlendTreeInfo[] blendTreeAnim, string motionName, float normalizedTimeOffset=0.25f)
        {
            var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new Exception($"Can not find {motionName}");
            }
            if (item.animationController == null)
            {
                throw new NullReferenceException($"animationController is null, refer {motionName}");
            }
            item.animationController.SetLayerWeight(item.layerIndex, 1);
            item.animationController.CrossFadeInFixedTime(motionName, normalizedTimeOffset,item.layerIndex);
        }
        public static void PlayAddtiveMotion(this BlendTreeInfo[] blendTreeAnim, string motionName)
        {
            var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new Exception($"Can not find {motionName}");
            }
            if (item.animationController == null)
            {
                throw new NullReferenceException($"animationController is null, refer {motionName}");
            }
            item.animationController.ResetAllAddtiveLayers();
            if(item.layerIndex > 0)
            item.animationController.SetLayerWeight(item.layerIndex, 1);
            item.animationController.Play(item.motionName, item.layerIndex, 0f);
        }
    }
}