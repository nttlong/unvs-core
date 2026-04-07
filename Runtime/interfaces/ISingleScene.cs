using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using unvs.gameword;
using unvs.interfaces.sys;
using unvs.shares;
using unvs.ui;
namespace unvs.interfaces
{
    public interface IRealTimeStats
    {

    }
    public interface ISingleScene
    {
       
        UISettingsInfo UISettings { get; }
        Image CursorImage {  get; }
        Canvas TopCanvas { get; }
       
      
        Rigidbody2D CamRigidBody { get; }
        
        //IActor CurrentActor { get; set; }
        IGlobalWorldBound GlobalWorldBound { get; }
        IScenePrefab CurrentWorld { get; set; }
        IFadeScreen FadeScreen { get; }
        IMainMenu MainMenu { get; }

       // IWorldMonitor WorldMonitor { get; }
        IPauseMenu PauseMenu { get; }
        ISceneLoader SceneLoader { get; }
        IRealTimeStats RealTimeStats { get; }
        string StartPath { get; set; }
        

        void CursorOff();
        void CursorOn();
        void InitDefaultCursor();
       
        UniTask StartGame();
    }
}