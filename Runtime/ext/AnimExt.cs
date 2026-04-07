using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor.Animations;
//using UnityEditorInternal;

#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using UnityEngine.XR;
using unvs.shares;


namespace unvs.ext
{
    public static class AnimExt
    {
        public static List<AnimationLayerInfo> LayersIndices(this Animator anim)
        {
            List<AnimationLayerInfo> indices = new List<AnimationLayerInfo>();
            for (int i = 0; i < anim.layerCount; i++)
            {
                indices.Add(new AnimationLayerInfo
                {
                    Index = i,
                    Name = anim.GetLayerName(i),
                });

            }
            return indices;
        }
        public static List<AnimationClip> ClipNames(this Animator anim)
        {
            if (anim.runtimeAnimatorController == null) return new List<AnimationClip>();

            // Lấy tất cả các clip có trong Controller
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

            return clips.ToList();
        }
        public static void CrossFadeMotion(this Animator animator, string layerName, string clipName, float normalizedTransitionDuration = 1f)
        {

            if (animator == null) return;
            //animator.TurnOffAllLayers();
            // 1. Lấy index của layer từ tên

            //animator.SetLayerWeightByLayerName(layerName, 1);
            animator.CrossFade($"{layerName}.{clipName}", normalizedTransitionDuration);

        }
        public static void CrossFadeMotion(this Animator animator, string clipName, float normalizedTransitionDuration = 0.25f)
        {

            if (animator == null) return;
            //animator.TurnOffAllLayers();
            // 1. Lấy index của layer từ tên

            animator.CrossFade(clipName, normalizedTransitionDuration);

        }
        public static void SetLayerWeightByLayerName(this Animator anim, string layerName, float w = 1f)
        {
            anim.SetLayerWeight(anim.GetLayerIndex(layerName), w);
        }
        public static void Motion(this Animator animator, string clipName)
        {

            animator.PlayInFixedTime(clipName);


        }
        public static void Motion(this Animator animator, string layerName, string clipName, float percent = 1)
        {

            animator.Play(clipName, animator.GetLayerIndex(layerName), percent);


        }
#if UNITY_EDITOR
        public static List<BlendTreeInfo> EditorExtractBaseLayer(this Animator anim)
        {
            var ret = new List<BlendTreeInfo>();
            var controller = anim.runtimeAnimatorController as AnimatorController;
            
            for(var i=0;i< controller.layers.Length; i++)
            {
                var layer = controller.layers[i];
                for (var j = 0; j < layer.stateMachine.states.Length; j++)
                {
                    var stateInMachine = layer.stateMachine.states[j];
                    var state = stateInMachine.state;
                    if (state.motion is BlendTree blendTree)
                    {
                        extractBlendTree(anim, ret, i,j, layer, blendTree);

                    }

                }
            }
            
            return ret;
        }

        private static void extractBlendTree(Animator anim, List<BlendTreeInfo> ret,int layerIndex, int blendIndex, AnimatorControllerLayer layer, BlendTree blendTree)
        {
            foreach (var child in blendTree.children)
            {
                ret.Add(new BlendTreeInfo
                {
                    blendName = blendTree.name,
                    blendIndex = blendIndex,
                    motionName = child.motion.name,

                    layerName = layer.name,
                    animationController = anim,
                    layerIndex = layerIndex,
                    paramName = blendTree.blendParameter,
                    value = child.threshold


                });
            }
        }

        public static List<BlendTreeInfo> EditorExtractAllMotions(this Animator anim)
        {
            AnimatorController controller = null;
            var ret = new List<BlendTreeInfo>();
            if (anim.runtimeAnimatorController is AnimatorOverrideController oController)
            {
                controller = oController.runtimeAnimatorController as AnimatorController;
                
            } else {
                controller = anim.runtimeAnimatorController as AnimatorController;
            }
            for (var i=0;i<controller.layers.Length;i++)
            {
               var layer = controller.layers[i];
                var j=0;
                foreach (var stateInMachine in layer.stateMachine.states)
                {
                    var state = stateInMachine.state;
                    if (state.motion is BlendTree tree)
                    {
                        extractBlendTree(anim, ret, i, j, layer, tree);
                        continue;

                    }
                    
                        if (state is AnimatorState animSt)
                    {
                        ret.Add(new BlendTreeInfo
                        {
                            motionName = animSt.name,

                            layerName = layer.name,
                            animationController = anim,
                            layerIndex = i,

                        });
                    }
                }
               
            }
            return ret;
        }

#endif

