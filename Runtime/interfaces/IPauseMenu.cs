using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace unvs.interfaces
{
    public interface IPauseMenu
    {
       
        Canvas PauseMenuCanvas { get; }
        Image PauseMenuPanel { get; }
        Action OnResume { get; set; }
        Action OnExit { get; set; }
        Action OnToMain { get; set; }
        void Show();
        void Hide();
        void Toggle();
        
    }
}
