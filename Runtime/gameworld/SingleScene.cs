
using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using Unity.Cinemachine;


using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
//using UnityEngine.UIElements.InputSystem;
using unvs.ext;
//using unvs.gameword.manager;
using unvs.interfaces;
using unvs.interfaces.sys;
using unvs.manager;
using unvs.shares;
using unvs.ui;
namespace unvs.gameword
{
    [RequireComponent(typeof(LightManagerObject))]
    public class SingleScene : MonoBehaviour, ISingleScene
    {
        
        public static ISingleScene Instance;
        public Camera cam;
        public CinemachineCamera vcam;
        public CinemachineConfiner2D confiner;
        
        private GameObject currentActorTr;
        private IGlobalWorldBound _globalWorldBound;
        private GameObject globalWorldBound;
        private IScenePrefab _currentWorld;
        public GameObject currentWorld;
        public Image fadePanel;

        public CinemachineBrain brain;
        private GameObject maiMenu;
        private IFadeScreen fadeScreen;
        private IMainMenu _mainMenu;
        private EventSystem eventsSys;
       
        private IPauseMenu pauseMenu;

        public Camera Cam
        {
            get
            {
                if (cam == null)
                {
                    cam = Camera.main;
                    
                }
                return cam;
                
            }
        }



        public CinemachineCamera VCam
        {
            get
            {

                if (vcam != null) return vcam;

                return vcam;
            }
        }

        public Rigidbody2D CamRigidBody => cam?.GetComponent<Rigidbody2D>();

        public CinemachineConfiner2D Confiner => confiner;

        
        public IGlobalWorldBound GlobalWorldBound
        {
            get
            {
                if (!Application.isPlaying) return null;
                if (_globalWorldBound != null) return _globalWorldBound;
                _globalWorldBound =this.GetComponentInChildrenByName<GlobalWorldBound>(Constants.ObjectsConst.GLOBAL_WORLD_BOUND);
                if (_globalWorldBound != null) return _globalWorldBound;
                globalWorldBound = new GameObject(Constants.ObjectsConst.GLOBAL_WORLD_BOUND);
                _globalWorldBound = globalWorldBound.AddComponent<GlobalWorldBound>();
                globalWorldBound.transform.SetParent(this.transform);
                return _globalWorldBound;
            }
        }

        public IScenePrefab CurrentWorld
        {
            get => _currentWorld;
            set
            {
                _currentWorld = value;
                if (_currentWorld != null)
                {
                    currentWorld = (value as MonoBehaviour)?.gameObject;
                }
            }
        }




        public CinemachineBrain Brain => brain;

        public GameObject Speaker { get; private set; }

        public IFadeScreen FadeScreen => fadeScreen;

        public IMainMenu MainMenu => _mainMenu;


        

        public IPauseMenu PauseMenu => pauseMenu;

        public ISceneLoader SceneLoader => sceneLoader;

        

        public string StartPath;
        private ISceneLoader sceneLoader;
        public GameObject goSceneLoader;
        private bool isStarting;
        [Header("Prefab settings")]
        [Description("Example: Assets/Prefabs/Cinema/Cinema.prefab")]
        public string cinemaSettingPrefabPath;
        /// <summary>
        /// Exmaple "Assets/Prefabs/UI/Hub/Hub.prefab"
        /// </summary>
        public string HubPrefabAddressablePath;
        /// <summary>
        /// example : "Assets/Prefabs/UI/DiscoveryDiialog/DiscoveryDialog.prefab"
        /// </summary>
        public string DiscoveryDialogAddressablePath;
        /// <summary>
        /// Exmaple "Assets/Prefabs/UI/Speaker/Speaker.prefab"
        /// </summary>
        public string SpeakerDialogAddressablePath;
        /// <summary>
        /// Exmaple "Assets/Prefabs/UI/FadeScreenPanel/FadeScreen.prefab"
        /// </summary>
        public string FadeSceenAddressablePath;
        /// <summary>
        /// "Assets/Prefabs/UI/MainMenu/MainMenu.prefab"
        /// </summary>
        public string MainMenuAddressablePath;
        /// <summary>
        /// "Assets/Prefabs/UI/PauseMenu/PauseMenu.prefab"
        /// </summary>
        public string PauseMenuAddressalbePath;
      

        private bool SetupSingeScene()
        {
            //if (Instance != null)
            //{
            //    Destroy(gameObject);
            //    return false;
            //}
            //DontDestroyOnLoad(gameObject);
            Instance = this;

            _ = Instance.Cam;
            _ = Instance.VCam;
            //if (cam == null)
            //{
            //    cam = Camera.main;
            //    if (cam != null)
            //        cam.AddComponentIfNotExist<CamObject>();
            //}
            unvs.shares.GlobalApplication.SingleScene = this;
            if (goSceneLoader == null)
            {
                goSceneLoader = new GameObject(Constants.ObjectsConst.SCENE_LOADER);
                sceneLoader = goSceneLoader.AddComponent<SceneLoaderManager>();
            }

            return true;
        }

