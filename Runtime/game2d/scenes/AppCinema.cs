using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace unvs.game2d.scenes{
    public class AppCinema : AppUIElemen<AppCinema>
    {
        public Camera cam;
        public CinemachineCamera vcam;
        public CompositeCollider2D compositeCollider2D;
        public CinemachineConfiner2D confiner;

        public override void InitRunTime()
        {
           
        }
    }
}