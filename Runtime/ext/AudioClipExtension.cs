using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using unvs.shares;

namespace unvs.ext
{
    public static class AudioClipExtension
    {
        /// <summary>
        /// Phát clip1 nếu không null, ngược lại phát clip2. 
        /// Hàm sẽ đợi cho đến khi clip kết thúc mới hoàn tất task.
        /// </summary>
        public static async UniTask PlayBetterAudioClipAsync(this AudioSource source, AudioClip clip1, AudioClip clip2, float volumScale = 1f)
        {

            // 1. Xác định clip nào sẽ được phát
            AudioClip clipToPlay = clip1 != null ? clip1 : clip2;

            // Nếu cả hai đều null thì không làm gì cả
            if (clipToPlay == null)
            {
                Debug.LogWarning("Cả hai AudioClip đều null!");
                return;
            }

            // 2. Tạo một GameObject tạm thời để phát âm thanh
            // Cách này đảm bảo âm thanh không bị ngắt nếu object gọi hàm bị destroy

            source.PlayOneShot(clipToPlay);


            // 4. Chờ cho đến khi clip phát xong
            // Chúng ta sử dụng UniTask.Delay để đợi theo độ dài của clip
            await UniTask.Delay(TimeSpan.FromSeconds(clipToPlay.length), delayTiming: PlayerLoopTiming.Update);

        }
        public static void PlayBetterAudioClip(this AudioSource source, AudioClip clip1, AudioClip clip2, float volumScale)
        {

            // 1. Xác định clip nào sẽ được phát
            AudioClip clipToPlay = clip1 != null ? clip1 : clip2;

            // Nếu cả hai đều null thì không làm gì cả
            if (clipToPlay == null)
            {
                Debug.LogWarning("Cả hai AudioClip đều null!");
                return;
            }

            // 2. Tạo một GameObject tạm thời để phát âm thanh
            // Cách này đảm bảo âm thanh không bị ngắt nếu object gọi hàm bị destroy

            source.PlayOneShot(clipToPlay, volumScale);


            // 4. Chờ cho đến khi clip phát xong
            // Chúng ta sử dụng UniTask.Delay để đợi theo độ dài của clip
            //await UniTask.Delay(TimeSpan.FromSeconds(clipToPlay.length), delayTiming: PlayerLoopTiming.Update);

        }
        public static void Play(this AudioSource source, AudioClip clip1, float? scale = 1f)
        {
            if (clip1 == null) return;

            source.PlayOneShot(clip1, scale ?? 1);
        }
        public static void PlayFirstNotNull(this AudioSource audioSource, AudioClip primarySource, params AudioClip[] otherSources)
        {
            // 1. Kiểm tra chính source gọi hàm đầu tiên
            if (primarySource != null)
            {
                audioSource.PlayBetterAudioClipAsync(primarySource, null).Forget();
                return;
            }

            // 2. Nếu primarySource null, duyệt qua danh sách params
            if (otherSources != null)
            {
                foreach (var source in otherSources)
                {
                    if (source != null)
                    {
                        audioSource.Play(source);
                      
                        return; // Thoát ngay sau khi tìm thấy và phát source đầu tiên
                    }
                }
            }

            // 3. Tùy chọn: Log cảnh báo nếu không tìm thấy cái nào để phát
            // UnityEngine.Debug.LogWarning("Không có AudioSource nào hợp lệ để phát!");
        }
    }


    public static class AudioSourceExtensions
    {
        public static async UniTask CrossFadeAmbientAsync(
        this AudioSource currentSource,
        AudioSource newSource,
        float duration = 2.5f,
        float finalVolume = -1f, // Thêm tùy chọn set cứng volume
        CancellationToken cancellationToken = default)
        {
            // 1. Nếu cả hai giống hệt nhau về tham chiếu, chỉ đảm bảo nó đang Play
            if (currentSource == newSource && currentSource != null)
            {
                if (!currentSource.isPlaying) currentSource.Play();
                return;
            }

            // 2. Xác định Volume mục tiêu (Ưu tiên finalVolume truyền vào, nếu không lấy từ Inspector)
            // Nếu Inspector để 0.204 như trong hình, nó sẽ lấy đúng 0.204
            float targetVol = finalVolume >= 0 ? finalVolume : (newSource != null ? newSource.volume : 0f);

            // Nếu target quá nhỏ (gần bằng 0), hãy mặc định nó là 0.5f để tránh mất tiếng
            if (targetVol <= 0.01f && newSource != null) targetVol = 0.5f;

            // 3. Thực hiện Fade Out thằng cũ (nếu có)
            if (currentSource != null && currentSource.isPlaying)
            {
                // Chạy song song Fade Out và Fade In
                FadeOutAndStop(currentSource, duration, cancellationToken).Forget();
            }

            // 4. Thực hiện Fade In thằng mới (nếu có)
            if (newSource != null)
            {
                newSource.volume = 0;
                newSource.Play();

                float elapsed = 0f;
                while (elapsed < duration)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    elapsed += Time.deltaTime;
                    newSource.volume = Mathf.Lerp(0, targetVol, Mathf.SmoothStep(0, 1, elapsed / duration));
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
                newSource.volume = targetVol; // Chốt hạ volume, không cho phép biến mất
            }
        }

