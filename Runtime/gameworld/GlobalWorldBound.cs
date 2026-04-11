//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using unvs.ext;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.gameword
//{
//    [RequireComponent(typeof(CompositeCollider2D))]
//    public class GlobalWorldBound : MonoBehaviour, IGlobalWorldBound
//    {
//        [Header("Settings")]
//        public bool isMultiPolygon=false;
//        //public float scaleOut=2f;
//        public bool autoGenerateGeometry = true;
//        public bool autoInvalidateBoundingShapeCache=true;
//        public bool fixDampling = true;
//        public bool DefautlUseDelaunayMesh = false;
//        public CompositeCollider2D coll;
//        public Rigidbody2D rigiBody;
//        public bool IsMultiPolygon => isMultiPolygon;
//        public CompositeCollider2D Coll => coll;
//        public static GlobalWorldBound Instance;
//        public PolygonCollider2D singleCollider;

//        public Rigidbody2D Rigidbody => rigiBody;
//        List<IScenePrefab> prefabList=new List<IScenePrefab>();

//        public void SetBound(IScenePrefab newScenePrefab)
//        {
//            if(isMultiPolygon) SetBoundModeMulti(newScenePrefab);
//            else SetBoundModeSingle(newScenePrefab);
//        }
//        public void SetBoundModeSingle(IScenePrefab newScenePrefab)
//        {
            
//            Clear();
//            newScenePrefab.WorldBound.Owner = newScenePrefab;
//            newScenePrefab.WorldBound.refColl = newScenePrefab.WorldBound.Coll;
//            //if (newScenePrefab.WorkTracker == null)
//            //{
//            //    var track = newScenePrefab.WorldBound.Coll.Clone(Constants.ObjectsConst.SCENE_TRACKER);
//            //    track.transform.SetParent((newScenePrefab as MonoBehaviour).transform, true);
//            //    track.AddComponent<WorldTrackerObject>();
//            //}
//            newScenePrefab.WorldBound.Coll.enabled = false;
//            prefabList.Add(newScenePrefab);
//            singleCollider.points = PolygonCollider2Extension.CreateRectFromVectorList(prefabList.Select(p => p.WorldBound.Coll).ToArray());
//            coll.GenerateGeometry();
//            GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();
           
//            //var track = newScenePrefab.RefColliderWorldBound.CloneWithMarginByScale(SingleSceneController.Instance.WorldBoundMrgin);


//        }
//        public void AddBound(IScenePrefab newScenePrefab)
//        {
//            if (isMultiPolygon) AddBoundModeMulti(newScenePrefab);
//            else AddBoundModeSingle(newScenePrefab);
//        }
//        public void AddBoundModeSingle(IScenePrefab newScenePrefab)
//        {
//            var instance = SettingsSingleScene.Instance;
//            newScenePrefab.WorldBound.refColl = newScenePrefab.WorldBound.Coll;
//            //if (newScenePrefab.WorkTracker == null)
//            //{
//            //    var track = newScenePrefab.WorldBound.Coll.Clone(Constants.ObjectsConst.SCENE_TRACKER, false);
//            //    track.transform.SetParent((newScenePrefab as MonoBehaviour).transform, true);
//            //    var tck = track.AddComponent<WorldTrackerObject>();
//            //    tck.enabled = false;
//            //}
//            //else
//            //{
//            //    newScenePrefab.WorkTracker.Off();
//            //}
//            newScenePrefab.WorldBound.Coll.enabled = false;
//            Instance.ClearOrphanBound();
//            prefabList.Add(newScenePrefab);
//            singleCollider.points = PolygonCollider2Extension.CreateRectFromVectorList(prefabList.Select(p => p.WorldBound.Coll).ToArray());
            



//            // Force regenerate Composite Collider
//            if (autoGenerateGeometry)
//                coll.GenerateGeometry(); // Thay vì disable/enable, dùng hàm này chuyên dụng hơn



//            if (fixDampling)
//            {
//                //SingleScene.Instance.VCam.CancelDamping();
//                GlobalApplication.Cinema.Confiner2D.Damping = 5;

//            }
            
//            if (autoInvalidateBoundingShapeCache)
//                GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();
//        }
//        public void SetBoundModeMulti(IScenePrefab newScenePrefab)
//        {
            
//            Clear();
//            newScenePrefab.WorldBound.Owner = newScenePrefab;
//            newScenePrefab.WorldBound.refColl = newScenePrefab.WorldBound.Coll;
//            //if (newScenePrefab.WorkTracker == null)
//            //{
//            //    var track = newScenePrefab.WorldBound.Coll.Clone(Constants.ObjectsConst.SCENE_TRACKER);
//            //    track.transform.SetParent((newScenePrefab as MonoBehaviour).transform, true);
//            //    track.AddComponent<WorldTrackerObject>();
//            //}

