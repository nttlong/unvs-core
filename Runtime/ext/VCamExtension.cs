using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using unvs.interfaces;

namespace unvs.ext
{
    public static class VCamExtension
    {
        public static void Watch(this CinemachineCamera vcam, ICamWacher camWatcher)
        {
            vcam.Watch((camWatcher  as MonoBehaviour).transform );
        }
        public static void Watch(this CinemachineCamera vcam, Transform camWatcher)
        {


            vcam.Target.TrackingTarget = camWatcher;
            vcam.Follow = camWatcher;
            vcam.LookAt = camWatcher;
            vcam.Target.TrackingTarget = camWatcher;
            vcam.Target.LookAtTarget = camWatcher;
            WatchPosImediately(vcam, camWatcher);
        }

        public static void WatchPosImediately(this CinemachineCamera vcam, Transform camWatcher)
        {
            vcam.OnTargetObjectWarped(camWatcher, camWatcher.position - vcam.transform.position);

            // 3. (Tùy chọn) Force camera cập nhật ngay trong Frame này
            vcam.ForceCameraPosition(camWatcher.position, vcam.transform.rotation);
            vcam.transform.position = vcam.transform.position + new Vector3(0, 0, -10);
        }

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

            // 2. Xử lý Confiner2D (Nguyên nhân chính gây trượt)
            var confiner = vcam.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                // Tạm thời tắt Damping để chặn đứng sự trượt
                float originalDamping = confiner.Damping;
                confiner.Damping = 0;

                // Ép tính toán lại vùng bao dựa trên Size mới
                confiner.InvalidateBoundingShapeCache();

                // 3. Ép Camera nhảy tới vị trí đúng ngay lập tức
                // Hàm này sẽ xóa bỏ trạng thái "mượt" của frame cũ
                vcam.ForceCameraPosition(vcam.transform.position, vcam.transform.rotation);

                // (Tùy chọn) Nếu bạn muốn mượt lại sau đó, dùng Invoke hoặc chờ 1 frame
                // Nhưng thông thường khi chuyển vùng, ta để Damping thấp là tốt nhất
            }
            else
            {
                // Nếu không có confiner, chỉ cần ForcePosition là đủ
                vcam.ForceCameraPosition(vcam.transform.position, vcam.transform.rotation);
            }
            var bodyCam = Camera.main.GetComponentInChildren<ICamBody>();
            if(bodyCam != null)
            {
                bodyCam.UpdateSizeByLensSettings(vcam.Lens);
            }
        }
        public static void UpdateDamping(this CinemachineCamera vcam, float DampingValue = 5f)
        {
            var confiner = vcam.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                confiner.Damping = DampingValue;
                confiner.InvalidateBoundingShapeCache();
            }
        }

        /// <summary>
        /// Thay đổi Zoom mượt mà (Dùng kèm với Coroutine hoặc Tweening)
        /// </summary>
        public static float GetOrthoSize(this CinemachineCamera vcam)
        {
            return vcam.Lens.OrthographicSize;
        }
        public static async UniTask SetOrthoSizeSmoothlyAsync(
        this CinemachineCamera vcam,
        float targetSize,
        float duration = -1f,
        float zoomSpeed = 3f,
        CancellationToken cancellationToken = default)
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
                    await UniTask.Yield(PlayerLoopTiming.PreLateUpdate, cancellationToken);
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
            }
            catch (System.OperationCanceledException) { throw; }
        }


    }
}