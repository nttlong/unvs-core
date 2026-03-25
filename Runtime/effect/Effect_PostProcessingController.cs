//using Cysharp.Threading.Tasks;
//using System.Collections;
//using UnityEngine;
//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;
//using Cysharp.Threading.Tasks;
//namespace unvs.effect
//{
//    public class Effect_PostProcessingController : MonoBehaviour
//    {

//        [SerializeField] private Volume postVolume;

//        private ColorAdjustments colorAdjustments;
//        private Vignette vignette;
//        private FilmGrain filmGrain;

//        void Awake()
//        {
//            // Lấy các thành phần hiệu ứng từ Volume Profile
//            postVolume.profile.TryGet(out colorAdjustments);
//            postVolume.profile.TryGet(out vignette);
//            postVolume.profile.TryGet(out filmGrain);
//        }

//        // Hàm tạo hiệu ứng "Hoảng sợ" (Sắc nét đen trắng, nhiễu mạnh, viền tối)
//        public async UniTask SetHorrorMood(bool active, float duration = 1.0f)
//        {
//            float elapsed = 0;
//            float startSaturation = colorAdjustments.saturation.value;
//            float startVignette = vignette.intensity.value;
//            float startGrain = filmGrain.intensity.value;

//            float targetSaturation = active ? -80f : 0f; // Gần như đen trắng
//            float targetVignette = active ? 0.6f : 0.4f;   // Bo góc tối mạnh hơn
//            float targetGrain = active ? 0.8f : 0.2f;      // Nhiễu hạt cực mạnh

//            while (elapsed < duration)
//            {
//                elapsed += Time.deltaTime;
//                float t = elapsed / duration;

//                // Interpolation mượt mà
//                colorAdjustments.saturation.Override(Mathf.Lerp(startSaturation, targetSaturation, t));
//                vignette.intensity.Override(Mathf.Lerp(startVignette, targetVignette, t));
//                filmGrain.intensity.Override(Mathf.Lerp(startGrain, targetGrain, t));

//                await UniTask.Yield(); // Trả về main thread mỗi frame
//            }
//        }
//    }
//}