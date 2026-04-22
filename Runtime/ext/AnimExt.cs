using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;

using UnityEditor;


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
        //public static List<AnimStateInfo> EditorExtractBaseLayer(this Animator anim, Transform motionObjs)
        //{
        //    var ret = new List<AnimStateInfo>();
        //    var controller = anim.runtimeAnimatorController as AnimatorController;
            
        //    for(var i=0;i< controller.layers.Length; i++)
        //    {
        //        var layer = controller.layers[i];
        //        for (var j = 0; j < layer.stateMachine.states.Length; j++)
        //        {
        //            var stateInMachine = layer.stateMachine.states[j];
        //            var state = stateInMachine.state;
        //            if (state.motion is BlendTree blendTree)
        //            {
        //                extractBlendTree(motionObjs,anim, ret, i,j, layer, blendTree);

        //            }

        //        }
        //    }
            
        //    return ret;
        //}

        private static void extractBlendTree(Animator anim, List<AnimStateInfo> ret,int layerIndex, int blendIndex, AnimatorControllerLayer layer, BlendTree blendTree)
        {
            foreach (var child in blendTree.children)
            {
                var obj = new AnimStateInfo($"{layer.name}-{blendTree.name}-{child.motion.name}", anim); //motionObjs.AddChildComponentIfNotExist<AnimStateInfo>($"{layer.name}-{blendTree.name}-{child.motion.name}");
                obj.blendName = blendTree.name;
                obj.blendIndex = blendIndex;
                obj.motionName= child.motion.name;
                obj.layerName= layer.name;
                obj.layerIndex= layerIndex;
                obj.paramName = blendTree.blendParameter;
                //obj.name = $"{layer.name}-{blendTree.name}-{child.motion.name}";
                //obj.animationController = anim;
                obj.value = child.threshold;
                obj.clip = child.motion;
                ret.Add(obj);
                
            }
        }

        public static List<AnimStateInfo> EditorExtractAllMotions(this Animator anim)
        {
           
            AnimatorController controller = null;
            var ret = new List<AnimStateInfo>();
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

                        var motion = new AnimStateInfo($"{layer.name}-{animSt.name}", anim);//  motionObjs.AddChildComponentIfNotExist<AnimStateInfo>($"{layer.name}-{animSt.name}");
                        //motion.name = $"{layer.name}-{animSt.name}";
                        motion.motionName = animSt.name;
                        motion.layerName = layer.name;
                        //motion.animationController = anim;
                        motion.layerIndex = i;
                        motion.clip= state.motion;
                        ret.Add(motion);
                    }
                }
               
            }
            return ret;
        }

