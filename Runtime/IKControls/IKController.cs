using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using unvs.ext;

namespace Assets.Script.unvs.IKControls
{
    public class IKController : MonoBehaviour
    {

        public Transform rootBone;
        public Transform IKControls;
        private void Awake()
        {
           
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) return;
            IKControls = this.AddChildComponentIfNotExist<Transform>("IKControls");
            rootBone = FindRootBone();

        }
#endif
        private Transform FindRootBone()
        {
            
           
            var spriteSkins = this.GetComponentsInChildren<SpriteSkin>(true);
            if (IKControls == null) return null;
            spriteSkins.CreateIK(IKControls.transform);
            return null; 
        }
    }
}