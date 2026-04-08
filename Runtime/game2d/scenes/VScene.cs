/*
    this file define Virtuale scene, acctually, that is prefab of scene
    it contain Main cam
   
   
*/
using UnityEngine;

namespace unvs.game2d.scenes
{
    public class VScene : MonoBehaviour
    {
        /// <summary>
        /// Main Camera, it will be hide when scene is loaded
        /// The main cam just run in editor mode
        /// At production mode we use AppScene for full gameplay
        /// </summary>
        public Camera MainCam;
        
        
    }
}