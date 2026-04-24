using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using unvs.editor.components;
using unvs.ext;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using unvs.shares;
#if UNITY_EDITOR

#endif
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
            scene.groundThickness.AddComponentIfNotExist<UnvsGeometryChunks>();
        }
        [UnvsButton("Geometry outline")]
        public async UniTask EditorGeometryOutline()
        {
            EditorCreateThicknessGround();
            var scene = this.GetComponentInParent<UnvsScene>();
            var poly= scene.groundThickness.GetComponent<PolygonCollider2D>();
            var info= unvs.editor.utils.EditorTools.GetFolderOfGameObjectByScene(poly.gameObject);
            //if (info == null)
            //{
            //    return;
            //}
           

            
            var subFolder = System.IO.Path.Combine(info.FolderPath, $"{info.Name}-sprites");
            var fullPath = System.IO.Path.Combine(subFolder, $"{gameObject.name}.png");
            var fullPathPsd = System.IO.Path.Combine(subFolder, $"{gameObject.name}.psd");
            await unvs.editor.utils.UnvsPythonCall.Call("UnvsGeometry", "CreateGuidelinePng", new
            {
                width= poly.bounds.size.x,
                height= poly.bounds.size.y,
                points= poly.points.Select(p=>new
                {
                    x= p.x,y= p.y,
                }).ToArray(),
                output_path= fullPath,
                output_path_psd= fullPathPsd,
            });
        }

#endif
        #endregion
    }

}