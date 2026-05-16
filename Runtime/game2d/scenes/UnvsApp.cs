using Cysharp.Threading.Tasks;
using game2d.ext;
using game2d.scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using unvs.ext;

using unvs.game2d.actors;



using unvs.shares;

using unvs.components;
using unvs.game2d.objects.types;
using unvs.controllers.inputs;

using unvs.ui;
using unvs.data;





#if UNITY_EDITOR
using UnityEditor;
using unvs.editor.utils;
using unvs.game2d.objects.editor;

#endif




namespace unvs.game2d.scenes
{
    
    public class UnvsApp : UnvsComponent
    {
        [Header("Game settings")]
        public UnvsGameSettings Settings;
        
        public UnvsUIInput<UnvsApp> uiInputs;
      
        public GameObject controllerInput;
        [Header("prefabs requirements")]
        public AssetReference refPlayerInput;
       
        public AssetReference refUiInventory;
        public AssetReference refActorDialogue;
        public AssetReference refInteractUI;
        public AssetReference refSceneLoader;
        public AssetReference refCinema;
        public AssetReference refMainMenu;
        public AssetReference refPauseMenu;
        public AssetReference refFadeScreen;
        public AssetReference refDialog;
        
        public AssetReference refCinemaSub;
        [Header("Sart Path")]
        public string startScenePath;
        public AssetReference startScene;
        [Header("Components")]
        public UnvsUIInventory UiInventory;
        public UnvsPlayerInput playerInput;
        public UnvsMainMenu MainMenu;
        public UnvsPauseMenu PauseMenu;
        public EventSystem events;
        public Transform container;
        public UnvsCinema cinema;
      
        public UnvsFadeScreen fadeScreen;
        public UnvsSceneLoader SceneLoader;
        public UnvsDialog dialog;
        public UnvsInteractUI InteractUI;
        public UnvsActorDialogue ActorDialogue;
        public UnvsCinemaSub CinemaSub;





        public UnvsActor currentActor;
        public static UnvsApp Instance { get; private set; }
        public event Action<UnvsScene> OnEnterScene;
        public event Action<UnvsScene> OnExitScene;
        
        public  void ExitGame()
        {
#if UNITY_EDITOR

            UnvsGlobalInput.EditorExitGame();
            // This will stop the Play Mode in the Unity Editor
           
#else
                // This will close the actual built application (.exe, .app, .apk)
               UnvsGlobalInput.ExitGame();
               
#endif


        }

        public virtual async UniTask InitRuntimeAsync()
        {
            
            container = transform.CreateIfNoExist<Transform>("container");
            container.gameObject.SetActive(false);
            InteractUI = await Commons.LoadPrefabsAsync<UnvsInteractUI>(refInteractUI, container, true);
            SceneLoader = await refSceneLoader.LoadPrefabsAsync<UnvsSceneLoader>(container, true); 
            cinema = await refCinema.LoadPrefabsAsync<UnvsCinema>(container, true);
            MainMenu = await refMainMenu.LoadPrefabsAsync<UnvsMainMenu>(container, true);
            PauseMenu = await Commons.LoadPrefabsAsync<UnvsPauseMenu>(refPauseMenu, container, true);
           
            dialog = await Commons.LoadPrefabsAsync<UnvsDialog>(refDialog, container, true);
            playerInput = await Commons.LoadPrefabsAsync<UnvsPlayerInput>(refPlayerInput, container, true);
            
            ActorDialogue = await Commons.LoadPrefabsAsync<UnvsActorDialogue>(refActorDialogue, container, true);
            fadeScreen = await Commons.LoadPrefabsAsync<UnvsFadeScreen>(refFadeScreen, container, true);
            UiInventory = await Commons.LoadPrefabsAsync<UnvsUIInventory>(refUiInventory, container, true);
            CinemaSub = await Commons.LoadPrefabsAsync<UnvsCinemaSub>(refCinemaSub, container, true);
          
            InitEvents();
            uiInputs.StartInputController();

        }
        async UniTask startGameAsyn()
        {
            var oldFadeTime = this.Settings.DefaultFadeTimeLoadScene;
            this.Settings.DefaultFadeTimeLoadScene = 0;
            await UnvsFadeScreen.Instance.FadeInAsync(this.Settings.FadeTimeStartGame);
            await SceneLoader.LoadNewAsync(this.startScene, "", false);
           
            UnvsCinema.Instance.OnFirstStart = () =>
            {
               
                Debug.Log("UnvsFadeScreen.Instance.FadeOutAsync(FadeTimeStartGame).Forget();");
            };
            await UnvsFadeScreen.Instance.FadeOutAsync(this.Settings.FadeTimeStartGame);
            this.Settings.DefaultFadeTimeLoadScene = oldFadeTime;
        }
        public virtual void InitEvents()
        {
            UnvsGlobalInput.OnUIInputReady += () =>
            {
                UnvsGlobalInput.NewMapUIAction(this, "Pause", action =>
                {
                    
                    action.performed += ctx =>
                    {

                        if (this.MainMenu.IsShow) return;
                        this.PauseMenu.Toggle();
                        
                    };
                });
            };
            MainMenu.btnStart.onClick.AddListener(() =>
            {
                MainMenu.Hide();
                startGameAsyn().Forget();
            });

            MainMenu.btnExit.onClick.AddListener(() =>
            {
                this.ExitGame();
            
            });
            container.gameObject.SetActive(true);
            //InteractUI.Activate();
            //var back = UnvsGlobalInput.UI["Pause"];
            //back.started += Back_started;
            MainMenu.Show();
        }
        private void OnDisable()
        {
            this.uiInputs.ControlDisable();
        }
        private void OnEnable()
        {
            this.uiInputs.ControlEnable();
        }
       

