
using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using Unity.Cinemachine;


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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

        public Image CursorImage => cursor;

        public Canvas TopCanvas => topCanvas;

        

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
        public Texture2D cursorIcon;
        public Canvas topCanvas;
        public Image cursor;

        private bool SetupSingeScene()
        {
          
            Instance = this;

            _ = Instance.Cam;
            _ = Instance.VCam;
           
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


                GlobalApplication.DoExitGame();

            };
            pauseMenu.OnResume = () =>
            {
                pauseMenu.Hide();
            };
            pauseMenu.OnToMain = () =>
            {
                GlobalApplication.SceneLoaderManagerInstance.ClearAllAsync().ContinueWith(() =>
                {
                    PauseMenu.Hide();
                    MainMenu.Show();
                }).Forget();
            };
        }
         async UniTask StartGame()
        {
            await GlobalApplication.SceneLoaderManagerInstance.LoadNewAsync(this.StartPath, null);
            maiMenu.GetComponent<IMainMenu>().Hide();
        }
        private void LoadPrefabMainMenu()
        {
            InitDefaultCursor();
            maiMenu = Commons.LoadPrefabs(MainMenuAddressablePath);
            maiMenu.transform.SetParent(transform);
            // maiMenu.SetActive(false);
            _mainMenu = maiMenu.GetComponent<IMainMenu>();
            _mainMenu.OnStartClick = () =>
            {
                
                StartGame().Forget();
            };
            _mainMenu.OnExitClick = () =>
            {
                GlobalApplication.DoExitGame();
            };
        }
        private void Awake()
        {
            
            // Optional: Lock it to the center so it doesn't accidentally click outside the window
           
        }

        

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (cursorIcon == null)
                {
                    throw new Exception($"Pleass, setup cursorIcon for {name}");
                }
                if (GetComponent<SettingsGlobalEvents>() == null)
                {
                    throw new Exception($"Pleass, setup {typeof(SettingsGlobalEvents)}");
                }
                InitGlobalEvents();
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
            InitDefaultCursor();

        }

        private void InitGlobalEvents()
        {
            GlobalApplication.Events.OnHoverInteractObject += Events_OnHoverInteractObject;
        }
        string _unknownIconPrefabPath = "Assets/prefabs/Sprites/unknowni.png";
        string _openableIconPrefab = "Assets/prefabs/Sprites/Openable.psd";
        string _pickItemInconPrefab = "Assets/prefabs/Sprites/pick-item.psd";
        private void Events_OnHoverInteractObject(Vector2 arg1, Image cursor, GameObject target)
        {
            var interactableObject = target.GetComponent<IInteractableObject>();
            if (interactableObject != null)
            {
                var t = interactableObject.Exploring;
                switch (t)
                {
                    case ExploringType.Unknown:
                        cursor.sprite = Commons.LoadAsset<Sprite>(_unknownIconPrefabPath);
                       
                        break;
                    case ExploringType.None:
                        cursor.sprite = Commons.LoadAsset<Sprite>(_unknownIconPrefabPath);
                      
                        break;
                    case ExploringType.Openable:
                        cursor.sprite = Commons.LoadAsset<Sprite>(_openableIconPrefab);

                        break;
                    case ExploringType.Pickable:

                        cursor.sprite = Commons.LoadAsset<Sprite>(_pickItemInconPrefab);

                        break;
                }
            }
        }

        public void InitDefaultCursor()
        {
            if (!Application.isPlaying) return;
            if (topCanvas == null)
            {
                topCanvas = this.AddChildComponentIfNotExist<Canvas>(Constants.ObjectsConst.TOP_CANVAS);
                //topCanvas.AddComponentIfNotExist<GraphicRaycaster>();
                topCanvas.FullSize();
                topCanvas.SetMeOnLayer(Constants.Layers.UI);
                topCanvas.sortingOrder = 1024;

                cursor = topCanvas.transform.AddChildComponentIfNotExist<Image>(Constants.ObjectsConst.VIRTUAL_CURSOR);
                // 3. Convert Texture2D to Sprite
                // cursorIcon is your Texture2D asset
                if (cursorIcon != null)
                {
                    // Create a new Sprite from the texture
                    // Rect defines the area (full texture), Pivot (0.5, 0.5) centers it
                    cursor.sprite = Sprite.Create(
                        cursorIcon,
                        new Rect(0, 0, cursorIcon.width, cursorIcon.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    // 4. Set the UI size based on the texture dimensions
                    cursor.rectTransform.sizeDelta = new Vector2(cursorIcon.width, cursorIcon.height);
                }

                // 5. Reset position to center of screen initially
                cursor.rectTransform.anchoredPosition = Vector2.zero;
                // Hide the system cursor
                Cursor.visible = false;
            }
        }
        /// <summary>
        /// This function will update postion of cursor for input_system keyboard and mouse or even gamepad
        /// </summary>
        void Update()
        {
            UpdateCursorPosition();
        }
        private void LateUpdate()
        {
            var pos = (Vector2)_virtualMousePos;
            var interactObject = pos.GetHitCollider<Transform>(Constants.Layers.INTERACT_OBJECT);
            if (interactObject != null)
            {
                GlobalApplication.Events.RaiseOnHoverInteractObject(pos,this.cursor,interactObject.gameObject);
            } else
            {
                cursor.sprite = Sprite.Create(
                           cursorIcon,
                           new Rect(0, 0, cursorIcon.width, cursorIcon.height),
                           new Vector2(0.5f, 0.5f)
                       );
            }
        }
        [SerializeField] float gamepadSensitivity = 1000f;
        private Vector3 _virtualMousePos;
        
        void UpdateCursorPosition()
        {
            Vector2 deltaMouse = Vector2.zero;
            Vector2 stickInput = Vector2.zero;

            // 1. Check Mouse (New System)
            if (Mouse.current != null)
            {
                deltaMouse = Mouse.current.delta.ReadValue();
            }

            // 2. Check Gamepad (New System)
            if (Gamepad.current != null)
            {
                // Right Stick thường là stick phía tay phải
                stickInput = Gamepad.current.rightStick.ReadValue();
            }

            // --- Logic cập nhật vị trí ---

            // Nếu chuột di chuyển (delta khác 0)
            if (deltaMouse.sqrMagnitude > 0.01f)
            {
                // Với hệ thống mới, bạn có thể lấy vị trí chuột trực tiếp
                _virtualMousePos = Mouse.current.position.ReadValue();
            }
            else if (stickInput.sqrMagnitude > 0.1f) // Deadzone
            {
                _virtualMousePos += (Vector3)stickInput * gamepadSensitivity * Time.deltaTime;
            }

            // Clamp và Update UI như cũ
            _virtualMousePos.x = Mathf.Clamp(_virtualMousePos.x, 0, Screen.width);
            _virtualMousePos.y = Mathf.Clamp(_virtualMousePos.y, 0, Screen.height);

            cursor.rectTransform.position = _virtualMousePos;
        }

        public void CursorOff()
        {
            topCanvas.gameObject.SetActive(false);
        }

        public void CursorOn()
        {
            topCanvas.gameObject.SetActive(true);
        }
    }

}