//using Assets.Prefabs.InteractRequire;

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace unvs.actions
{
    public struct ActionBaseSender
    {
        public MonoBehaviour Source;
        public MonoBehaviour Target;
        public bool IsCancel;
        public CancellationTokenSource Cts;

        public void Cancel()
        {
            IsCancel=true;
            this.Cts?.Cancel();
        }
    }
    [Serializable]
    public abstract class ActionBase // Chuyển thành abstract
    {


        public string name;

        public abstract UniTask ExecuteAsync(ActionBaseSender Sender);
    }
}