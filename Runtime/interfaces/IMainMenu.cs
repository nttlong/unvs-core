using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace unvs.interfaces
{
    public interface IMainMenu
    {

        void Show();
        void Hide();
        Action OnStartClick { get; set; }
        Action OnExitClick { get; set; }
        Canvas MenuCanvas { get; }
        Image Panel { get; }
        void DoStartGame();
        void DoExitGame();
    }
    public interface IUIHub
    {
        float Height { get; }
        Canvas HubCanvas { get; }
        Image HubPanel { get; }
        void Hide();
        void Show();

    }
}