using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Unity.Burst;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using unvs.shares;

namespace unvs.ext 
{
    public static class BlendTreeInfoExtension
    {
        public static void PlayBaseLayer(this BlendTreeInfo[] blendTreeAnim, string motionName, string overideState=null)
        {
            var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(p.blendName));
            if (item == null)
            {
                throw new Exception($"Can not find {motionName}");
            }
            item.animationController.ResetAllOverideLayers();

            item.animationController.SetLayerWeight(item.layerIndex, 1f);
            item.animationController.SetFloat(item.paramName, item.value);
            if (overideState != null)
            {
                var overideStateItem = blendTreeAnim.FirstOrDefault(p => p.layerIndex != item.layerIndex && p.motionName.Equals(overideState, StringComparison.OrdinalIgnoreCase));
                if (overideStateItem == null)
                {
                    Debug.LogError($"{overideState} was not found");
                }
                overideStateItem.animationController.SetLayerWeight(overideStateItem.layerIndex, 1f);
                overideStateItem.animationController.PlayInFixedTime(overideStateItem.motionName, overideStateItem.layerIndex);
            }
        }
        public static void PlayBlendTree(this BlendTreeInfo[] blendTreeAnim,  string motionName)
        {
            
            var item= blendTreeAnim.FirstOrDefault(p=>p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty( p.blendName));
            if (item == null)
            {
                throw new Exception($"Can not find {motionName}");
            }
            
            item.animationController.ResetAllOverideLayers();
            item.animationController.SetLayerWeight(item.layerIndex, 1f);
            item.animationController.SetFloat( item.paramName, item.value);
        }
       
        public static void PlayMotion(this BlendTreeInfo[] blendTreeAnim, string motionName,string overideState)
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
            if(overideState!=null)
            {
                var overideStateItem = blendTreeAnim.FirstOrDefault(p => p.layerIndex != item.layerIndex && p.motionName.Equals(overideState, StringComparison.OrdinalIgnoreCase));
                item.animationController.ResetAllOverideLayers();
                item.animationController.SetLayerWeight(item.layerIndex, 1);
                item.animationController.PlayInFixedTime(motionName, item.layerIndex, 1f);
                overideStateItem.animationController.SetLayerWeight(overideStateItem.layerIndex, 1f);
                overideStateItem.animationController.PlayInFixedTime(overideStateItem.motionName, overideStateItem.layerIndex);
            } else
            {
                item.animationController.ResetAllOverideLayers();
                item.animationController.SetLayerWeight(item.layerIndex, 1);
                if (item.blendIndex > 0)
                {

                    item.animationController.SetFloat(item.paramName, item.value);
                }
                else
                    item.animationController.PlayInFixedTime(motionName, item.layerIndex, 1f);
            }
                

           
        }
        public static async UniTask PlayMotionAsync(this BlendTreeInfo[] blendTreeAnim, string motionName,string ovveriSate=null)
        {
            if(ovveriSate == null)
            {
                var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));
                item.animationController.ResetAllOverideLayers();
                await item.animationController.PlayAnimationAsync(motionName, item.layerIndex, () =>
                {

                });
            } else
            {
                
                var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));
                var ovveriSateItem = blendTreeAnim.FirstOrDefault(p => p.layerIndex != item.layerIndex && p.motionName.Equals(ovveriSate, StringComparison.OrdinalIgnoreCase));
                
                await item.animationController.PlayAnimationAsync(motionName, item.layerIndex, () =>
                {


                    ovveriSateItem.animationController.SetLayerWeight(ovveriSateItem.layerIndex, 1);
                    ovveriSateItem.animationController.PlayInFixedTime(ovveriSateItem.motionName, ovveriSateItem.layerIndex, 0f);
                });
            }
            

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
            item.animationController.ResetAllOverideLayers();
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
        public static async UniTask PlayCrossFadeMotionAsync(this BlendTreeInfo[] blendTreeAnim, string motionName, float normalizedTimeOffset = 0.25f)
        {
            var item = blendTreeAnim.FirstOrDefault(p => p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));

            if (item == null) throw new Exception($"Can not find {motionName}");
            if (item.animationController == null) throw new NullReferenceException($"animationController is null for {motionName}");

            // 1. Kích hoạt Motion
            item.animationController.SetLayerWeight(item.layerIndex, 1);
            item.animationController.CrossFade(motionName, normalizedTimeOffset, item.layerIndex);

            // 2. Đợi 1 frame để Animator nhận diện State mới (Bắt buộc)
            await UniTask.Yield();

            // 3. Đợi cho đến khi Motion kết thúc hoặc chuyển sang State khác
            await UniTask.WaitUntil(() =>
            {
                var stateInfo = item.animationController.GetCurrentAnimatorStateInfo(item.layerIndex);
                return !stateInfo.IsName(motionName) || stateInfo.normalizedTime >= 1.0f;
            });

            Debug.Log($"[Architect Log] Motion {motionName} hoàn tất.");
        }
        public static void PlayAddtiveMotion(this BlendTreeInfo[] blendTreeAnim, string motionName)
        {
            var item = blendTreeAnim.FirstOrDefault(p =>p.layerIndex>0 && p.motionName.Equals(motionName, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new Exception($"Can not find {motionName}");
            }
            if (item.animationController == null)
            {
                throw new NullReferenceException($"animationController is null, refer {motionName}");
            }
            item.animationController.ResetAllOverideLayers();
            if(item.layerIndex > 0)
            item.animationController.SetLayerWeight(item.layerIndex, 1);
            item.animationController.Play(item.motionName, item.layerIndex, 0f);
        }
    }
}