using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using unvs.shares;

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
        bool IsShowing { get; }

        void DoStartGame();
        void DoExitGame();
    }
    public interface IUIHub
    {
        DockDirection Dock { get; }

        float Size { get; }
        Canvas HubCanvas { get; }
        Image HubPanel { get; }
        void Hide();
        void Show();

    }
}