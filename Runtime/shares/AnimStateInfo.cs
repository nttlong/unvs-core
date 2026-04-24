using System;
using UnityEditor;
using UnityEngine;
using unvs.game2d.scenes;
using unvs.game2d.actors;
using unvs.game2d.objects.editor;

namespace unvs.shares
{
    [System.Serializable]
    public class PolygonData
    {
        public Vector2[] points;
    }
    [Serializable]
    public abstract class UnvsEditableObject
    {

    }
    public enum TrasitionEnum
    {
        Up, Down, Left, Right
    }
    [Serializable]
    public partial class AnimStateInfo : UnvsEditableObject
    {
      
      
        public string motionName;
        public float value;
        public string layerName;
        public Animator animationController;
       
        public string paramName;
        public int layerIndex;
        public string blendName;
        public int blendIndex;
        public Motion clip;
        public string name;
        public AnimStateInfo(string name, Animator animationControlle)
        {
            this.name= name;
            this.animationController= animationControlle;
        }
    }
#if UNITY_EDITOR
    public partial class AnimStateInfo : UnvsEditableObject// : unvs.game2d.scenes.UnvsBaseComponent
    {
        //[UnvsButton("Play")]
        //public void PlayInEditor()
        //{
        //    //this.PlayInEditor();
        //    this.animationController.GetComponentInParent<UnvsAnimStates>().EditotPlay(this);
        //    //new UnvsAnimStates().EditotPlay(this);
        //   // GetComponentInParent<UnvsAnimStates>().EditotPlay(this);

        //}
        [UnvsButton("Review")]
        public void PlayOutEditor()
        {
            Debug.Log($"Review {AssetDatabase.GetAssetPath(this.clip)}");
            unvs.shares.editor.UnvsEditorUtils.EditorOpenClipV2(this.animationController.gameObject, AssetDatabase.GetAssetPath(this.clip));
            EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
        }

    }
#endif
}