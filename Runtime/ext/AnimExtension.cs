using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.CompilerServices;
using Unity.VisualScripting;
//using Unity.VisualScripting.YamlDotNet.Core.Tokens;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.U2D.Animation;
using unvs.shares;

namespace unvs.ext
{
    public static class AnimExtension
    {
        public static void UpdateValue(this Animator anim, string layerName, string VarName, float value)
        {
            if (anim.IsDestroyed())
            {
                return;
            }
            // 1. Get the index from the name
            int layerIndex = anim.GetLayerIndex(layerName);

            // 2. Validate the layer exists (-1 means not found)
            if (layerIndex != -1)
            {
                anim.SetLayerWeight(layerIndex, 1);
                anim.SetFloat(VarName, value);
            }
            else
            {
                Debug.LogWarning($"Layer {layerName} not found on animator!");
            }
        }
        public static List<int> GetAllLayerIndices(this Animator anim)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < anim.layerCount; i++)
            {
                indices.Add(i);
            }
            return indices;
        }
        public static List<AnimationClip> GetAllClipNames(this Animator anim)
        {
            if (anim.runtimeAnimatorController == null) return new List<AnimationClip>();

            // Lấy tất cả các clip có trong Controller
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

            return clips.ToList();
        }
        public static AnimationClip GetClipByName(this Animator anim, string name)
        {
            if (anim.runtimeAnimatorController == null) return null;

            // Lấy tất cả các clip có trong Controller
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name.Equals(name, StringComparison.OrdinalIgnoreCase)) return clip;
            }
            return null;
        }
        public static void TurnOffAllLayers(this Animator anim)
        {
            for (int i = 0; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i, 0);
            }
        }
        /// <summary>
        /// Reset all layer 
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="ExcludeLayers">skip if match</param>
        public static void ResetAllOverideLayers(this Animator anim,params int[] ExcludeLayers)
        {
            if(anim==null || anim.IsDestroyed()) return;
            for (int i = 0; i < anim.layerCount; i++)
            {
                if (ExcludeLayers.Contains(i)) continue;
                anim.SetLayerWeight(i, 0);
            }
        }
       

        public static async UniTask PlayCrossFadeAsync(this Animator animator, string layerName, string clipName, float crossFadeDuration = 0.1f)
        {
            if (animator == null) return;

            int layerIndex = animator.GetLayerIndex(layerName);
            if (layerIndex == -1) return;

            animator.SetLayerWeight(layerIndex, 1);
            animator.CrossFade(clipName, crossFadeDuration, layerIndex);

            // 1. Chờ hết thời gian CrossFade để Animator thực sự bước vào State mới
            // Nếu không chờ, IsName(clipName) có thể trả về false vì vẫn đang ở clip cũ
            await UniTask.Delay(System.TimeSpan.FromSeconds(crossFadeDuration));

            // 2. Vòng lặp chờ clip chạy đến cuối
            while (animator != null)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

                // ĐIỀU KIỆN ĐÚNG:
                // - Đang chơi đúng clip mục tiêu
                // - normalizedTime >= 1.0f (đã chạy xong 100%)
                // - KHÔNG còn trong trạng thái chuyển tiếp (Transition) sang Exit hoặc State khác
                if (stateInfo.IsName(clipName))
                {
                    if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(layerIndex))
                    {
                        break;
                    }
                }
                else
                {
                    // Nếu vì lý do nào đó Animator nhảy sang State khác (như Exit), cũng phải break để tránh treo máy
                    break;
                }

                await UniTask.Yield();
            }
        }
        public static void PlayCrossFade(this Animator animator, string clipName, float normalizedTransitionDuration = 0.25f)
        {

            if (animator == null) return;
            //animator.TurnOffAllLayers();
            // 1. Lấy index của layer từ tên

            animator.CrossFade(clipName, normalizedTransitionDuration);

        }
        public static void PlayCrossFade(this Animator animator, string layerName, string clipName, float normalizedTransitionDuration = 0.25f)
        {

            if (animator == null) return;
            // animator.TurnOffAllLayers();
            // 1. Lấy index của layer từ tên
            int layerIndex = animator.GetLayerIndex(layerName);

            if (layerIndex == -1)
            {
                Debug.LogError($"Không tìm thấy Layer: {layerName}");
                return;
            }
            var clip = animator.GetClipByName(clipName);
            // 2. Chạy Clip (State) trên layer đó
            // Tham số thứ 3 là 'normalizedTime': 0 có nghĩa là bắt đầu từ đầu clip
            animator.SetLayerWeightByLayerName(layerName, 1);
            animator.CrossFade(clipName, normalizedTransitionDuration, layerIndex);

        }
        public static void PlayAnimation(this Animator animator, string layerName, string clipName, float normalizedTransitionDuration = 0.25f)
        {
            if (animator == null)
            {
                Debug.LogError($"animator is null: {animator}");
            }
            ;
            //animator.TurnOffAllLayers();
            // 1. Lấy index của layer từ tên
            //var chck = animator.GetAllClipNames().FirstOrDefault(p => p.name == clipName);
            //if (chck==null)
            //{
            //    string[] names= animator.GetAllClipNames().Select(p=>p.name).ToArray();
            //    throw new Exception($"{clipName} not found,{string.Join(',', names)}");
            //}
            int layerIndex = animator.GetLayerIndex(layerName);

            if (layerIndex == -1)
            {
                Debug.LogError($"Không tìm thấy Layer: {layerName}");
                return;
            }
            // var clip=animator.GetClipByName(clipName);
            // 2. Chạy Clip (State) trên layer đó
            // Tham số thứ 3 là 'normalizedTime': 0 có nghĩa là bắt đầu từ đầu clip
            //animator.SetLayerWeightByLayerName(layerName, 1);
            animator.Play(clipName, layerIndex, normalizedTransitionDuration);
        }
        public static List<SpriteSkin> ExtractAllSpriteSkins(this Animator anim)
        {
            return anim.GetComponentsInChildren<SpriteSkin>(true).ToList();
        }
#if UNITY_EDITOR
        public static List<BlendTreeEntry> EditorGetListOfAnim(this Animator anim)
        {
            var ret = new List<BlendTreeEntry>();
            var controller = anim.runtimeAnimatorController as AnimatorController;
            foreach (var layer in controller.layers)
            {
                // 3. Duyệt qua các State trong StateMachine của mỗi Layer
                foreach (var stateInMachine in layer.stateMachine.states)
                {
                    var state = stateInMachine.state;

                    // 4. Kiểm tra xem Motion của State này có phải là Blend Tree không
                    if (state.motion is BlendTree blendTree)
                    {
                        Debug.Log($"--- Blend Tree: {blendTree.name} ---");

                        // 5. Lấy danh sách các Motion con
                        ChildMotion[] children = blendTree.children;
                        foreach (var child in children)
                        {
                            if (child.motion != null)
                            {
                                ret.Add(new BlendTreeEntry
                                {
                                    motionName = child.motion.name,
                                    value = child.threshold,
                                });



                            }
                        }
                    }
                }
            }
            return ret;
        }


#endif
        
    }
}

