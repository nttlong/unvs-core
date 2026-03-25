using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

namespace unvs.interfaces
{
    public interface ISpeakableObject
    {
        LocalizedString MsgICanNotDoThat { get; }
        void Say(LocalizedString message);
        Vector2 GetPosition();
        void SayText(string v);
        void Off();
        UniTask SayICanNotDoThatAsync();
    }
}