//            newScenePrefab.WorldBound.DoScaleOnce(Constants.Settings.DEFAULT_WORLD_BOUND_SCALE);
//            PolygonCollider2D newBounds = newScenePrefab.WorldBound.Coll;
//            newBounds.useDelaunayMesh = DefautlUseDelaunayMesh;
//            newBounds.SetMeOnLayer(Constants.Layers.WORLD_BOUND);

//            //var track = newScenePrefab.RefColliderWorldBound.CloneWithMarginByScale(SingleSceneController.Instance.WorldBoundMrgin);

//            if (newBounds != null)
//            {
//                newBounds.transform.SetParent(transform, true);

//                // 3. Kích hoạt tính năng "Gộp"
//                // Khi dòng này bật lên, Composite Collider ở cha sẽ tự động nới rộng ra
//                newBounds.compositeOperation = Collider2D.CompositeOperation.Merge;

//                // 4. BẮT BUỘC: Báo cho Cinemachine biết là cái khung đã thay đổi kích thước
//                // Nếu không có dòng này, Camera vẫn sẽ bị kẹt ở vùng cũ
//                coll.GenerateGeometry();
//                GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();
//                //this.storage.Add(newScenePrefab, newBounds);
//            }
//            Instance.ClearOrphanBound();
//            //newScenePrefab.WorldBound.Disable();
//            // newScenePrefab.WorldBound.Disable();

//        }
//        public void AddBoundModeMulti(IScenePrefab newScenePrefab)
//        {

            
//            newScenePrefab.WorldBound.refColl = newScenePrefab.WorldBound.Coll;
//            if (newScenePrefab.WorkTracker == null)
//            //{
//            //    var track = newScenePrefab.WorldBound.Coll.Clone(Constants.ObjectsConst.SCENE_TRACKER,false);
//            //    track.transform.SetParent((newScenePrefab as MonoBehaviour).transform, true);
//            //    var tck=track.AddComponent<WorldTrackerObject>();
//            //    tck.enabled = false;
//            //} else
//            //{
//            //    newScenePrefab.WorkTracker.Off();
//            //}


//                newScenePrefab.WorldBound.DoScaleOnce(Constants.Settings.DEFAULT_WORLD_BOUND_SCALE);
//            PolygonCollider2D newBounds = newScenePrefab.WorldBound.Coll;
//            newBounds.useDelaunayMesh = DefautlUseDelaunayMesh;
//            newBounds.SetMeOnLayer(Constants.Layers.WORLD_BOUND);
//            newBounds.isTrigger = true;
//            newScenePrefab.WorldBound.Owner = newScenePrefab;
//            if (newBounds == null) return;
//            newBounds.compositeOperation = Collider2D.CompositeOperation.Merge;
//            newBounds.transform.SetParent(transform, true);
//            newBounds.gameObject.SetActive(true);

//            // Tắt Renderer để ẩn vùng Bound (nếu nó có hiển thị) nhưng giữ Collider hoạt động
//            var renderer = newBounds.GetComponent<Renderer>();
//            if (renderer != null) renderer.enabled = false;

//            // Force regenerate Composite Collider
//            if(autoGenerateGeometry)
//            coll.GenerateGeometry(); // Thay vì disable/enable, dùng hàm này chuyên dụng hơn


           
//            if (fixDampling)
//            {
//                //SingleScene.Instance.VCam.CancelDamping();
//                GlobalApplication.Cinema.Confiner2D.Damping = 5;
               
//            }
//            Instance.ClearOrphanBound();
//            if (autoInvalidateBoundingShapeCache)
//                GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();
           
           
//        }

//        public void ClearOrphanBound()
//        {
//            foreach (var item in coll.GetComponentsInChildren<IScenePrefabWorldBound>(true).Where(p => p.TrOWner.IsDestroyed()))
//            {

//                UnityEngine.Object.Destroy((item as MonoBehaviour).gameObject);
//            }
//            var phantom=prefabList.FirstOrDefault(p=>p.IsDestroying);
//            while (phantom != null)
//            {
//                prefabList.Remove(phantom);
//                phantom = prefabList.FirstOrDefault(p => p.IsDestroying);
//            }
//        }