        private void LoadAllUI()
        {
            LoadPrefabMainMenu();
            var hub = Commons.LoadPrefabs(HubPrefabAddressablePath);

            var discoveryDialog = Commons.LoadPrefabs(DiscoveryDialogAddressablePath);
            discoveryDialog.transform.SetParent(transform);


            Speaker = Commons.LoadPrefabs(SpeakerDialogAddressablePath);

            Speaker.transform.SetParent(transform);
            Speaker.GetComponent<IUISpeakerController>().Hide();


            var fadeScreenGo = Commons.LoadPrefabs(FadeSceenAddressablePath);
            fadeScreen = fadeScreenGo.GetComponent<IFadeScreen>();

            LoadPrefabPauseMenu();

        }
        private async UniTask RunExitAsync()
        {
           
            
            //await GlobalApplication.FadeScreenController.FadeInAsync();
            
            await GlobalApplication.SceneLoaderManagerInstance.ClearAllAsync();
           
            pauseMenu.Hide();
           
            _mainMenu.Show();
           
        }
        private void LoadPrefabPauseMenu()
        {
            var pauseMenuGo = Commons.LoadPrefabs(PauseMenuAddressalbePath);
            pauseMenuGo.transform.SetParent(transform);
            pauseMenu = pauseMenuGo.GetComponent<IPauseMenu>();
            pauseMenu.OnExit = () =>
            {
                

                RunExitAsync().Forget();

            };
            pauseMenu.OnResume = () =>
            {
                pauseMenu.Hide();
            };
        }

        private void LoadPrefabMainMenu()
        {
            maiMenu = Commons.LoadPrefabs(MainMenuAddressablePath);
            maiMenu.transform.SetParent(transform);
            // maiMenu.SetActive(false);
            _mainMenu = maiMenu.GetComponent<IMainMenu>();
            _mainMenu.OnStartClick = () =>
            {
                if (isStarting) return;
                isStarting=true;
                //var runner = ChunkSceneLoaderUtils.LoadNewAsync(this.StartPath, null);
                var runner = GlobalApplication.SceneLoaderManagerInstance.LoadNewAsync(this.StartPath,null);
                runner.ContinueWith((p) =>
                {
                    _mainMenu.Hide();
                    isStarting = false;
                }).Forget();
            };
        }
        private void Awake()
        {
            
        }

        private void UiEvents_PauseStarted()
        {
            pauseMenu.Toggle();
           
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (GetComponent<SettingsGlobalEvents>() == null)
                {
                    throw new Exception($"Pleass, setup {typeof(SettingsGlobalEvents)}");
                }
            }
            if (Application.isPlaying && !string.IsNullOrEmpty(cinemaSettingPrefabPath))
            {
                var cmp = SetupSceneWorld.CreateComponents(
                    transform, cinemaSettingPrefabPath);
                cam = cmp.Main;
                vcam = cmp.VCam;
                brain = cmp.Brain;
                confiner = cmp.Confiner;
                //SetupComponents();
            }
            LoadAllUI();
            SetupSingeScene();
            //var isEditPlay = ChunkSceneLoaderUtilsOld.PlayActiveSceneFromEditor();
            var isEditPlay = GlobalApplication.SceneLoaderManagerInstance.PlayActiveSceneFromEditor();
            if (!isEditPlay)
            {
                this.MainMenu.Show();
            }
            else
            {
                this.MainMenu.Hide();
            }
            cam = Camera.main;
           
            }



        //public void SetupComponentsOld()
        //{

        //    if (this.Cam == null || this.VCam == null) return;
        //    this.Cam.AddComponentIfNotExist<CamObject>();
        //    // 1. Setup vật lý cho Main Camera (Dựa theo ảnh 2)
        //    this.Cam.orthographic = true;
        //    this.Cam.orthographicSize = 20;
        //    this.Cam.nearClipPlane = 0.5f;
        //    this.Cam.farClipPlane = 1000f;

        //    // 2. Setup Cinemachine Camera (Để vcam điều khiển Main Cam đúng thông số)
        //    // Trong Unity 6, LensSettings nằm trực tiếp trong vcam.Lens
        //    //VCam.Lens.OrthographicSize = 20;
        //    //VCam.Lens.NearClipPlane = 0.5f;
        //    //VCam.Lens.FarClipPlane = 1000f;

        //    // Đảm bảo vcam có Priority cao để Brain ưu tiên (Ảnh 1 cho thấy nó đang Live)
        //    //VCam.Priority = 100;

        //    // 3. Setup Confiner (Giới hạn vùng nhìn)
        //    if (this.Confiner != null && this.GlobalWorldBound.Coll != null)
        //    {
        //        Confiner.BoundingShape2D = this.GlobalWorldBound.Coll;
        //        // Xóa cache cũ để Confiner tính toán lại theo Collider mới của Scene
        //        Confiner.InvalidateBoundingShapeCache();
        //    }

        //    // 4. FIX MÀN HÌNH XANH: Ép Camera về vị trí của mục tiêu ngay lập tức
        //    // Nếu vcam.Follow đang null, nó sẽ đứng ở (0,0,0) và dễ gây màn hình xanh
        //    //if (VCam.Follow != null)
        //    //{
        //    //    vcam.ForceCameraPosition(vcam.Follow.position, Quaternion.identity);
        //    //}
        //    // Brain.ManualUpdate();
        //}

        
    }

}