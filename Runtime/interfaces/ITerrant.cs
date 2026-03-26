using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
namespace unvs.interfaces
{
    public interface ITerrant
    {
        public AudioClip Sound { get; }
    }
    public enum WallTypeEnum
    {
        Left, Right
    }
    public interface IWall
    {
        WallTypeEnum WallType { get; }
        BoxCollider2D Coll { get; }

    }
    public enum TriggerZoneDirection
    {
        Left, Right
    }
    public interface ITriggerZone
    {
        TriggerZoneDirection Direction { get; set; }
        string TriggerPath { get; }
        BoxCollider2D Coll { get; }

        void Off();
        void On();
    }
    public enum CamTrackingDirectionEnum
    {
        Left, Right, Top, Bottom
    }


    public interface ICamBody
    {
        ICamCenterCamTracking Tracker { get; }
        //void AlignCeter(BoxCollider2D master, float watcherSize = 0.1f);
        void UpdateSizeByLensSettings(LensSettings lens);
    }

}