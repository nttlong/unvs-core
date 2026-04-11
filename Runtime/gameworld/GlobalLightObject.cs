//using System.Collections;
//using UnityEngine;
//using UnityEngine.Rendering.Universal;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.gameword
//{
//    [ExecuteInEditMode]
//    [RequireComponent(typeof(Light2D))]
//    public class GlobalLightObject : MonoBehaviour, IGlobalLightWapper
//    {
//        public IScenePrefab owner;
       
//        public Light2D lightSource;
//        public Vector2 position;
//        public GameObject goOwner;
        

//        public IScenePrefab Owner => owner;

//        public Light2D Light
//        {
//            get
//            {
//                var wt = this.GetComponentInParent<IWorldTracker>();
//                if (lightSource != null)
//                {
//                    if (wt != null) lightSource.transform.position = wt.Coll.bounds.center;
//                    return lightSource;
//                }
//                lightSource = GetComponent<Light2D>();
//                if (wt != null) lightSource.transform.position = wt.Coll.bounds.center;
//                return lightSource;
//            }
//        }

//        public Vector2 Position { get => position; set => position = value; }

//        public GameObject GoOwner => goOwner;

        

//        public void TunOff()
//        {
//            Light?.gameObject.SetActive(false);
//        }
//        private void Awake()
//        {
//            lightSource = GetComponent<Light2D>();
//            owner = GetComponentInParent<IScenePrefab>(true);
//            goOwner = owner.GoWorld;
           
//        }
//    }
//}