        public event Action<UnvsScene> OnScenseDestroying;
        public void RaiseEventScenseDestroying(UnvsScene unvsScene)
        {
            OnScenseDestroying?.Invoke(unvsScene);
        }

       
        
        
        public Dictionary<UnvsScene,string> Scenes { get; private set; }
      

        private UnvsScene _LastScene;
        private UnvsScene _LastExitScene;
        private CheckPintInfo restartCheckPoint;
        public UniTask InteractingTask;
        

        public void RaiseResart(CheckPintInfo value)
        {
            this.restartCheckPoint = value;
            UnvsSceneLoader.Instance.LoadNewAsync(this.restartCheckPoint.scene, restartCheckPoint.checkPointName, true).Forget();
        }
        public void CleanUp()
        {
            this.Scenes = new Dictionary<UnvsScene, string>();
        }
        public void RaiseEnterScene(UnvsScene unvsScene,bool reset=false)
        {
            if(reset)
            {
                if (this.Scenes == null)
                {
                    this.Scenes=new Dictionary<UnvsScene, string>();
                } 
            }
            if (unvsScene == null) return;

            this.Scenes ??= new Dictionary<UnvsScene, string>();
            if (this.Scenes.TryAdd(unvsScene, unvsScene.name))
            {


                void OnDestroyHandler(UnvsScene s)
                {
                    unvsScene.OnDestroying -= OnDestroyHandler;
                    this.Scenes.Remove(unvsScene);
                }
                if (this.Scenes.Count() > 1)
                {
                    var s = Scenes.Select(p => p.Key).ToList();
                    UnvsCinema.Instance.ChangeCameraState(s, reset);
                }
                unvsScene.OnDestroying += OnDestroyHandler;
                

            }
            if (_LastExitScene != unvsScene)
            {
                _LastScene = unvsScene;
                OnEnterScene?.Invoke(_LastScene);
            }
            
        }

        public void RaiseExitScene(UnvsScene unvsScene)
        {
            if (unvsScene == null) return;

            this.Scenes ??= new Dictionary<UnvsScene, string>();
            this.Scenes.Remove(unvsScene);
            if (this.Scenes.Count() <2)
            {

                UnvsApp.Instance.currentActor.SayOff();
            }
            if (_LastExitScene != unvsScene)
            {
                _LastExitScene = unvsScene;
                OnExitScene?.Invoke(_LastExitScene);
            }
            var s = Scenes.Select(p => p.Key).ToList();
            UnvsCinema.Instance.ChangeCameraState(s, false);
        }
        public static void SayText(string v)
        {
            if (Instance != null && Instance.currentActor != null)
            {
                Instance.currentActor.SayText(v);
            }
        }

