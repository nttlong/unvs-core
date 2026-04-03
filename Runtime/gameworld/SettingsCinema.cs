using interfaces.cinema;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;

using UnityEngine;
using unvs.ext;
using unvs.gameword;
using unvs.interfaces;
using unvs.ui;
using static Unity.Cinemachine.CinemachineBrain;

namespace unvs.gameworld
{
    [ExecuteInEditMode]
    public class SettingsCinema : MonoBehaviour, ICinemaScene
    {
        public Camera main;
        public CinemachineBrain brain;
        public CinemachineCamera vCam;
        public GlobalWorldBound worldBound;
        public CinemachineFollow cinemaFollow;
        public SettingsCommonAudioSource commonAudioSource;

        public Camera Main => main;

        public CinemachineBrain Brain=> brain;

        public CinemachineCamera VCam => vCam;

        public GlobalWorldBound WorldBound => worldBound;

        public CinemachineFollow CinemaFollow => cinemaFollow;

        public SettingsCommonAudioSource AudioSource => commonAudioSource;

        private void Awake()
        {
            worldBound = this.AddChildComponentIfNotExist<GlobalWorldBound>(unvs.shares.Constants.ObjectsConst.GLOBAL_WORLD_BOUND);
            main = this.AddChildComponentIfNotExist<Camera>("Main Camera");
            main.tag = "MainCamera";
            main.AddComponentIfNotExist<AudioListener>();
            commonAudioSource = this.AddChildComponentIfNotExist<SettingsCommonAudioSource>(unvs.shares.Constants.ObjectsConst.COMMON_AUDIO_SOURCE);
            vCam = this.AddChildComponentIfNotExist<CinemachineCamera>("VCam");
            brain= main.AddComponentIfNotExist<CinemachineBrain>();
            cinemaFollow=vCam.AddComponentIfNotExist<CinemachineFollow>();
            
            var confiner = vCam.AddComponentIfNotExist<CinemachineConfiner2D>();
            confiner.BoundingShape2D = worldBound.GetComponent<CompositeCollider2D>();
            main.AddComponentIfNotExist<CamObject>();
            main.orthographic = true;
          
            main.orthographicSize = 20;
            main.nearClipPlane = 0.5f;
            main.farClipPlane = 1000f;
            vCam.Lens.OrthographicSize = 20f;
            vCam.Lens.NearClipPlane = 0.5f;
            vCam.Lens.FarClipPlane = 1000f;
            vCam.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            brain.LensModeOverride = new LensModeOverrideSettings
            {
                
                Enabled = true,
            };
            vCam.transform.position = new Vector3(0, 0, -10);
            vCam.Priority = new PrioritySettings {
                Enabled = true,
            };
            
        }

       

        private void Start()
        {
            var confiner = vCam.AddComponentIfNotExist<CinemachineConfiner2D>();
            confiner.BoundingShape2D = worldBound.GetComponent<CompositeCollider2D>();
            vCam.transform.position = new Vector3(0, 0, -10);
            
        }
    }
}