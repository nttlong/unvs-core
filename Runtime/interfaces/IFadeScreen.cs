using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace unvs.interfaces
{
    public interface IFadeScreen
    {
        UniTask FadeInAsync(float fadingTime = 0.5f);
        UniTask FadeOutAsync(float fadingTime = 0.5f);
        Canvas UICanvas { get; }
        Image Panel { get; }
        void Hide();
    }
}