        public static void SayOff()
        {
            if (Instance != null && Instance.currentActor != null)
            {
                Instance.currentActor.SayOff();
            }
        }
        public override void InitRuntime()
        {
            Instance = this;
#if UNITY_EDITOR
            if (this.Settings == null)
            {
                unvs.editor.utils.Dialogs.Show("Please,right click Create/Data/Game Settings ");
                
                return;
            }
#endif
            Application.targetFrameRate = this.Settings.fps;
            InitRuntimeAsync().ContinueWith(() =>
            {
               
            }).Forget();
            if (!Settings.UseLookNavigator)
            {
                Cursor.visible= false;
                Cursor.lockState= CursorLockMode.Locked;
            }
        }

#if UNITY_EDITOR
       
        public override void OnValidate()
        {
           
            if(startScene!=null)
            {
                startScenePath = startScene.EditorGetAddressPath();
            }
        }
       
        [UnvsButton]
        public async UniTask ValidateGameApp()
        {
            if (this.Settings == null)
            {
                unvs.editor.utils.Dialogs.Show($"Please create {this.Settings}, by using Create->Unvs->Data->Game Settings");
            }
           var ret= this.refPlayerInput.LoadAssetAsync<GameObject>();
            this.controllerInput = await ret.ToUniTask();
            var input= this.controllerInput.GetComponent< UnvsPlayerInputMap >();
            unvs.editor.utils.Dialogs.Show(input.name);
        }
        [UnvsButton]
        public void GenerateUIEvents()
        {
            this.events = this.AddChildComponentIfNotExist<EventSystem>("EventSystem");
            this.events.AddComponentIfNotExist<InputSystemUIInputModule>();
        }
        [UnvsButton()]
        public void CreateCinema()
        {
            var r = this.EditorCreatePrefab<UnvsCinema>("cinema");
            this.cinema = r.value;
            
            this.refCinema = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton()]
        public void GenerateMainMenu()
        {
            var r = this.EditorCreatePrefab<UnvsMainMenu>("MainMenu");
            this.MainMenu = r.value;
            
            this.refMainMenu = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton()]
        public void GeneratePauseMenu()
        {
            var r = this.EditorCreatePrefab<UnvsPauseMenu>("PauseMenu");
            this.PauseMenu = r.value;
           
            this.refPauseMenu = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton()]
        public void GenerateFadeScreen()
        {
            var r = this.EditorCreatePrefab<UnvsFadeScreen>("fadeScreen");
            this.fadeScreen = r.value;
            
            this.refFadeScreen = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton()]
        public void GenerateDialog()
        {
            var r = this.EditorCreatePrefab<UnvsDialog>("dialog");
            this.dialog = r.value;
          
            this.refDialog = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton()]
        public void GenerateSceneLoader()
        {
            var r = this.EditorCreatePrefab<UnvsSceneLoader>("SceneLoader");
            this.SceneLoader = r.value;
            //this.SceneLoaderPath = r.PrefabPath;
            this.refSceneLoader = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton()]
        public void GeneratePlayerInput()
        {
            var r = this.EditorCreatePrefab<UnvsPlayerInput>("playerInput");
            this.playerInput = r.value;
           
           
            this.refPlayerInput = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton]
        public void GenerateInteracUI()
        {
            var r = this.EditorCreatePrefab<UnvsInteractUI>("InteractUI");
            this.InteractUI = r.value;
           
            this.refInteractUI = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);

        }
        [UnvsButton]
        public void GenerateActorDialogue()
        {
            var r = this.EditorCreatePrefab<UnvsActorDialogue>("ActorDialogue");
            this.ActorDialogue = r.value;
           
            this.refActorDialogue = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }
        [UnvsButton("Generate Inventory")]
        public void GenerateInventoryUI()
        {
            var r = this.EditorCreatePrefab<UnvsUIInventory>("UnvsUiInventory");
            this.UiInventory = r.value;

            this.refUiInventory = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }


        [UnvsButton("Generate Cinema sub")]
        public void GenerateCinemaSub()
        {
            var r = this.EditorCreatePrefab<UnvsCinemaSub>("UnvsCinemaSub");
            this.CinemaSub = r.value;

            this.refCinemaSub = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }





#endif

    }
}