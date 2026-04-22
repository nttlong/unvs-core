//using interfaces.cinema;

//using System;
//using Unity.Cinemachine;
//using Unity.VisualScripting;
//using UnityEngine;
//
//using unvs.shares;
//namespace unvs.manager
//{

//    //public class SceneComponents
//    //{
//    //    public Camera Main;
//    //    public CinemachineBrain Brain;
//    //    public CinemachineCamera VCam;
//    //    public GlobalWorldBound WorldBound;

//    //    public CinemachineConfiner2D Confiner;
//    //}
//    public class SetupSceneWorld
//    {
//        //public static SceneComponents CreateComponents(Transform transform, string PrefabPath=null)
//        //{
//        //    if (!string.IsNullOrEmpty(PrefabPath))
//        //    {
//        //        return loadFormPrefab(PrefabPath);
//        //    }
//        //    var ret = new SceneComponents();
//        //    ret.Main = CreateMainCam(transform);
//        //    ret.Brain = ret.Main.GetComponent<CinemachineBrain>();
//        //    ret.VCam = CreateVCam(transform);
//        //    ret.WorldBound = CreateWorldBound(transform);
//        //    ret.Confiner = ret.VCam.GetComponent<CinemachineConfiner2D>();
//        //    return ret;
//        //}

//        //private static SceneComponents loadFormPrefab(string prefabPath)
//        //{
//        //    var ret = new SceneComponents();
//        //    var go = Commons.LoadPrefab<ICinemaScene>(prefabPath);
//        //    if(go==null)
//        //    {
//        //        throw new Exception($"{typeof(ICinemaScene)} not found in {prefabPath}");
//        //    }
//        //    ret.Main = go.Main;
//        //    ret.Brain = go.Brain;
//        //    ret.VCam = go.VCam;
//        //    ret.WorldBound = go.WorldBound;
//        //    ret.Confiner = ret.VCam.GetComponent<CinemachineConfiner2D>();
//        //    return ret;
//        //}

//        //private static GlobalWorldBound CreateWorldBound(Transform transform)
//        //{
//        //    var go = new GameObject("GlobalWorldBound");
//        //    go.transform.SetParent(transform);
//        //    var ret = go.AddComponent<GlobalWorldBound>();
//        //    return ret;

//        //}

//        //private static CinemachineCamera CreateVCam(Transform transform)
//        //{
//        //    var go = new GameObject("VCam");
//        //    go.transform.SetParent(transform);
//        //    var ret = go.AddComponent<CinemachineCamera>();
//        //    go.AddComponent<CompositeCollider2D>();
//        //    go.AddComponent<CinemachineConfiner2D>();
//        //    var rb = ret.GetComponent<Rigidbody2D>();
//        //    rb.gravityScale = 0;
//        //    rb.bodyType = RigidbodyType2D.Kinematic;
//        //    ret.transform.position = new Vector3(0, 0, -10);
//        //    ret.gameObject.AddComponent<CinemachineFollow>();

//        //    // 2. Để gán Rotation Control là "None" (Fix lỗi xoay như Ảnh 2)


//        //    return ret;

//        //}

//        private static Camera CreateMainCam(Transform transform)
//        {
//            var go = new GameObject("Main Camera");
//            go.transform.SetParent(transform);
//            go.tag = "MainCamera";
//            var ret = go.AddComponent<Camera>();
//            go.AddComponent<CamObject>();
//            go.AddComponent<AudioListener>();
//            var brain = go.AddComponent<CinemachineBrain>();
//            return ret;

//        }
//    }
//}