using game2d.scenes;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.interfaces;

namespace unvs.game2d.scenes{
    public class UnvsCinema : UnvsUIComponentInstance<UnvsCinema>
    {
        public Camera cam;
        public CinemachineCamera vcam;
        public CompositeCollider2D compositeCollider2D;
        public CinemachineConfiner2D confiner;
        private Rigidbody2D rb;
        private MonoBehaviour camTracking;

       
#if UNITY_EDITOR
        [UnvsButton()]
        public void Generate()
        {
            var cinema = this;
            cinema.cam = cinema.AddChildComponentIfNotExist<Camera>("Main Camera");
            cinema.cam.tag = "MainCamera";
            cinema.cam.orthographic = true;
            cinema.cam.AddComponentIfNotExist<CinemachineBrain>();
            cinema.cam.AddComponentIfNotExist<AudioListener>();
            cinema.vcam = cinema.AddChildComponentIfNotExist<CinemachineCamera>("VCam");
            cinema.vcam.AddComponentIfNotExist<CinemachineFollow>();
            cinema.compositeCollider2D = cinema.AddChildComponentIfNotExist<CompositeCollider2D>("compositeCollider2D");

            cinema.compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
            cinema.confiner = cinema.vcam.AddComponentIfNotExist<CinemachineConfiner2D>();
            cinema.confiner.BoundingShape2D = cinema.compositeCollider2D;
            var b = cinema.cam.AddComponentIfNotExist<Rigidbody2D>();
            b.bodyType = RigidbodyType2D.Static;
            var c = cinema.cam.AddComponentIfNotExist<BoxCollider2D>();
            c.isTrigger = true;
        }

        public override void InitEvents()
        {
            //throw new System.NotImplementedException();
        }
#endif
    }
}