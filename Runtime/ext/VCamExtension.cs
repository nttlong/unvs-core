using Cysharp.Threading.Tasks;
using game2d.ext;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using unvs.game2d.scenes;


using unvs.shares;

namespace unvs.ext
{
    public class CamStateObject : MonoBehaviour
    {
        internal Vector3 offSetValue;
        internal bool isInteruptValue;
        internal bool isInProgress;
    }
    public static class VCamExtension
    {
        
        public static void Watch(this CinemachineCamera vcam, Transform camWatcher)
        {
            if (vcam == null)
            {
                return;
            }
            vcam.Target.TrackingTarget = null;
            vcam.Follow = null;
            vcam.LookAt = null;
            //vcam.Target.TrackingTarget = null;
            //vcam.Target.LookAtTarget = null;
            vcam.Follow = camWatcher;
            vcam.LookAt = camWatcher;

            // Force Cinemachine update ngay lập tức
            vcam.PreviousStateIsValid = false;
        }
        public static void ClearWatch(this CinemachineCamera vcam)
        {
            vcam.Target.TrackingTarget = null;
            vcam.Follow = null;
            vcam.LookAt = null;
            vcam.Target.TrackingTarget = null;
            vcam.Target.LookAtTarget = null;
            vcam.ForceCameraPosition(new Vector3(0, 0, -10), Quaternion.identity);
        }
        //public static void WatchPosImediately(this CinemachineCamera vcam, Transform camWatcher)
        //{
        //    vcam.OnTargetObjectWarped(camWatcher, camWatcher.position);

        //    // 3. (Tùy chọn) Force camera cập nhật ngay trong Frame này
        //    vcam.ForceCameraPosition(camWatcher.position, vcam.transform.rotation);
        //    vcam.transform.position = vcam.transform.position + new Vector3(0, 0, -10);
        //}

        /// <summary>
        /// Thiết lập độ rộng tầm nhìn (Zoom) cho Camera Orthographic.
        /// Giá trị càng nhỏ = Càng gần (Zoom in), Giá trị càng lớn = Càng xa (Zoom out).
        /// </summary>
        public static void SetOrthoSizeImmediate(this CinemachineCamera vcam, float size)
        {
            if (vcam == null) return;

            // 1. Cập nhật Lens Size
            LensSettings lens = vcam.Lens;
            if (lens.OrthographicSize == size) return;
            lens.OrthographicSize = size;
            vcam.Lens = lens;



        }


        /// <summary>
        /// Thay đổi Zoom mượt mà (Dùng kèm với Coroutine hoặc Tweening)
        /// </summary>
        public static float GetOrthoSize(this CinemachineCamera vcam)
        {
            if(vcam == null) { return 0; }
            return vcam.Lens.OrthographicSize;
        }
        public static CinemachineCamera UpdateOffset2D(this CinemachineCamera vcam,Vector2 Offset)
        {
            vcam.GetComponent<CinemachineFollow>().FollowOffset = new Vector3(Offset.x, Offset.y, -10);
            return vcam;
        }
        /// <summary>
        /// Setup cam an vcam
        /// </summary>
        /// <param name="vcam"></param>
        /// <param name="scene"></param>
        public static void UpdateByUnvsScene(this CinemachineCamera vcam, UnvsScene scene,float limitDistance)
        {
            var state = vcam.AddComponentIfNotExist<UnvsCamStaus>();

            if (UnvsCinema.Instance.cam.orthographic)
            {
                if (scene.followOffset != Vector2.zero)
                {

                    vcam.GetComponent<CinemachineFollow>().FollowOffset = scene.followOffset;
                    if (state.isInProgress)
                    {
                        state.isInteruptValue = true;
                        state.offSetValue = scene.followOffset;
                    }
                }
                vcam.SetOrthoSizeImmediate(scene.OrthographicSize);
                return;
            } else
            {
               
                //if (scene.followOffset.z <0)
                //{
                //    z = scene.followOffset.z;
                //}
                var z = UnvsCinema.Instance.cam.OrthoSizeToPerspectiveDistance(scene.OrthographicSize,limitDistance);
                vcam.GetComponent<CinemachineFollow>().FollowOffset = new Vector3(scene.followOffset.x, scene.followOffset.y, z);
            }
               
        }

        public static async UniTask ChangeFollowOffsetSmoothAsync(this CinemachineCamera vcam, Vector3 targetOffset, CancellationToken cancellationToken, float duration = 1.0f)
        {
            var f = vcam.GetComponent<CinemachineFollow>();
            if (f == null) return;

            var state = vcam.AddComponentIfNotExist<CamStateObject>();
            Vector3 oldOffset = f.FollowOffset;

            // Sử dụng using cho cả hai để đảm bảo giải phóng tài nguyên hệ thống
            using var userCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(userCts.Token, cancellationToken);

            // Lấy token tổng hợp để sử dụng xuyên suốt
            var linkedToken = linkedCts.Token;

            float elapsed = 0;
            state.isInProgress = true;

            try
            {
                while (elapsed < duration)
                {
                    // Kiểm tra token tổng hợp trước
                    linkedToken.ThrowIfCancellationRequested();

                    if (state.isInteruptValue)
                    {
                        f.FollowOffset = state.offSetValue;
                        state.isInteruptValue = false;
                        state.isInProgress = false;
                        return; // Thoát ra, khối 'finally' sẽ lo việc reset isInProgress
                    }

                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    float step = Mathf.SmoothStep(0, 1, t);

                    f.FollowOffset = Vector3.Lerp(oldOffset, targetOffset, step);

                    // Truyền linkedToken vào đây để UniTask tự ngắt nếu 1 trong 2 nguồn bị hủy
                    await UniTask.Yield(PlayerLoopTiming.Update, linkedToken);
                }

                f.FollowOffset = targetOffset;
            }
            catch (OperationCanceledException)
            {
                if (state.isInteruptValue)
                {
                    f.FollowOffset = state.offSetValue;
                    state.isInteruptValue = false;

                }
                else
                {
                    if (f != null) f.FollowOffset = oldOffset;
                }

            }
            finally
            {
                // Dùng finally để đảm bảo dù chạy xong, bị lỗi, hay bị cancel thì flag vẫn về false
                if (state != null) state.isInProgress = false;
            }
        }


