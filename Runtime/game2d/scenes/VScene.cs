/*
    this file define Virtuale scene, acctually, that is prefab of scene
    it contain Main cam
   
   
*/
using game2d.ext;
using game2d.scenes;
using UnityEngine;
using unvs.ext;

namespace unvs.game2d.scenes
{
    [EditButtons("Generate")]
    public class VScene : UnvsComponent
    {
        public Camera cam;

        public override void InitRuntime()
        {
            throw new System.NotImplementedException();
        }


#if UNITY_EDITOR
        [InspectorButton("Generate elements")]
        public void Generate()
        {

        }
#endif
    }
}