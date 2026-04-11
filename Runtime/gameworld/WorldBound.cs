//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Cinemachine;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using unvs.ext;
//using unvs.shares;
//namespace unvs.gameword
//{
//    [RequireComponent(typeof(PolygonCollider2D))]
//    public class WorldBound : MonoBehaviour
//    {
//        public string SceneName;
//        private void Awake()
//        {
//            SceneName = SceneManager.GetActiveScene().name;
//            GetComponent<PolygonCollider2D>().isTrigger = true;

//        }
//        public void Start()
//        {
//            this.SetMeOnLayer(Constants.Layers.WORLD_BOUND);
//            if (GlobalApplication.Cinema == null) return;
//            if (GlobalApplication.Cinema.Confiner2D.BoundingShape2D == null)
//            {
//                GlobalApplication.Cinema.Confiner2D.BoundingShape2D = GetComponent<PolygonCollider2D>();
//            }
//        }
//        //private void OnEnable()
//        //{
//        //    if (SceneController.instance == null) return;
//        //    if (SceneController.instance.confiner.BoundingShape2D == null)
//        //    {
//        //        SceneController.instance.confiner.BoundingShape2D = GetComponent<PolygonCollider2D>();
//        //    }
//        //    else
//        //    {
//        //        if (!SceneName.Equals(SceneLoader.CurrentScene, StringComparison.OrdinalIgnoreCase))
//        //        {
//        //            SceneController.instance.confiner.BoundingShape2D = GetComponent<PolygonCollider2D>();
//        //        }
//        //    }
//        //}
//        // Start is called before the first frame update



//    }
//}