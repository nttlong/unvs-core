using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using unvs.baseobjects;
using unvs.gameobjects;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace unvs.interfaces
{
    public interface IActorMovable
    {
        float WalkSpeed { get; set; }
        float RunSpeed { get; set; }
        Vector2 Direction { get; set; }
         
       Vector2 GetPostion();
        void MoveTo(Vector2 dir);
        UniTask MoveToAsync(Vector2 dir,Action<float> OnMoving, Action Stop,CancellationToken ct, float distance = 0);
    }
}