        public static void CreateAllIK(this Animator anim)
        {

            if (anim == null) return;
            var sp = anim.ExtractAllSpriteSkins();
            var bones = sp.SelectMany(p => p.boneTransforms);
            foreach (var bone in bones)
            {
                Debug.Log($"ExtractAllSpriteSkins {bone.name}");
            }
            Transform rootBone = TransformExt.GetRoot(bones);

            var ik = rootBone.AddComponentIfNotExist<IKManager2D>();


            //IKManager2D ret= ik.AddChildIfNotExist<LimbSolver2D>(nameOfSover);
            //var ls=rootBone.transform.CreateIfNoExist<LimbSolver2D>(nameOfSover);
            var leafs = bones.GetAllLeafBones();
            foreach (var bone in leafs)
            {
                Debug.Log($"ExtractAllSpriteSkins.GetAllLeafBones {bone.name}");
            }
            foreach (var bone in leafs)
            {
                var nameOfSover = $"{bone.name}_solver";
                var nameOfTarget = $"{bone.name}_target";
                //if (ik.solvers.FirstOrDefault(p => p.name == nameOfTarget) != null) continue;
                var ret = rootBone.AddChildIfNotExist<LimbSolver2D>(nameOfSover);
                if (ret == null)
                {
                    throw new System.Exception($"ik.AddChildIfNotExist<LimbSolver2D>({nameOfSover}) fail");
                }
                if (ik.solvers.FirstOrDefault(p => p.name == nameOfTarget) == null)
                {
                    ik.AddSolver(ret);
                }
                //ik.AddSolver(ret);

                var ch = ret.GetChain(0);
                if (ch == null)
                {
                    Debug.LogError($"{ret.name} GetChain is null");
                    return;
                }
                if (ch.target == null)
                {
                    ch.target = ret.AddChildIfNotExist<Transform>(nameOfTarget);

                }
                ch.target.transform.position = bone.transform.position;
                ch.effector = bone;
                //Debug.Log($"CreateAllIK {rootBone.name}->{nameOfSover} {nameOfTarget}");

            }

        }
        public static void CreateAllSortingGroup(this Animator anim, string sortingLayerName)
        {


            var st = anim.GetComponentInParent<SortingGroup>();
            if (st == null) return;
            st.sortingLayerName = sortingLayerName;
            var lst = anim.ExtractAllSpriteSkins();
            foreach (SpriteSkin s in lst)
            {
                var sg = s.transform.AddComponentIfNotExist<SortingGroup>();
                sg.sortingLayerName = sortingLayerName;
                sg.sortingOrder = s.GetComponent<SpriteRenderer>().sortingOrder;
                Debug.Log($"{s.transform.name}");
            }
        }
        public static async UniTask PlayAnimationAsync(this Animator animator, string stateName, int layer,Action OnFinish, CancellationToken ct, float normalizedTime=1f)
        {
            // 1. Kích hoạt Animation
            animator.PlayInFixedTime(stateName, layer);

            // 2. Đợi 1 frame để Animator chuyển sang State mới 
            // (Nếu không nó sẽ lấy thông tin của State cũ)
            await UniTask.Yield();
            
            // 3. Đợi cho đến khi Animation chạy đến cuối (normalizedTime >= 1)
            await UniTask.WaitUntil(() =>
               // animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName) &&
               
                animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= animator.GetCurrentAnimatorStateInfo(layer).length,
                cancellationToken: ct
            );

            OnFinish?.Invoke();
        }
    }
}

