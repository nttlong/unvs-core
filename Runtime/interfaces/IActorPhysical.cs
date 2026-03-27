using System.Collections;
using UnityEngine;

namespace unvs.interfaces
{
    public enum DirectionEnum
    {
        Forward,
        Backward,
    }
    public interface IActorPhysical {
        public float ArmLen { get; }
        public Transform RootArm { get; }
        public Transform TopArm { get; }
        public Transform SocketBack { get; }
        public Transform SocketFront { get; }
        public IActorIK ActorIK { get; }
        DirectionEnum Direction { get; set; }
        MonoBehaviour CurrentHoldingObject { get; set; }
        float ReachTolerance { get; }
        bool CanReachTarget(Vector2 pos);
        Vector2 GetHandPosition();
        Vector2 GetPosition();
        void HoldItem(MonoBehaviour monoBehaviour);
       
        void Validate();
        bool IsTargetLower(Vector2 pos);
    }
}