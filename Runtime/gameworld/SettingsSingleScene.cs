
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
    public class SettingsSingleScene : MonoBehaviour, ISingleScene
    {
        
        public static ISingleScene Instance;
        [Header("Cinema settings")]
        public Camera cam;
       
        
        
        private GameObject currentActorTr;
        private IGlobalWorldBound _globalWorldBound;
        private GameObject globalWorldBound;
        private IScenePrefab _currentWorld;
         GameObject currentWorld;
       // public Image fadePanel;

      
        private GameObject maiMenu;
        private IFadeScreen fadeScreen;
        private IMainMenu _mainMenu;
        IPauseMenu _pauseMenu;
        //private EventSystem eventsSys;
       
        

       



        

        public Rigidbody2D CamRigidBody => cam?.GetComponent<Rigidbody2D>();

        

        
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




       

        public GameObject Speaker { get; private set; }

        public IFadeScreen FadeScreen => fadeScreen;

        public IMainMenu MainMenu => _mainMenu;


        

        public IPauseMenu PauseMenu => _pauseMenu;
        ISceneLoader _sceneLoader;
        public ISceneLoader SceneLoader => _sceneLoader;

        public Image CursorImage => cursor;

        public Canvas TopCanvas => topCanvas;

        public UISettingsInfo UISettings => uiSettings;

        public IRealTimeStats RealTimeStats => realTimeStats;

        public string StartPath;
        
        GameObject _goSceneLoader;
     
        [Header("UI")]
        #region UI settings
        public Texture2D defaultCursorIcon;
        [Description("Example: Assets/Prefabs/Cinema/Cinema.prefab")]
        [SerializeField]
        private UISettingsInfo uiSettings;
        
        #endregion

        public Canvas topCanvas;
        public Image cursor;

        private bool SetupSingeScene()
        {
          
            Instance = this;

           
           
            unvs.shares.GlobalApplication.SingleScene = this;
            if (_goSceneLoader == null)
            {
                _goSceneLoader = new GameObject(Constants.ObjectsConst.SCENE_LOADER);
                _sceneLoader = _goSceneLoader.AddComponent<SceneLoaderManager>();
            }

            return true;
        }

        private void LoadAllUI()
        {
            LoadPrefabMainMenu();
            var hub = this.UISettings.Hub;

            var discoveryDialog = this.UISettings.UIDiscoveryDialog;
           // discoveryDialog.transform.SetParent(transform);


            Speaker = (this.UISettings.SettingsUISpeaker as MonoBehaviour).gameObject;

           // Speaker.transform.SetParent(transform);
            Speaker.GetComponent<IUISpeakerController>().Hide();


            var fadeScreenGo = this.UISettings.UIFadeScreen; 
            fadeScreen = fadeScreenGo.GetComponent<IFadeScreen>();
            realTimeStats =this.uiSettings.RealTimeStats;
            LoadPrefabPauseMenu();

        }
        
        private void LoadPrefabPauseMenu()
        {
            var pauseMenuGo = this.UISettings.SettingsUIPauseMenu;
         //   pauseMenuGo.transform.SetParent(transform);
            _pauseMenu = pauseMenuGo.GetComponent<IPauseMenu>();
            _pauseMenu.OnExit = () =>
            {


                GlobalApplication.DoExitGame();

            };
            _pauseMenu.OnResume = () =>
            {
                _pauseMenu.Hide();
            };
            _pauseMenu.OnToMain = () =>
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
            maiMenu = (this.UISettings.SettingsUIMainMenu as MonoBehaviour).gameObject;
            //maiMenu.transform.SetParent(transform);
            //maiMenu.SetActive(false);
            _mainMenu = this.UISettings.SettingsUIMainMenu;
            _mainMenu.OnStartClick = () =>
            {
                
                StartGame().Forget();
            };
            _mainMenu.OnExitClick = () =>
            {
                GlobalApplication.DoExitGame();
            };
        }
        private  void Awake()
        {

           

        }

        private async void Start()
        {
            if (Application.isPlaying)
            {
                this.uiSettings.ValidateOnRequires(this);
                var obj = this.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.SCENE_SETTINGS);
                obj.gameObject.SetActive(false);
                await this.uiSettings.InitGameSettingsAsync(obj);
                obj.gameObject.SetActive(true);
            }
            if (Application.isPlaying)
            {
               
                if(string.IsNullOrEmpty(this.StartPath))
                {
                    Debug.LogError($"Please, setup StartPath for {name}");
                }
                if (defaultCursorIcon == null)
                {
                    Debug.LogError($"Please, setup defaultCursorIcon for {name}");
                    
                }
                if (GetComponent<SettingsGlobalEvents>() == null)
                {
                    Debug.LogError($"Please, setup {typeof(SettingsGlobalEvents)}, ref={name}");
                   
                }
                InitGlobalEvents();
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
                if (defaultCursorIcon != null)
                {
                    // Create a new Sprite from the texture
                    // Rect defines the area (full texture), Pivot (0.5, 0.5) centers it
                    cursor.sprite = Sprite.Create(
                        defaultCursorIcon,
                        new Rect(0, 0, defaultCursorIcon.width, defaultCursorIcon.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    // 4. Set the UI size based on the texture dimensions
                    cursor.rectTransform.sizeDelta = new Vector2(defaultCursorIcon.width, defaultCursorIcon.height);
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
            if(cursor==null) return;
            var pos = (Vector2)_virtualMousePos;
            var interactObject = pos.GetHitCollider<Transform>(Constants.Layers.INTERACT_OBJECT);
            if (interactObject != null)
            {
                GlobalApplication.Events.RaiseOnHoverInteractObject(pos,this.cursor,interactObject.gameObject);
            } else
            {
                cursor.sprite = Sprite.Create(
                           defaultCursorIcon,
                           new Rect(0, 0, defaultCursorIcon.width, defaultCursorIcon.height),
                           new Vector2(0.5f, 0.5f)
                       );
            }
        }
        [SerializeField] float gamepadSensitivity = 1000f;
        private Vector3 _virtualMousePos;
        private IRealTimeStats realTimeStats;

        void UpdateCursorPosition()
        {
            if(cursor==null) return;
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