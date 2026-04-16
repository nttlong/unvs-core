using UnityEngine;
using UnityEngine.U2D.IK;
using unvs.ext;
using unvs.game2d.scenes;

namespace Unvs.Core.Game2D.Scenes.Actors
{
    public partial class UnsvPlayerDoTweenAnim:UnvsBaseComponent
    {
        public Animator animController;
        public Transform doTweenAnimPaths;
        [SerializeField]
        public Solver2D[] Solvers;
    }
#if UNITY_EDITOR
    public partial class UnsvPlayerDoTweenAnim:UnvsBaseComponent
    {
        public bool editorTestPlay;

        private void OnValidate()
        {
            animController = GetComponentInChildren<Animator>();
            doTweenAnimPaths = this.AddChildComponentIfNotExist<Transform>("do-tween-anim-paths");
        }
        [UnvsButton("Create anim path")] 
        public void EdiorCreateAnimPath()
        {
            foreach (var anim in Solvers)
            {
                if (doTweenAnimPaths.GetComponentInChildrenByName<EdgeCollider2D>(anim.GetChain(0).target.name) != null) continue;
                var pos = anim.GetChain(0).target.GetSegment().Center();
                var path = doTweenAnimPaths.AddChildComponentIfNotExist<EdgeCollider2D>(anim.GetChain(0).target.name);
                path.points= new Vector2[]{pos-new Vector2(10,0),pos+new Vector2(10,0)};
                path.isTrigger = true;
            }
        }
        [UnvsButton("Play tween")]
        public void EdiorPlayTween()
        {
            this.editorTestPlay = !this.editorTestPlay;

        }
    }
#endif
}