using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace unvs.interfaces
{
    public interface IUISpeakerController
    { 
        
        float Width { get; }
        float Height { get; }
        Image SpeakerPanel { get; }
        Canvas SpeakerCanvas { get; }
        TextMeshProUGUI Text { get; }

        void Hide();
        
        
        void Show(Vector2 pos, string txt);
    }
    public interface IHubController
    {

    }
}