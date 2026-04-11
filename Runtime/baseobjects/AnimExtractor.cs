//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Localization;
//using UnityEngine.Rendering;
//using unvs.actions;
//using unvs.actors;
//using unvs.ext;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.baseobjects
//{
//    [ExecuteInEditMode]
//    public class AnimationExtractor : MonoBehaviour
//    {
//        [SerializeField]
//        public BlendTreeInfo[] blendTreeAnim = new BlendTreeInfo[] { };


//        [SerializeField]
//        public BlendTreeInfo[] motionsAnim = new BlendTreeInfo[] { };

//        public virtual void PlayBaselayerMotion(string motionName)
//        {

//            this.blendTreeAnim?.PlayBaseLayer(motionName);
//        }
//        public virtual void PlayBlendTreeMotion(string layername, string blendName, string motionName)
//        {
//            this.blendTreeAnim.PlayBlendTree(layername, blendName, motionName);

//        }
//        public virtual void PlayMotion(string MotionName)
//        {
//            // this.motionsAnim.PlayCrossFadeMotion("BendDownAndPickItem",0);

//            this.motionsAnim.PlayMotion("MotionName");
//        }
//        public Animator Anim ;
//#if UNITY_EDITOR
//        private void EditorTimeSetup()
//        {
//            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
//            if (Application.isPlaying) return;
//            GetComponentInChildren<Animator>()?.AddComponentIfNotExist<ActorAnimationEvents>();
//            if (Anim == null) Anim = GetComponentInChildren<Animator>();
//            if (Anim != null)
//            {
//                blendTreeAnim = Anim.EditorExtractBaseLayer().ToArray();



//                motionsAnim = Anim.EditorExtractAllMotions().ToArray();

//            }
//            UnityEditor.EditorUtility.SetDirty(this);

//        }



//        private void OnValidate()
//        {
//            if (Application.isPlaying) return;
//            EditorTimeSetup();
//        }


//#endif
//    }
//}