        public static async UniTask FadeOutAndStop(this AudioSource source, float duration, CancellationToken token)
        {
            float startVol = source.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (token.IsCancellationRequested) break;
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVol, 0, Mathf.SmoothStep(0, 1, elapsed / duration));
                await UniTask.Yield(token);
            }
            source.Stop();
            source.volume = startVol; // Reset lại volume để lần sau dùng lại không bị bằng 0
        }

        // --- Các hàm hỗ trợ để code sạch sẽ hơn ---

        public static async UniTask FadeInSource(this AudioSource source, float duration, CancellationToken token)
        {
            float target = source.volume > 0 ? source.volume : 1f;
            source.volume = 0;
            source.Play();

            float elapsed = 0;
            while (elapsed < duration)
            {
                if (token.IsCancellationRequested) break;
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(0, target, Mathf.SmoothStep(0, 1, elapsed / duration));
                await UniTask.Yield(token);
            }
            source.volume = target;
        }

        public static async UniTask FadeOutSource(AudioSource source, float duration, CancellationToken token)
        {
            float start = source.volume;
            float elapsed = 0;
            while (elapsed < duration)
            {
                if (token.IsCancellationRequested) break;
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(start, 0, Mathf.SmoothStep(0, 1, elapsed / duration));
                await UniTask.Yield(token);
            }
            source.Stop();
        }
        public static async UniTask CrossFadeAmbientAsync(
            this AudioSource currentSource,
            AudioClip newClip,
            float targetVolume = 1.0f,
            float duration = 2.5f,
            CancellationToken cancellationToken = default)
        {
            if (currentSource == null) return;

            // 1. Trường hợp: Đã đang phát đúng Clip đó rồi
            if (currentSource.clip == newClip && currentSource.isPlaying)
            {
                // Chỉ cần chỉnh lại Volume nếu nó khác target
                await currentSource.FadeVolumeOnlyAsync(targetVolume, duration, cancellationToken);
                return;
            }

            float startVolume = currentSource.volume;

            // 2. GIAI ĐOẠN 1: FADE OUT (Nếu đang có nhạc cũ phát)
            if (currentSource.isPlaying && currentSource.clip != null)
            {
                float elapsedOut = 0f;
                float fadeOutDuration = duration * 0.5f; // Dành 50% time để fade out

                while (elapsedOut < fadeOutDuration)
                {
                    if (cancellationToken.IsCancellationRequested) return;
                    elapsedOut += Time.deltaTime;

                    // Dùng SmoothStep để âm thanh lịm dần tự nhiên
                    currentSource.volume = Mathf.Lerp(startVolume, 0, Mathf.SmoothStep(0, 1, elapsedOut / fadeOutDuration));
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
                currentSource.Stop();
            }

            // 3. GIAI ĐOẠN 2: THIẾT LẬP CLIP MỚI
            currentSource.clip = newClip;
            currentSource.volume = 0;

            // Nếu clip mới là null, coi như xong (đã tắt nhạc cũ thành công)
            if (newClip == null) return;

            // 4. GIAI ĐOẠN 3: FADE IN (Chuẩn điện ảnh)
            currentSource.Play();
            float elapsedIn = 0f;
            float fadeInDuration = duration * 0.5f;

            while (elapsedIn < fadeInDuration)
            {
                if (cancellationToken.IsCancellationRequested) return;
                elapsedIn += Time.deltaTime;

                // Fade in nhẹ nhàng bằng SmoothStep
                currentSource.volume = Mathf.Lerp(0, targetVolume, Mathf.SmoothStep(0, 1, elapsedIn / fadeInDuration));
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            currentSource.volume = targetVolume;
        }

        private static async UniTask FadeVolumeOnlyAsync(this AudioSource source, float target, float duration, CancellationToken token)
        {
            float start = source.volume;
            float elapsed = 0;
            while (elapsed < duration)
            {
                if (token.IsCancellationRequested) return;
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(start, target, Mathf.SmoothStep(0, 1, elapsed / duration));
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            source.volume = target;
        }
    }
}