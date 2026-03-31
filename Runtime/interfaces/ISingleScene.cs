using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using unvs.interfaces.sys;
using unvs.ui;
namespace unvs.interfaces
{
    public interface ISingleScene
    {
        Camera Cam { get; }
        CinemachineBrain Brain { get; }
        CinemachineCamera VCam { get; }
        Rigidbody2D CamRigidBody { get; }
        CinemachineConfiner2D Confiner { get; }
        //IActor CurrentActor { get; set; }
        IGlobalWorldBound GlobalWorldBound { get; }
        IScenePrefab CurrentWorld { get; set; }
        IFadeScreen FadeScreen { get; }
        IMainMenu MainMenu { get; }

       // IWorldMonitor WorldMonitor { get; }
        IPauseMenu PauseMenu { get; }
        ISceneLoader SceneLoader { get; }
      
      
        
    }
}