

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using unvs.actions;
namespace unvs.interfaces
{
    public interface ITeleportPrefab
    {
        AudioClip OpenSound { get; }
        AudioClip CloseSound { get; }
        string PathToWord { get; }
        bool IsNew { get; }
        Texture2D TextT { get; }
        Sprite Sprite { get; }
        SpriteRenderer SpriteRenderer { get; }
        string TargetName { get; }
        bool HideSpriteWhenPlaying { get; }
        
    }
}
//public interface ITeleportObject
//{

//    public AudioClip SoundStart { get; }
//    public AudioClip SoundEnd { get; }

//    public bool ToNewWorld { get; }
//    public string SceneName { get; }
//    public string TargetSpawnName { get; }

   
//}

