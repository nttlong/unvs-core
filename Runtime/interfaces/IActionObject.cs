using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using unvs.shares;
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
        IActorObject Actor { get; }
        //UniTask<bool> ExecuteAsync(MonoBehaviour source, MonoBehaviour target, CancellationTokenSource ct);
    }
    public interface IPickableObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Direction">Direction of actor when moving</param>
        /// <param name="ReachDistance">Arm lenght of actor</param>
        /// <returns></returns>
        Vector2 GetPosition(float Direction, float ReachDistance);
        Texture2D TexT { get; }
        SpriteRenderer SpriteR { get; }
        bool HideSpriteRendererWhenPlaying { get; }
        Vector2 Size { get; }
    }
    public interface IStoragableObject
    {
        public Sprite Icon { get; }
        public string Name { get; }
        /// <summary>
        /// Name in game which is called by actor or NPC
        /// </summary>
        public LocalizedString GameName { get; }
        public LocalizedString Description { get; }
       
    }
    public interface IAudiableObject
    {
        AudioInfo DiscoverySound { get; }
    }
    public interface IWorldTracker
    {
        PolygonCollider2D Coll { get; }
        CancellationTokenSource Cts { get; set; }

        void Off();
        void On();
    }
}