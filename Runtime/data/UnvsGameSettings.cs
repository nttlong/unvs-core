namespace unvs.data
{
 
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.U2D;

    

    [CreateAssetMenu(fileName = "GameSettings", menuName = "Unvs/Data/GameSettings")]
    public partial class UnvsGameSettings : UnvsScriptObject
    {
        [Header("Visual loading")]
        public float FadeTimeStartGame = 0.5f;
        public float DefaultFadeTimeLoadScene = 0;
        public int DelayFrameBeforeInteriorSceneShow = 15;
        //public bool DisplayCursor = true;
        [SerializeField]
        public Vector2 PCUIScreenSize = new Vector2(1920f, 1080f);
        [Header("Performance")]
        public int ChunLenght = 3;
        public int fps = 120;
        public int FrequencyOfWorldBoundUpdating = 120;
        public bool useAssetReferenceAssetGUIDForName = true;
        [Header("Camera")]
        [SerializeField]
        public Vector3 DefaultTargetOffset = new Vector3(0, 0, -35);
        public float CamWacherDistance = -30;
        [Header("Cinema light")]
        public float DurationTimeSmoothChangeSate = 1.5f;
        public int MaintainGlobalLightNumber = 5;
        [Header("Game play")]
        public bool UseLookNavigator = false;
        public float GamepadLookCursorSpeed = 10f;
    }
}