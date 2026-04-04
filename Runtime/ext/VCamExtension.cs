using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using unvs.gameword;
using unvs.interfaces;

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
        public static void Watch(this CinemachineCamera vcam, ICamWacher camWatcher)
        {
            vcam.Watch((camWatcher as MonoBehaviour).transform);
        }
        public static void Watch(this CinemachineCamera vcam, Transform camWatcher)
        {


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
            SettingsSingleScene.Instance.VCam.ForceCameraPosition(new Vector3(0, 0, -10), Quaternion.identity);
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
            var bodyCam = Camera.main.GetComponentInChildren<ICamBody>();
            if (bodyCam != null)
            {
                bodyCam.UpdateSizeByLensSettings(vcam.Lens);
            }
            

        }
      

        /// <summary>
        /// Thay đổi Zoom mượt mà (Dùng kèm với Coroutine hoặc Tweening)
        /// </summary>
        public static float GetOrthoSize(this CinemachineCamera vcam)
        {
            return vcam.Lens.OrthographicSize;
        }
        public static void UpdateByScenePrefab(this CinemachineCamera vcam, IScenePrefab scene)
        {
            var state = vcam.AddComponentIfNotExist<CamStateObject>();


            if (!scene.CameraOffsetFolow.IsEmpty)
            {
                Vector3 v = CreateOffsetCameraFollowOffset(scene);
                vcam.GetComponent<CinemachineFollow>().FollowOffset = v;
                if (state.isInProgress)
                {
                    state.isInteruptValue = true;
                    state.offSetValue = v;
                }
            }
            vcam.SetOrthoSizeImmediate(scene.OrthographicSize);
        }
        public static async UniTask ChangeFollowOffsetSmoothAsync(
      this CinemachineCamera vcam,
      IScenePrefab scene,
      CancellationToken cancellationToken,
      float duration = 1.0f)
        {
            var f = vcam.GetComponent<CinemachineFollow>();
            if (f == null) return;

            var state = vcam.AddComponentIfNotExist<CamStateObject>();
            Vector3 oldOffset = f.FollowOffset;
            Vector3 targetOffset = CreateOffsetCameraFollowOffset(scene);

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
                        state.isInProgress=false;
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
                 
                } else
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vcam"></param>
        /// <param name="scene"></param>
        /// <param name="duration"></param>
        /// <param name="zoomSpeed"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async UniTask UpdateByScenePrefabAsync(this CinemachineCamera vcam, IScenePrefab scene,
        float duration = -1f,
        float zoomSpeed = 3f,
        CancellationToken cancellationToken = default)
        {
            vcam.ChangeFollowOffsetSmoothAsync(scene, cancellationToken, duration).Forget();

            await SetOrthoSizeSmoothlyAsync(vcam, scene.OrthographicSize, duration, zoomSpeed, cancellationToken, (size) =>
            {

            });
            //vcam.GetComponent<CinemachineFollow>().FollowOffset = scene.CameraOffsetFolow.CalculateOffset(scene.OrthographicSize);
        }
        public static Vector3 CreateOffsetCameraFollowOffset(this IScenePrefab scene)
        {
            float x = 0;
            float y = 0;
            if (!scene.CameraOffsetFolow.x.IsEmpty)
            {
                x = scene.CameraOffsetFolow.x.OffsetValue;
                if (scene.CameraOffsetFolow.x.ByRatio)
                {
                    x = scene.OrthographicSize * x;
                }
            }
            if (!scene.CameraOffsetFolow.y.IsEmpty)
            {
                y = scene.CameraOffsetFolow.y.OffsetValue;
                if (scene.CameraOffsetFolow.y.ByRatio)
                {
                    y = scene.OrthographicSize * y;
                }
            }
            var v = new Vector3(x, y, -10);
            return v;
        }

        public static async UniTask SetOrthoSizeSmoothlyAsync(
        this CinemachineCamera vcam,
        float targetSize,
        float duration = -1f,
        float zoomSpeed = 3f,
        CancellationToken cancellationToken = default,
        Action<float> OnUpdating = null)
        {
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
                var body = Camera.main.GetComponentInChildren<ICamBody>();
                if (body != null)
                {
                    body.UpdateSizeByLensSettings(vcam.Lens);
                }
                OnUpdating?.Invoke(targetSize);
            }
            catch (System.OperationCanceledException) { throw; }
        }


    }
}