
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
        Pickable,
        Openable

    }
    public interface IInteractableObject
    {
        ExploringType Exploring { get; }
        InteractionDefinition Data { get; }
        Collider2D Collider { get; }
       

        UniTask<bool> ExecAsync( MonoBehaviour target, CancellationTokenSource token);
        Vector2 GetPosition();
    }
    public interface IConsumerObject
    {
        InteractionDefinition ConsumeDefinintion { get; }
    }
    public interface IConsumableItem
    {
        IActorObject Owner { get; set; }
    }
}