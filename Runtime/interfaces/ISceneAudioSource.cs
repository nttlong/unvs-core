using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace unvs.interfaces
{
    public interface ISceneAudioSource
    {
        AudioSource Source { get; }

        UniTask CrossFadeAsync(ISceneAudioSource ambient,
            CancellationToken cancellationToken = default);
    }
}