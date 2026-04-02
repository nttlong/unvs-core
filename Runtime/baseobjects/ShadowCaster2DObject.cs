using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering;
using unvs.actions;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
using unvs.sys;
namespace unvs.baseobjects
{
    public class ShadowCaster2DObject : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            this.ApplyShadowCasterForAllSpriteRenderers();
        }
#endif
    }   
}