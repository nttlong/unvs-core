using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

namespace unvs.interfaces
{
    public interface ISpeakableObject
    {
        LocalizedString MsgThisDoesNotDoAnything { get; }
        LocalizedString MsgICanNotDoThat { get; }
        void Say(LocalizedString message);
        Vector2 GetPosition();
        void SayText(string v);
        void Off();
        UniTask SayIThisDoesNotDoAnythingAsync();
        UniTask SayICanNotDoThatAsync();
        UniTask SayAsync(LocalizedString msg, params LocalizedString[] p);
        
    }
}