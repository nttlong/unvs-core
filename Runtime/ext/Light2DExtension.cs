using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace unvs.ext
{
    public enum UpdateWorldEmun
    {
        Left,
        Right,
        Interior,
        New
    }
    [Serializable]
    public struct GlobalLightChunkInfo
    {
        public float intensity;
        public Color color;
        public Vector2 position;
        public DateTime createdOn;
       

       
    }
    public static class Light2DExtension
    {
        
        public static void CopyFrom(this Light2D source, Light2D another)
        {
            if (source == null || another == null) return;

            // 1. Các thuộc tính cơ bản (Public)
            source.lightType = another.lightType;
            source.color = another.color;
            source.intensity = another.intensity;
            source.blendStyleIndex = another.blendStyleIndex;
            source.lightOrder = another.lightOrder;

            // 2. Hình dạng
            source.pointLightInnerAngle = another.pointLightInnerAngle;
            source.pointLightOuterAngle = another.pointLightOuterAngle;
            source.pointLightInnerRadius = another.pointLightInnerRadius;
            source.pointLightOuterRadius = another.pointLightOuterRadius;

            if (another.lightType == Light2D.LightType.Freeform)
            {
                source.SetShapePath(another.shapePath);
            }

            //// 3. Dùng Reflection để copy Shadow và Layer Mask (Các field bị ẩn)
            //BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            //// Copy Volumetric/Shadow Volume Intensity
            //FieldInfo volField = typeof(Light2D).GetField("m_ShadowVolumeIntensity", flags);
            //if (volField != null) volField.SetValue(source, volField.GetValue(another));

            //// Copy m_ApplyToSortingLayers (Thay thế cho applyToSortingLayers)
            //FieldInfo applyLayerField = typeof(Light2D).GetField("m_ApplyToSortingLayers", flags);
            //if (applyLayerField != null) applyLayerField.SetValue(source, applyLayerField.GetValue(another));

            //// Copy Mask (targetSortingLayerIds)
            //FieldInfo maskField = typeof(Light2D).GetField("m_TargetSortingLayers", flags);
            //if (maskField != null) maskField.SetValue(source, maskField.GetValue(another));

            // 4. Cập nhật Shadow
            source.shadowsEnabled = another.shadowsEnabled;
            source.shadowIntensity = another.shadowIntensity;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(source);
#endif
        }
        /// <summary>
        /// Hàm này mix global light giua cac scene (moi scene la 1 prefab)
        /// và có global light source và nó nằm ngay tâm của cua scene
        /// Tọa độ y kg tham gia vào qua trình tính toán này, kg có giới han5n khoản cách
        /// Vì toàn bộ  Light2D[] lights là Global Light của scene, nên nó đã bị disable,
        /// do đó khi tính toán kg quan tâm đến tình trạng của nguồn sáng là đã active hay disable
        /// cứ có là tính.
        /// 
        /// </summary>
        /// <param name="camPos"></param>
        /// <param name="lights"></param>
        /// <returns></returns>
        public static MixedLightData MixGlobalLightSources(Vector2 camPos, List< GlobalLightChunkInfo> lights)
        {
            float ambientIntensity = 0.05f; // Ambient tối thiểu của game Emotion

            // 1. Phân loại và lấy danh sách đèn:
            // Không check active vì đèn Global của Scene có thể dang bị tắt
            List<GlobalLightChunkInfo> activeLights = new List<GlobalLightChunkInfo>();
            foreach (var light in lights.Cast<GlobalLightChunkInfo?>())
            {
                if (light==null) continue;
                activeLights.Add(light.Value);
            }

            // Trường hợp không có nguồn sáng nào
            if (activeLights.Count == 0)
            {
                return new MixedLightData { Color = Color.white, Intensity = ambientIntensity };
            }
            ambientIntensity = activeLights[0].intensity;
            // Trường hợp 1: Có đúng CHUẨN 1 nguồn sáng -> Trả về thẳng Data của nó (Không giảm trừ)
            if (activeLights.Count == 1)
            {
                var singleLight = activeLights[0];
                return new MixedLightData
                {
                    Color = singleLight.color,
                    Intensity = singleLight.intensity
                };
            }

            // Trường hợp 2: Có nhiều hơn 1 nguồn sáng -> Chuyển mượt khu vực dựa trên khoảng cách X
            // Áp dụng thuật toán Nội suy theo trọng số khoảng cách (Inverse Distance Weighting)
            // Càng gần trung tâm (X) của scene nào thì màu & cường độ của scene đó càng chiếm ưu thế
            float totalWeight = 0f;
            float[] weights = new float[activeLights.Count];

            for (int i = 0; i < activeLights.Count; i++)
            {
                // Chỉ tính khoảng cách chiều X (ngang), bỏ qua Y (cao/thấp)
                float distX = Mathf.Abs(camPos.x - activeLights[i].position.x);
                
                // Trọng số tỷ lệ nghịch với khoảng cách. Cộng thêm epsilon nhỏ (0.001) để tránh lỗi chia cho 0
                // Bình phương độ dài (Pow 2) giúp việc lân la giữa 2 map có độ chuyển fade đẹp hơn
                float weight = 1f / (Mathf.Pow(distX, 2f) + 0.001f); 
                weights[i] = weight;
                totalWeight += weight;
            }

            // Tính tổng nội suy (Weighted Average)
            Vector4 finalColor = Vector4.zero;
            float finalIntensity = 0f;

            for (int i = 0; i < activeLights.Count; i++)
            {
                // Phân bổ sao cho tổng các phần trăm (normalized) đều bằng 100% (1.0f)
                float normalizedWeight = weights[i] / totalWeight; 
                
                var light = activeLights[i];
                finalColor += (Vector4)light.color * normalizedWeight;
                finalIntensity += light.intensity * normalizedWeight;
            }

            return new MixedLightData
            {
                Color = (Color)finalColor,
                Intensity = Mathf.Max(finalIntensity, ambientIntensity)
            };
        }
        public struct MixedLightData
        {
            public Color Color;
            public float Intensity;
        }
        private static Transform tempLightStore;

        public static Transform TempLightStore
        {
            get
            {
                if (tempLightStore != null) return tempLightStore;
                var go = new GameObject("TempLightStore");
                go.SetActive(false);

                tempLightStore = go.transform;
                return tempLightStore;
            }
        }

        public static Light2D CloneToNew(this Light2D source)
        {
            if (source == null) return null;

            // 1. Nhân bản toàn bộ Object (Copy 100% thuộc tính kể cả Layer ẩn)
            Light2D target = UnityEngine.Object.Instantiate(source, TempLightStore);

            // 2. Tách nó ra khỏi Scene cũ (để không bị Destroy cùng Scene)

            // 3. Đổi tên để dễ quản lý Debug
            target.gameObject.name = source.name;
            target.gameObject.SetActive(false);
            return target;
        }

        public static async UniTask TransitionIntensitySmoothlyAsync(
            this Light2D light,
            float targetIntensity,
            float duration = -1f,
            float intensityChangeSpeed = 0.5f, // Tốc độ thay đổi cường độ mặc định
            CancellationToken cancellationToken = default)
        {
            if (light == null) return;

            float startIntensity = light.intensity;

            // --- TÍNH TOÁN DURATION HỢP LÝ ---
            // Nếu chuyển đổi cường độ lớn (ví dụ từ 0.2 lên 1.5) thì cần thời gian lâu hơn
            if (duration < 0)
            {
                float intensityDiff = Mathf.Abs(targetIntensity - startIntensity);
                // Giới hạn trong khoảng 0.3s đến 2s để giữ tính điện ảnh
                duration = Mathf.Clamp(intensityDiff / intensityChangeSpeed, 0.3f, 2.0f);
            }

            if (Mathf.Approximately(startIntensity, targetIntensity)) return;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (cancellationToken.IsCancellationRequested) return;

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Sử dụng SmoothStep để ánh sáng tăng/giảm dịu mắt hơn
                // Tránh cảm giác ánh sáng bị "gắt" ở thời điểm bắt đầu/kết thúc
                light.intensity = Mathf.Lerp(startIntensity, targetIntensity, Mathf.SmoothStep(0, 1, t));

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            // Đảm bảo giá trị cuối chính xác
            light.intensity = targetIntensity;
        }
        public static async UniTask TransitionLightDirectAsync(
        this Light2D targetLight, // Đèn của Scene mới
        Light2D sourceLight,      // Đèn của Scene cũ
        float duration = 1.0f,
        CancellationToken cancellationToken = default)
        {
            if (targetLight == null) return;

            // 1. Lưu lại thông số "đích" của đèn mới từ Prefab
            float finalIntensity = targetLight.intensity;
            Color finalColor = targetLight.color;

            // 2. Lấy thông số "bắt đầu" từ đèn cũ (nếu có)
            float startIntensity = 0f;
            Color startColor = Color.black;

            if (sourceLight != null)
            {
                startIntensity = sourceLight.intensity;
                startColor = sourceLight.color;

                // TẮT NGAY đèn cũ để tránh xung đột Layer
                sourceLight.enabled = false;
                sourceLight.gameObject.SetActive(false);
            }
            targetLight.TurnOffOthers();
            // 3. Đèn mới "tiếp quản" ngay lập tức với thông số của đèn cũ
            targetLight.intensity = startIntensity;
            targetLight.color = startColor;
            targetLight.gameObject.SetActive(true);

            targetLight.enabled = true; // Lúc này chỉ có duy nhất 1 Global Light được bật

            // 4. Bắt đầu quá trình biến đổi (Fade)
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (cancellationToken.IsCancellationRequested) return;

                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);

                targetLight.intensity = Mathf.Lerp(startIntensity, finalIntensity, t);
                targetLight.color = Color.Lerp(startColor, finalColor, t);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            // 5. Chốt giá trị cuối cùng
            targetLight.intensity = finalIntensity;
            targetLight.color = finalColor;
        }

        public static Light2D[] FindAllGlobalLightIfNotSame(this Light2D targetLight)
        {
            // 1. Tìm tất cả các Light2D đang Active trong toàn bộ các Scene đang Load
            Light2D[] allLights = UnityEngine.Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
            List<Light2D> conflictLights = new List<Light2D>();

            foreach (var light in allLights)
            {
                // 2. Chỉ lọc những đèn là Global Light
                if (light.lightType == Light2D.LightType.Global)
                {
                    // 3. Nếu không phải là cái đèn targetLight ta đang giữ
                    if (light != targetLight)
                    {
                        conflictLights.Add(light);
                    }
                }
            }

            return conflictLights.ToArray();
        }
        public static void TurnOffOthers(this Light2D targetLight)
        {
            // 1. Tìm tất cả các Light2D đang Active trong toàn bộ các Scene đang Load
            Light2D[] allLights = UnityEngine.Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
            List<Light2D> conflictLights = new List<Light2D>();

            foreach (var light in allLights)
            {
                // 2. Chỉ lọc những đèn là Global Light
                if (light.lightType == Light2D.LightType.Global)
                {
                    // 3. Nếu không phải là cái đèn targetLight ta đang giữ
                    if (light != targetLight)
                    {
                        light.gameObject.SetActive(false);
                        light.enabled = false;
                    }
                }
            }


        }
    }
}