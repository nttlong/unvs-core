using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace unvs.interfaces
{
    public interface IMovableObject
    {
        public float Speed { get; }
        public float Direction { get; }
        UniTask MoveToAsync(IInteractableObject target, CancellationTokenSource ct);
        UniTask MoveToPositionAsync(Vector2 obj, float speed, CancellationTokenSource ct);

        Action OnMoving { get; set; }
    }
}
