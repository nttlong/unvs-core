using Cysharp.Threading.Tasks;
using game2d.ext;
using game2d.scenes;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using unvs.ext;
using unvs.interfaces.sys;
using unvs.shares;

namespace unvs.game2d.scenes
{
    
    public class UnvsApp : UnvsUIComponentInstance<UnvsApp>
    {
        public UnvsPlayerInput playerInput;
        public UnvsMainMenu MainMenu;
        public UnvsPauseMenu PauseMenu;
        public Transform container;
        public UnvsCinema cinema;
        public string cinemaPath;
        public string mainMenuPath;
        public string pauseMenuPath;
        public string fadeScreenPath;
        public UnvsFadeScreen fadeScreen;
        public string dialogPath;
        public UnvsDialog dialog;
        public string playerInputPath;
        public string SceneLoaderPath;
        public UnvsSceneLoader SceneLoader;
        public string startScene;

        

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
            SceneLoader = await Commons.LoadPrefabsAsync<UnvsSceneLoader>(SceneLoaderPath, container,true);
            cinema = await Commons.LoadPrefabsAsync<UnvsCinema>(cinemaPath, container, true);
            MainMenu = await Commons.LoadPrefabsAsync<UnvsMainMenu>(mainMenuPath, container, false);
            PauseMenu = await Commons.LoadPrefabsAsync<UnvsPauseMenu>(pauseMenuPath, container, false);
            dialog = await Commons.LoadPrefabsAsync<UnvsDialog>(dialogPath, container, false);
            playerInput = await Commons.LoadPrefabsAsync<UnvsPlayerInput>(playerInputPath, container, true);
            MainMenu.Show();
            //this.ApplyNavigate<Button>();
            //container.gameObject.SetActive(true);
            //// MainMenu.Show();
            //SceneLoader.enabled = true;
            //SceneLoader.gameObject.SetActive(true);
            MainMenu.btnStart.onClick.AddListener(() =>
            {
                SceneLoader.LoadNewAsync(this.startScene).ContinueWith(s =>
                {
                    MainMenu.Hide();
                }).Forget();
            });

            MainMenu.btnExit.onClick.AddListener(() =>
            {
                SceneLoader.LoadNewAsync(this.startScene).ContinueWith(s =>
                {
                    MainMenu.Hide();
                }).Forget();
            });
            container.gameObject.SetActive(true);
           

            
        }

       

        public override void InitRunTime()
        {
            InitRuntimeAsync().ContinueWith(() =>
            {
                base.InitRunTime();
            }).Forget();

        }
        public override void InitEvents()
        {
            //throw new NotImplementedException();
        }


#if UNITY_EDITOR
        [UnvsButton()]
        public void CreateCinema()
        {
            var r = this.EditorCreatePrefab<UnvsCinema>("cinema");
            this.cinema = r.value;
            this.cinemaPath = r.PrefabPath;
        }
        [UnvsButton()]
        public void GenerateMainMenu()
        {
            var r = this.EditorCreatePrefab<UnvsMainMenu>("MainMenu");
            this.MainMenu = r.value;
            this.mainMenuPath = r.PrefabPath;
        }
        [UnvsButton()]
        public void GeneratePauseMenu()
        {
            var r = this.EditorCreatePrefab<UnvsPauseMenu>("PauseMenu");
            this.PauseMenu = r.value;
            this.pauseMenuPath = r.PrefabPath;
        }
        [UnvsButton()]
        public void GenerateFadeScreen()
        {
            var r = this.EditorCreatePrefab<UnvsFadeScreen>("fadeScreen");
            this.fadeScreen = r.value;
            this.fadeScreenPath = r.PrefabPath;
        }
        [UnvsButton()]
        public void GenerateDialog()
        {
            var r = this.EditorCreatePrefab<UnvsDialog>("dialog");
            this.dialog = r.value;
            this.dialogPath = r.PrefabPath;
        }
        [UnvsButton()]
        public void GenerateSceneLoader()
        {
            var r = this.EditorCreatePrefab<UnvsSceneLoader>("SceneLoader");
            this.SceneLoader = r.value;
            this.SceneLoaderPath = r.PrefabPath;
        }
        [UnvsButton()]
        public void GeneratePlayerInput()
        {
            var r = this.EditorCreatePrefab<UnvsPlayerInput>("playerInput");
            this.playerInput = r.value;
            this.playerInputPath = r.PrefabPath;
        }

#endif

    }
}