using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
//using XGame.Core;
namespace unvs.interfaces
{
    //public interface IActionObject
    //{

    //    //UniTask<bool> ExecuteAsync(MonoBehaviour source, MonoBehaviour target, CancellationTokenSource ct);
        
    //}
    public enum ActorStatus
    {
        Idle, Crouch, Walk, Run,
        None
    }
    public interface IActorController
    {
        ActorStatus Status { get; }
        CancellationTokenSource Cts { get; set; }
        IAnimObject AnimObject { get; }
        //UniTask<bool> ExecuteAsync(MonoBehaviour source, MonoBehaviour target, CancellationTokenSource ct);
    }
    public interface IPickableObject
    {

    }
    public interface IStoragableObject
    {
        public Sprite Icon { get; }
        public string Name { get; }
        public UnityEngine.Localization.LocalizedString Description { get; }
    }
    public interface IWorldTracker
    {
        PolygonCollider2D Coll { get; }
        CancellationTokenSource Cts { get; set; }

        void Off();
        void On();
    }
}