using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using unvs.actors.actions;

namespace unvs.interfaces
{
    public interface IActorObject
    {
        CancellationTokenSource Cts { get; set; }
        CapsuleCollider2D Coll { get; }
        Rigidbody2D Body { get; }
        IActorMotion Motion { get; }
        ISpeakableObject Speaker { get; }
        ICamWacher CamWacher { get; }
        IActorMovable Movable { get; }
        IActorInteractable Interactable { get; }
        IActorPhysical Physical { get; }
        DirectionEnum SideView { get; set; }
        ActorsControllers Controller { get; }
        bool IsActive { get; set; }

        UniTask MoveToAsync(Vector2 Pos, CancellationTokenSource ct);
       

        Action OnDestroying { get; set; }
    }
}