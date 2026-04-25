using Cysharp.Threading.Tasks;
using game2d.ext;
using game2d.scenes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.actors;


using unvs.shares;

using unvs.game2d.objects.components;
using unvs.game2d.objects.types;
using unvs.controllers_input;
using UnityEngine.UIElements;



#if UNITY_EDITOR
using UnityEditor;
using unvs.editor.utils;
using unvs.game2d.objects.editor;

#endif




namespace unvs.game2d.scenes
{
    
    public class UnvsApp : UnvsComponent
    {
        public float DefaultFadeTimeLoadScene=0;
        public GameObject controllerInput;
        [Header("prefabs requirements")]
        public AssetReference refPlayerInput;
        public AssetReference refActorDialogue;
        public AssetReference refInteractUI;
        public AssetReference refSceneLoader;
        public AssetReference refCinema;
        public AssetReference refMainMenu;
        public AssetReference refPauseMenu;
        public AssetReference refFadeScreen;
        public AssetReference refDialog;
        public int ChunLenght = 3;
        public int fps = 120;
        [Header("Sart Path")]
        public string startScenePath;
        public AssetReference startScene;
        [Header("Components")]
        
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
        public UnvsActirDialogue ActorDialogue;


        
       
       
       
        public UnvsActor currentActor;
        public static UnvsApp Instance { get; private set; }
        public event Action<UnvsScene> OnEnterScene;
        public event Action<UnvsScene> OnExitScene;
        public  void ExitGame()
        {
           
            #if UNITY_EDITOR

                        // This will stop the Play Mode in the Unity Editor
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                // This will close the actual built application (.exe, .app, .apk)
                Application.Quit();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            #endif
        }
       
        public virtual async UniTask InitRuntimeAsync()
        {

            container = transform.CreateIfNoExist<Transform>("container");
            container.gameObject.SetActive(false);
            SceneLoader = await refSceneLoader.LoadPrefabsAsync<UnvsSceneLoader>(container, true); 
            cinema = await refCinema.LoadPrefabsAsync<UnvsCinema>(container, true);
            MainMenu = await refMainMenu.LoadPrefabsAsync<UnvsMainMenu>(container, true);
            PauseMenu = await Commons.LoadPrefabsAsync<UnvsPauseMenu>(refPauseMenu, container, true);
            dialog = await Commons.LoadPrefabsAsync<UnvsDialog>(refDialog, container, true);
            playerInput = await Commons.LoadPrefabsAsync<UnvsPlayerInput>(refPlayerInput, container, true);
            InteractUI = await Commons.LoadPrefabsAsync<UnvsInteractUI>(refInteractUI, container, true);
            ActorDialogue = await Commons.LoadPrefabsAsync<UnvsActirDialogue>(refActorDialogue, container, true);
            fadeScreen = await Commons.LoadPrefabsAsync<UnvsFadeScreen>(refFadeScreen, container, true);
            MainMenu.Show();
            ActorDialogue.Hide();
            dialog.Hide();
            PauseMenu.Hide();

            InitEvents();

        }

        public virtual void InitEvents()
        {
            MainMenu.btnStart.onClick.AddListener(() =>
            {
                SceneLoader.LoadNewAsync(this.startScenePath, "",false).ContinueWith(s =>
                {
                    MainMenu.Hide();
                }).Forget();
            });

            MainMenu.btnExit.onClick.AddListener(() =>
            {
                SceneLoader.LoadNewAsync(this.startScenePath, "", false).ContinueWith(s =>
                {
                    MainMenu.Hide();
                }).Forget();
            });
            container.gameObject.SetActive(true);
            InteractUI.Activate();
            var back = UnvsGlobalInput.UI["Pause"];
            back.started += Back_started;
        }

        private void Back_started(InputAction.CallbackContext obj)
        {
            if (MainMenu.IsShow) return;
            UnvsPauseMenu.Instance.Toggle();
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
        

        public void RaiseResart(CheckPintInfo value)
        {
            this.restartCheckPoint = value;
            UnvsSceneLoader.Instance.LoadNewAsync(this.restartCheckPoint.scenePath, restartCheckPoint.checkPointName, true).Forget();
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

                unvsScene.OnDestroying += OnDestroyHandler;
                if (this.Scenes.Count() > 1)
                {
                    var s = Scenes.Select(p => p.Key).ToList();
                    UnvsCinema.Instance.ChangeCameraState(s, reset);
                }

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

        }
        public override void InitRuntime()
        {
            Instance = this;
            Application.targetFrameRate = this.fps;
            InitRuntimeAsync().ContinueWith(() =>
            {
               
            }).Forget();
        }

#if UNITY_EDITOR
       
        private void OnValidate()
        {
            if(startScene!=null)
            {
                startScenePath = startScene.EditorGetAddressPath();
            }
        }
       
        [UnvsButton]
        public async UniTask ValidateGameApp()
        {
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
            var r = this.EditorCreatePrefab<UnvsActirDialogue>("ActorDialogue");
            this.ActorDialogue = r.value;
           
            this.refActorDialogue = unvs.editor.utils.UnvsEditorUtils.CreateAssetReference(r.PrefabPath);
        }



        



#endif

    }
}