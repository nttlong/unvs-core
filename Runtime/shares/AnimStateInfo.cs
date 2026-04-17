using System;
using UnityEditor;
using UnityEngine;
using unvs.game2d.scenes;
using unvs.game2d.scenes.actors;

namespace unvs.shares
{
    //[Serializable]
    public partial class AnimStateInfo:unvs.game2d.scenes.UnvsBaseComponent
    {
      
      
        public string motionName;
        public float value;
        public string layerName;
        public Animator animationController
        {
            get
            {
                return this.GetComponentInParent<Animator>();
            }
        }
        public string paramName;
        public int layerIndex;
        public string blendName;
        public int blendIndex;
        public Motion clip;
    }
#if UNITY_EDITOR
    public partial class AnimStateInfo : unvs.game2d.scenes.UnvsBaseComponent
    {
        [UnvsButton("Play")]
        public void PlayInEditor()
        {
            GetComponentInParent<UnvsAnimStates>().EditotPlay(this);
            
        }
        [UnvsButton("Review")]
        public void PlayOutEditor()
        {
            unvs.shares.editor.UnvsEditorUtils.EditorOpenClip(GetComponentInParent<Animator>().gameObject, AssetDatabase.GetAssetPath(this.clip));
            //EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
        }
        
    }
#endif
}