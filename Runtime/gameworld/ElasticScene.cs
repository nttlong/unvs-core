//using Cysharp.Threading.Tasks;
//using System;
//using Unity.VisualScripting;

//using UnityEngine;
//using unvs.interfaces;
//namespace unvs.gameword
//{
//    public class ElasticScene : MonoBehaviour, IElasticScene
//    {

//        public ScenePrefabTypeEnum worldType;
//        public string leftScenePath;
//        public string rightScenePath;


//        public IScenePrefab left;
//        public IScenePrefab right;


        

//        public ScenePrefabTypeEnum WorldType => worldType;



//        public string LeftScenePath => leftScenePath;



//        public string RightScenePath => rightScenePath;

//        public IScenePrefab CurrentWorld
//        {
//            get
//            {

//                return SettingsSingleScene.Instance.CurrentWorld; ;
//            }
//        }

//        public IScenePrefab Left { get => left; set => left = value; }

//        public IScenePrefab Right { get => right; set => right = value; }



//        public IScenePrefab Prefab => GetComponent<IScenePrefab>();

//        //private void Awake()
//        //{
//        //    if (Application.isPlaying)
//        //    {
//        //        if (worldType == ScenePrefabTypeEnum.Interior)
//        //        {
//        //           var r= this.GetComponentInChildrenByName<WorldTrackerObject>("SceneTracker");
//        //            if (r != null)
//        //            {
//        //                r.gameObject.SetActive(false);
//        //                r.enabled = false;
//        //            }
//        //        }
//        //    }
//        //}
//        //public void AppendRightScene(Vector2 joinPos, IScenePrefab RightScene)
//        //{
//        //    var rightJoinPoint = RightScene.Floor.GetIntersectPointWithFirstEdge(RightScene.LeftWall);
//        //    // dua vao 2 diem joinPos la diem tan cung ben phai cua Current Scene
//        //    // va rightJoinPoint la diem tan cung ben trai cua RightScene
//        //    // di chuyen RightScene sao cho khop voi joinPos
//        //    Vector3 offset = (Vector3)joinPos - (Vector3)rightJoinPoint;
//        //    RightScene.GoWorld.transform.position += offset;

//        //}

//        //public void AppendLeftScene(Vector2 joinPos, IScenePrefab LeftScene)
//        //{
//        //    var leftJoinPoint = LeftScene.Floor.GetIntersectPointWithLastEdge(LeftScene.RightWall);

//        //     // dua vao 2 diem joinPos la diem tan cung ben trai cua Current Scene
//        //    // va leftJoinPoint la diem tan cung ben phai cua LeftScene
//        //    // di chuyen LeftScene sao cho khop voi joinPos
//        //    Vector3 offset = (Vector3)joinPos - (Vector3)leftJoinPoint;
//        //    LeftScene.GoWorld.transform.position += offset;
//        //}

       






//    }
//}