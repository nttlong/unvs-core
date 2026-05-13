using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace unvs.data
{
    [CreateAssetMenu(fileName = "SceneReviewSetting", menuName = "Unvs/Data/Scene Review Setting")]
    public class UnvsScenePreviewSettings : UnvsScriptObject
    {
#if UNITY_EDITOR
        // Biến này dùng để kéo thả file Scene vào Inspector
        public SceneAsset TestScene;
#endif

        // Nếu bạn cần tên scene để load lúc runtime
        [HideInInspector]
        public string sceneName;

        // Tự động cập nhật tên scene khi bạn kéo thả file vào
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (TestScene != null)
            {
                sceneName = TestScene.name;
            }
#endif
        }
    }
}