        public static async UniTask SetOrthoSizeSmoothlyAsync(this CinemachineCamera vcam, float targetSize, float duration = -1f, float zoomSpeed = 3f, CancellationToken cancellationToken = default, Action<float> OnUpdating = null)
        {
            if(targetSize==0) return;
            try
            {
                if (vcam == null) return;

                float startSize = vcam.Lens.OrthographicSize;

                // --- TÍNH TOÁN DURATION ---
                if (duration < 0)
                {
                    float sizeDifference = Mathf.Abs(targetSize - startSize);
                    duration = Mathf.Clamp(sizeDifference / zoomSpeed, 0.4f, 2.0f); // Tăng min lên 0.4s để mượt hơn
                }

                if (Mathf.Approximately(startSize, targetSize)) return;

                float elapsed = 0f;
                var confiner = vcam.GetComponent<CinemachineConfiner2D>();

                // TẮT TẠM THỜI SMOOTHING CỦA CONFINER ĐỂ TRÁNH XUNG ĐỘT KHI ZOOM
                float originalDamping = 0;
                if (confiner != null)
                {
                    originalDamping = confiner.Damping;
                    confiner.Damping = 0; // Chống trễ gây giật hình
                }

                while (elapsed < duration)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);

                    // Dùng SmoothStep để khử gia tốc gắt
                    float curveT = Mathf.SmoothStep(0, 1, t);
                    float currentSize = Mathf.Lerp(startSize, targetSize, curveT);

                    LensSettings lens = vcam.Lens;
                    lens.OrthographicSize = currentSize;

                    vcam.Lens = lens;

                    if (confiner != null)
                    {
                        // QUAN TRỌNG: Chỉ Invalidate khi thực sự cần thiết 
                        // hoặc mỗi 2 frame để giảm tải CPU cho MSI Cyborg
                        if (Time.frameCount % 2 == 0)
                        {
                            confiner.InvalidateBoundingShapeCache();
                        }
                    }

                    // Dùng PreLateUpdate để đảm bảo Camera tính toán xong TRƯỚC KHI Render
                    await UniTask.Yield(PlayerLoopTiming.PreUpdate, cancellationToken);
                    OnUpdating?.Invoke(currentSize);
                }

                // KẾT THÚC: Đảm bảo trị số chuẩn và trả lại Damping
                LensSettings finalLens = vcam.Lens;
                finalLens.OrthographicSize = targetSize;
                vcam.Lens = finalLens;

                if (confiner != null)
                {
                    confiner.InvalidateBoundingShapeCache();
                    confiner.Damping = originalDamping;
                }

                OnUpdating?.Invoke(targetSize);
            }
            catch (System.OperationCanceledException) { throw; }
        }


        public static async UniTask SetFollowOffsetSmoothlyAsync(this CinemachineCamera vcam, Vector3 targetFollowOffset, float moveSpeed = 3f, float duration = -1f, CancellationToken cancellationToken = default, Action<Vector3> OnUpdating = null)
        {
            if (vcam == null) return;

            try
            {
                // Lấy trực tiếp từ vcam đang gọi extension
                var follow = vcam.GetComponent<CinemachineFollow>();
                if (follow == null) return;

                Vector3 startOffset = follow.FollowOffset;

                // Kiểm tra khoảng cách bao gồm cả trục Z
                if (Vector3.Distance(startOffset, targetFollowOffset) < 0.001f)
                    return;

                if (duration < 0f)
                {
                    float diff = Vector3.Distance(startOffset, targetFollowOffset);
                    duration = Mathf.Clamp(diff / moveSpeed, 0.4f, 2f);
                }

                float elapsed = 0f;

                while (elapsed < duration)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    elapsed += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

                    Vector3 currentOffset = Vector3.Lerp(startOffset, targetFollowOffset, t);

                    // GÁN TRỰC TIẾP
                    follow.FollowOffset = currentOffset;

                    // Dùng Update thay vì PreUpdate để đồng bộ với logic game
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                    OnUpdating?.Invoke(currentOffset);
                }

                follow.FollowOffset = targetFollowOffset;
                OnUpdating?.Invoke(targetFollowOffset);
            }
            catch (System.OperationCanceledException) { throw; }
        }

        public static float OrthoSizeToPerspectiveDistance(this Camera cam, float orthoSize, float limitDistance)
        {
            if (cam.orthographic) return 0f; // Hoặc xử lý theo logic riêng của bạn

            // fieldOfView của Unity là Vertical FOV (góc mở theo chiều dọc)
            float halfFovRad = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;

            // Tính toán khoảng cách dương
            float distance = orthoSize / Mathf.Tan(halfFovRad);

            // Nếu bạn muốn trả về giá trị để gán cho Camera Offset Z (thường là lùi ra sau)
            return -distance-limitDistance;
        }

    }
}