//        private IEnumerator DelayedRefresh()
//        {
//            yield return new WaitForEndOfFrame();
//            coll.enabled = false;
//            coll.enabled = true;
//            GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();
//        }
//        public void RemoveBound(IScenePrefab newScenePrefab)
//        {
//            if(IsMultiPolygon) RemoveBoundMultiMode(newScenePrefab);
//            else RemoveBoundSingleMode(newScenePrefab);
//        }
//        private void RemoveBoundSingleMode(IScenePrefab newScenePrefab)
//        {
//            var instance = SettingsSingleScene.Instance;
//            prefabList.Remove(newScenePrefab);


//            Instance.ClearOrphanBound();
//            prefabList.Add(newScenePrefab);
//            singleCollider.points = PolygonCollider2Extension.CreateRectFromVectorList(prefabList.Select(p => p.WorldBound.Coll).ToArray());




//            // Force regenerate Composite Collider
//            if (autoGenerateGeometry)
//                coll.GenerateGeometry(); // Thay vì disable/enable, dùng hàm này chuyên dụng hơn



//            if (fixDampling)
//            {
//                //SingleScene.Instance.VCam.CancelDamping();
//                GlobalApplication.Cinema.Confiner2D.Damping = 5;

//            }

//            if (autoInvalidateBoundingShapeCache)
//                GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();
//        }

//        private void RemoveBoundMultiMode(IScenePrefab newScenePrefab)
//        {
//            var wb =coll.GetComponentsInChildren<IScenePrefabWorldBound>(true).FirstOrDefault(p => p.Owner == newScenePrefab);
//            if (wb == null)
//            {
//                //  UnityEngine.Object.Destroy(tr.gameObject);
//                return;
//            }
//            var tr = (wb as MonoBehaviour);
//            if (wb.Owner == newScenePrefab)
//            {
//                tr.gameObject.transform.SetParent(newScenePrefab.GoWorld.transform);
//            }
//            if (wb.Owner.IsDestroying || (wb.Owner as MonoBehaviour).IsDestroyed())
//            {
//                UnityEngine.Object.Destroy(tr.gameObject);
//            }

//            GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();

//        }
//        private void Start()
//        {
//            if (Instance != null)
//            {
//                Destroy(gameObject);
//                return;
//            }
//            Instance = this;
//            if (GlobalApplication.Cinema != null )
//            {
//                GlobalApplication.Cinema.Confiner2D.BoundingShape2D = coll;
//                GlobalApplication.Cinema.Confiner2D.InvalidateBoundingShapeCache();
//            }
//            DontDestroyOnLoad(gameObject);

//        }
//        private void InitRunTime()
//        {
//            if (Application.isPlaying)
//            {
//                singleCollider = this.transform.AddChildComponentIfNotExist<PolygonCollider2D>(Constants.ObjectsConst.GLOBAL_WORLD_BOUND_SINGLE_POLYGON);
//                singleCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
//                singleCollider.isTrigger = true;
//            }
//        }
//        private void Awake()
//        {
//            InitRunTime();
//            coll = GetComponent<CompositeCollider2D>();
//            coll.geometryType = CompositeCollider2D.GeometryType.Polygons; // Hợp nhất diện tích
//            coll.vertexDistance = 0.0005f; // Hàn kín các đỉnh
//            //coll.edgeRadius = 0.02f; // Chỉ để một chút để tránh kẹt, không để bằng
//            coll.offsetDistance = 15f;
           
//            rigiBody = GetComponent<Rigidbody2D>();
//            rigiBody.bodyType = RigidbodyType2D.Static;

//            // Đảm bảo nó vẫn tham gia tính toán vật lý
//            rigiBody.simulated = true;
//            coll.isTrigger = true;
//        }

        

//        public void Clear()
//        {
//            if(isMultiPolygon)
//            ClearModeMulty();
//            else ClearModeSingle();
//        }

//        private void ClearModeSingle()
//        {
//            singleCollider.points = null;
//            this.prefabList.Clear();
//        }

//        private void ClearModeMulty()
//        {
//            foreach (Transform tr in GetComponentInChildren<Transform>())
//            {
//                //tr.SetParent(tr.GetComponent<IScenePrefabWorldBound>().Owner.GoWorld.transform);
//                Destroy(tr.gameObject);
//            }
//        }

//        public IScenePrefabWorldBound FindByOwner(IScenePrefab chunk)
//        {
//            return coll.GetComponentsInChildren<IScenePrefabWorldBound>(true).FirstOrDefault(p=>p.Owner==chunk);
//        }
//        //private void OnDestroy()
//        //{
//        //    Debug.Log("loi");
//        //}
//    }
//}