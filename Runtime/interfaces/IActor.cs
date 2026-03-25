using Aunvs.shares;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using unvs.interfaces;

namespace unvs.interfaces
{
    [Obsolete("IActor is deprecated. Please use IEntity for the new IActorObject system instead.")]
    public interface IActor
    {

        public bool IsActive { get; }
        public GameObject CamWatcher { get; }
        public float WalkSpeed { get; }
        public float RunSpeed { get; }
        public float Direction { get; }
        public float CrouchSpeed { get; }
      

        Vector2 Position { get; }
        ISpeakableObject Speaker { get; }
        IdleStateEnum IdleState { get; set; }

        UniTask IdleAsync();
        UniTask MoveToAsync(Vector2 pos, CancellationTokenSource ct);
        void MoveTo(Vector2 pos, CancellationTokenSource ct);
        UniTask SitdownAsync();
        void DoDestroy();
        void RunTo(Vector2 pos, CancellationTokenSource cts);
        void CrouchTo(Vector2 moveInput, CancellationTokenSource cts);
        //public void WalkTo(Vector2 pos, CancellationToken token);

    }
}