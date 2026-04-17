using System;
using UnityEngine;
using unvs.ext;
using unvs.shares;

namespace unvs.game2d.scenes
{
    public class UnvsAudible : UnvsBaseComponent
    {
        public AudioInfo audioInfo;
        #region MyRegion
#if UNITY_EDITOR
        public enum PotyType
        {
            Parallel,
            FlatBottom,
            FlatBottomOneLine
        }
        [Range(1f,100)]
        public float ThiknessDown=10;
        public PotyType polyType;
        [UnvsButton("Create thickness ground")]
        public void EditorCreateThicknessGround()
        {
            var polygonName = $"{name}-thickness";
            var scene=this.GetComponentInParent<UnvsScene>();
            scene.groundThickness = this.transform.parent.AddChildComponentIfNotExist<PolygonCollider2D>(polygonName);
            scene.groundThickness.SetMeOnLayer(Constants.Layers.WORLD_GROUND);
            scene.groundThickness.transform.localPosition = transform.localPosition;
            scene.groundThickness.transform.localScale= transform.localScale;
            scene.groundThickness.transform.position = transform.position;
            if (polyType==PotyType.Parallel)
            scene.groundThickness.points = EdgeCollider2DExtension.MakeThicnessPoly(
                this.GetComponent<EdgeCollider2D>(), 
                ThiknessDown,scene.wallLeft.bounds.max.x, 
                scene.wallRight.bounds.min.x);
            if (polyType == PotyType.FlatBottom)
            {
                // scene.groundThickness.points = EdgeCollider2DExtension.MakeThicnessPolyWithFlatBottom(
                //this.GetComponent<EdgeCollider2D>(),
                //ThiknessDown, scene.wallLeft.bounds.max.x,
                //scene.wallRight.bounds.min.x);
                scene.groundThickness.points = EdgeCollider2DExtension.MakeThicnessPolyWithFlatBottom(
              this.GetComponent<EdgeCollider2D>(),
              ThiknessDown, scene.wallLeft.bounds.max.x,
              scene.wallRight.bounds.min.x);
            }
            if (polyType == PotyType.Parallel)
            {
                    scene.groundThickness.points = EdgeCollider2DExtension.MakeThicnessPoly(
                        this.GetComponent<EdgeCollider2D>(),
                        ThiknessDown, scene.wallLeft.bounds.max.x,
                        scene.wallRight.bounds.min.x);
            }
            if (polyType == PotyType.FlatBottomOneLine)
            {
                scene.groundThickness.points = EdgeCollider2DExtension.MakeThicnessPolyWithFlatBottomOneLine(
                this.GetComponent<EdgeCollider2D>(),
                ThiknessDown, scene.wallLeft.bounds.max.x,
                scene.wallRight.bounds.min.x);
            }
        }

        
#endif
        #endregion
    }

}