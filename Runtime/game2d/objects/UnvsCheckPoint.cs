using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.shares;

namespace unvs.game2d.objects
{
    public struct CheckPintInfo
    {
        public string scenePath;
        public string checkPointName;

    }
    [RequireComponent(typeof(SpriteRenderer))]
    public class UnvsCheckPoint : UnvsBaseComponent
    {
        private void Awake()
        {
            if(Application.isPlaying)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponent<SpriteRenderer>().sprite == null)
                GetComponent<SpriteRenderer>().sprite = Commons.LoadAsset<Sprite>("Packages/com.unvs.core/Runtime/Sprites/Circle.png");
        }
#endif
    }
}