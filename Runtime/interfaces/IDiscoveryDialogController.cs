using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using unvs.actors;
namespace unvs.interfaces
{
    public interface IDiscoveryDialog
    {
        Canvas DiscoveryDialogCanvas {  get; }
        Image DiscoveryDialogPanel {  get; }
        Image Icon { get; }
        TextMeshProUGUI Content { get; }

        AudioSource AudioSource { get; }
        AudioClip Sound { get; }

        UniTask DoShowDialogAsync(MonoBehaviour owner, MonoBehaviour source);
        void Hide();
        void Show();
    }
}

