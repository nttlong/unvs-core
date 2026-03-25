
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using unvs.actions;
namespace unvs.interfaces
{
    public enum ExploringType
    {
        None,
        Unknown,
        Pick,

    }
    public interface IInteractableObject
    {
        public ExploringType Exploring { get; }
        public InteractionDefinition Data { get; }
        public Collider2D Collider { get; }
       

        public UniTask<bool> ExecAsync( MonoBehaviour target, CancellationTokenSource token);
        Vector2 GetPosition();
    }
}