#endif

      
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
        //public static async UniTask PlayAnimationAsync(
        //         this Animator animator,
        //         string stateName,
        //         int layer,
        //         Func<bool> onPlay,
        //         Action onFinish,
        //         CancellationToken ct,
        //         float normalizedEnd = 1f)
        //{
        //    animator.Play(stateName, layer, 0f);

        //    await UniTask.Yield(PlayerLoopTiming.Update, ct);

        //    await UniTask.WaitUntil(() =>
        //    {
        //        if (onPlay != null)
        //        {
        //            if (!onPlay()) return true;
        //        }
        //        var info = animator.GetCurrentAnimatorStateInfo(layer);

        //        // đảm bảo đúng state
        //        if (!info.IsName(stateName)) return false;

        //        // normalizedTime >= 1 nghĩa là đã chạy hết 1 vòng
        //        return info.normalizedTime >= normalizedEnd;
        //    }, cancellationToken: ct);

        //    onFinish?.Invoke();
        //}
        public static async UniTask PlayAnimationAsyncOld(
            this Animator animator,
            string stateName,
            int layer,
            CancellationToken ct = default,
            Func<bool> onPlay = null,
            Action onStart = null
            )
                {
            if(animator==null || animator.IsDestroyed() || animator.gameObject.IsDestroyed()) return;

            animator.ResetAllOverideLayers();
            animator.SetLayerWeight(layer, 1f);

            animator.Play(stateName, layer, 0f);
            onStart?.Invoke();

            await UniTask.Yield(PlayerLoopTiming.Update, ct);

            await UniTask.WaitUntil(() =>
            {
                if (onPlay != null)
                {
                   if(!onPlay())
                    {
                        return  true; // muon stop anim ngay tai day
                    }
                }
                if (animator == null || animator.IsDestroyed() || animator.gameObject.IsDestroyed()) return true;
                var info = animator.GetCurrentAnimatorStateInfo(layer);
                return info.IsName(stateName) && info.normalizedTime >= 1f;
            }, cancellationToken: ct);
        }
        public static async UniTask PlayAnimationAsync(
    this Animator animator,
    string stateName,
    int layer,
    CancellationToken ct = default,
    Func<bool> onPlay = null,
    Action onStart = null)
        {
            if (animator == null || animator.IsDestroyed()) return;

            // 1. Setup Layer
            animator.ResetAllOverideLayers(); // Giả định đây là extension của bạn
            animator.SetLayerWeight(layer, 1f);

            // 2. Start
            animator.Play(stateName, layer, 0f);
            animator.speed = 1f; // Đảm bảo speed là 1 khi bắt đầu
            onStart?.Invoke();

            // 3. Đợi 1 frame để Animator chuyển sang State mới
            await UniTask.Yield(PlayerLoopTiming.Update, ct);

            // 4. Chờ cho đến khi xong hoặc bị ngắt bởi onPlay
            await UniTask.WaitUntil(() =>
            {
                if (animator == null || animator.IsDestroyed()) return true;

                // Kiểm tra điều kiện dừng từ bên ngoài (onPlay)
                if (onPlay != null && !onPlay())
                {
                    animator.speed = 0f; // Khựng anim lại ngay lập tức
                    return true;
                }

                var info = animator.GetCurrentAnimatorStateInfo(layer);

                // Kiểm tra xem đã đúng State chưa (tránh trường hợp đang transition)
                bool isCorrectState = info.IsName(stateName);

                // Nếu không Loop: kết thúc khi >= 1. Nếu có Loop: sẽ kết thúc ở cuối vòng lặp đầu tiên
                return isCorrectState && info.normalizedTime >= 1f;

            }, PlayerLoopTiming.Update, ct);
        }
    }
    public static class IKManagerExt
    {

        public static async UniTask MoveTargetDirectTestAsync(this IKManager2D ik, Transform target, Vector2 pos, CancellationToken token = default)
        {
            // 1. Chuyển đổi pos từ World Space sang Local Space của cha cái Target 
            // (Điều này giúp loại bỏ ảnh hưởng của việc Rotation/Scale của cha)
            Vector3 localGoal = target.parent != null
                ? target.parent.InverseTransformPoint(new Vector3(pos.x, pos.y, target.position.z))
                : new Vector3(pos.x, pos.y, target.position.z);

            while (true)
            {
                if (token.IsCancellationRequested) return;

                // 2. Gán vào localPosition để đảm bảo không bị trôi do rotation của cha
                target.localPosition = localGoal;

                // 3. ÉP IK cập nhật (Đây là phần quan trọng nhất)
                // Vì trong ảnh bạn tick "Always Update", nhưng nó vẫn cần nhịp trigger
                ik.UpdateManager();

                // 4. Đợi ở nhịp Update (Thay vì LastUpdate để không bị trễ sau IK)
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                if (Vector3.Distance(target.localPosition, localGoal) < 0.01f) break;
            }
        }
        //public static async UniTask MoveTargetDirectTestAsync(this IKManager2D ik, Transform target, Vector2 pos, CancellationToken token = default)
        //{
        //    Vector3 targetPos = new Vector3(pos.x, pos.y, target.position.z);

        //    while (true)
        //    {
        //        if (token.IsCancellationRequested) return;

        //        // ĐỢI TRƯỚC: Đợi cho Animator và mọi thứ chạy xong xuôi
        //        await UniTask.Yield(PlayerLoopTiming.LastUpdate, token);

        //        // GÁN SAU: Lúc này là cơ hội cuối cùng trước khi Render
        //        target.position = targetPos;

        //        // ÉP IK Manager giải thuật toán ngay lập tức
        //        ik.UpdateManager();

        //        // Kiểm tra xem lần này Unity có còn dám giật lại không
        //        Debug.Log($"Check: Real Pos: {target.position} | Goal: {targetPos}");

        //        if (Vector3.Distance(target.position, targetPos) < 0.05f) break;
        //    }
        //}
        public static async UniTask MoveTargetToPointAsync(this IKManager2D ik, Transform target, Vector2 pos, float duration = 1f, CancellationToken token = default)
        {
            var solver = ik.solvers.Find(s => s.GetChain(0).target == target);
            Vector3 startPos = target.position;
            Vector3 targetPos = new Vector3(pos.x, pos.y, target.position.z);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (token.IsCancellationRequested) return;

                elapsed += Time.deltaTime;
                float percent = Mathf.Clamp01(elapsed / duration);
                float curve = Mathf.SmoothStep(0, 1, percent);

                // Đợi Animator chạy xong
                await UniTask.Yield(PlayerLoopTiming.LastUpdate, token);

                // Gán vị trí ngay tại LastUpdate
                target.position = Vector3.Lerp(startPos, targetPos, curve);

                // Cưỡng bức Solver giải thuật toán
                if (solver != null)
                {
                    // Mẹo: Tắt đi bật lại weight rất nhỏ để ép Solver Resolve
                    float oldWeight = solver.weight;
                    ik.UpdateManager();
                }
            }
            target.position = targetPos;
            ik.UpdateManager();
        }
        //public static async UniTask MoveTargetToPointAsync(this IKManager2D ik, Transform target, Vector2 pos, float duration = 1f, CancellationToken token = default)
        //{
        //    if (target == null || ik == null) return;

        //    // 1. Tìm Solver điều khiển target này
        //    // Sử dụng GetChain(0).target để khớp chính xác
        //    var solver = ik.solvers.Find(s => s.GetChain(0).target == target);
        //    if (solver == null) return;

        //    Vector3 startPos = target.position;
        //    Vector3 targetPos = new Vector3(pos.x, pos.y, target.position.z);
        //    float elapsed = 0f;

        //    while (elapsed < duration)
        //    {
        //        if (token.IsCancellationRequested) return;

        //        elapsed += Time.deltaTime;
        //        float percent = Mathf.Clamp01(elapsed / duration);
        //        float curve = Mathf.SmoothStep(0, 1, percent);

        //        // 2. Cập nhật vị trí cho Target
        //        target.position = Vector3.Lerp(startPos, targetPos, curve);

        //        // 3. ÉP CẬP NHẬT (Force Update)
        //        // Thay vì UpdateComponent, ta sử dụng thuộc tính weight để đánh dấu "Dirty"
        //        // Hoặc gọi trực tiếp UpdateManager của IKManager2D
        //        ik.UpdateManager();
        //        Debug.Log($"MoveTargetToPointAsync={target.position}");
        //        // 4. Đợi khung hình tiếp theo tại nhịp LateUpd ate
        //        // Đây là nơi IK 2D của Unity thường thực hiện việc Resolve
        //        await UniTask.Yield(PlayerLoopTiming.LastUpdate, token);

        //        // Sau khi đợi, gán lại một lần nữa để chống Animator ghi đè
        //        target.position = Vector3.Lerp(startPos, targetPos, curve);
        //    }

        //    // 5. Kết thúc
        //    target.position = targetPos;
        //    ik.UpdateManager();
        //}
        //public static async UniTask MoveTargetToPointAsync(this IKManager2D ik, Transform target, Vector2 pos, float duration = 1f, CancellationToken token = default)
        //{
        //    // 1. Kiểm tra an toàn
        //    if (target == null || ik == null) return;
        //    var solver = ik.solvers.Find(s => s.GetChain(0).target == target);

        //    // Ép Solver tính toán lại từ đầu
        //    if (solver != null) solver.Initialize();
        //    Vector3 startPos = target.position;
        //    // Giữ nguyên Z để không làm hỏng tính toán của Solver 2D
        //    Vector3 targetPos = new Vector3(pos.x, pos.y, target.position.z);
        //    float elapsed = 0f;

        //    while (elapsed < duration)
        //    {
        //        if (token.IsCancellationRequested) return;

        //        elapsed += Time.deltaTime;
        //        float percent = Mathf.Clamp01(elapsed / duration);
        //        float curve = Mathf.SmoothStep(0, 1, percent);
        //        target.gameObject.transform.hasChanged = false;
        //        // 2. Cập nhật vị trí
        //        target.position = Vector3.Lerp(startPos, targetPos, curve);

        //        // 3. Cập nhật IK - Đây là phần quan trọng
        //        // Đôi khi UpdateManager không đủ, ta cần đảm bảo các solver được đánh dấu là "bẩn" (dirty)
        //        ik.UpdateManager();

        //        // 4. Chờ đến LastUpdate - Đây là thời điểm VÀNG
        //        // Nó chạy SAU Animator nhưng TRƯỚC khi Render
        //        await UniTask.Yield(PlayerLoopTiming.LastUpdate, token);

        //        // Nếu sau khi Yield mà thấy target bị Animator kéo lại, 
        //        // ta gán lại một lần nữa ngay tại đây.
        //        target.position = Vector3.Lerp(startPos, targetPos, curve);
        //    }

        //    // 5. Đảm bảo kết thúc chính xác
        //    target.position = targetPos;
        //    ik.UpdateManager();
        //}
    }
}

