using Cysharp.Threading.Tasks;
using game2d.scenes;
using System;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.shares;

namespace unvs.game2d.scenes
{
    public class AppScene : AppUIElemen<AppScene>
    {
        public AppMainMenu MainMenu;
        public AppPauseMenu PauseMenu;
        public Transform container;
        public AppCinema cinema;
        public string cinemaPath;
        public string mainMenuPath;
        public string pauseMenuPath;
        public string fadeScreenPath;
        public AppFadeScreen fadeScreen;
        public string dialogPath;
        public AppDialog dialog;

        public static AppScene Instance { get; private set; }
        

        public override void InitRunTime()
        {
            InitRuntimeAsync().Forget();
        }

        public virtual async UniTask InitRuntimeAsync()
        {
            Instance = this;
            container = transform.CreateIfNoExist<Transform>("container");
            cinema= await Commons.LoadPrefabsAsync<AppCinema>(cinemaPath, container, true);
            MainMenu = await Commons.LoadPrefabsAsync<AppMainMenu>(mainMenuPath, container, true);
            PauseMenu = await Commons.LoadPrefabsAsync<AppPauseMenu>(pauseMenuPath, container, true);
            dialog= await Commons.LoadPrefabsAsync<AppDialog>(dialogPath, container, true);
            this.ApplyNavigate<Button>();
        }
    }
}