using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace unvs.interfaces
{
    public interface IDynamicSceneManager
    {

        Transform Container { get; }
        BoxCollider2D Left { get; set; }
        BoxCollider2D Right { get; set; }
       
        float TriggerSize { get; }

        UniTask DoLoadRightAsync();
        UniTask DoLoadLeftAsync();
        bool IsReady(string rightScenePath);

        bool IsLockEvent { get; set; }

    }
}