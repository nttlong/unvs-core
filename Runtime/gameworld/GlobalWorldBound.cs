using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.gameword
{
    [RequireComponent(typeof(CompositeCollider2D))]
    public class GlobalWorldBound : MonoBehaviour, IGlobalWorldBound
    {
        [Header("Settings")]
        //public float scaleOut=2f;
        public bool autoGenerateGeometry = true;
        public bool autoInvalidateBoundingShapeCache=true;
        public bool fixDampling = true;
        public bool DefautlUseDelaunayMesh = false;
        public CompositeCollider2D coll;
        public Rigidbody2D rigiBody;

        public CompositeCollider2D Coll => coll;
        public static GlobalWorldBound Instance;
        

        public Rigidbody2D Rigidbody => rigiBody;

        

        //Dictionary<IScenePrefab,PolygonCollider2D> storage=new Dictionary<IScenePrefab, PolygonCollider2D> ();

        public void AddBound(IScenePrefab newScenePrefab)
        {

            var instance = SingleScene.Instance;
            newScenePrefab.WorldBound.refColl = newScenePrefab.WorldBound.Coll;
            if (newScenePrefab.WorkTracker == null)
            {
                var track = newScenePrefab.WorldBound.Coll.Clone(Constants.ObjectsConst.SCENE_TRACKER,false);
                track.transform.SetParent((newScenePrefab as MonoBehaviour).transform, true);
                var tck=track.AddComponent<WorldTrackerObject>();
                tck.enabled = false;
            } else
            {
                newScenePrefab.WorkTracker.Off();
            }


                newScenePrefab.WorldBound.DoScaleOnce(Constants.Settings.DEFAULT_WORLD_BOUND_SCALE);
            PolygonCollider2D newBounds = newScenePrefab.WorldBound.Coll;
            newBounds.useDelaunayMesh = DefautlUseDelaunayMesh;
            newBounds.SetMeOnLayer(Constants.Layers.WORLD_BOUND);
            newBounds.isTrigger = true;
            newScenePrefab.WorldBound.Owner = newScenePrefab;
            if (newBounds == null) return;
            newBounds.compositeOperation = Collider2D.CompositeOperation.Merge;
            newBounds.transform.SetParent(transform, true);
            newBounds.gameObject.SetActive(true);

            // Tắt Renderer để ẩn vùng Bound (nếu nó có hiển thị) nhưng giữ Collider hoạt động
            var renderer = newBounds.GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;

            // Force regenerate Composite Collider
            if(autoGenerateGeometry)
            coll.GenerateGeometry(); // Thay vì disable/enable, dùng hàm này chuyên dụng hơn


           
            if (fixDampling)
            {
                //SingleScene.Instance.VCam.CancelDamping();
                instance.Confiner.Damping = 5;
                
            }
            Instance.ClearOrphanBound();
            if (autoInvalidateBoundingShapeCache)
                instance.Confiner.InvalidateBoundingShapeCache();
           
           
        }

        public void ClearOrphanBound()
        {
            foreach (var item in coll.GetComponentsInChildren<IScenePrefabWorldBound>(true).Where(p => p.TrOWner.IsDestroyed()))
            {
                UnityEngine.Object.Destroy((item as MonoBehaviour).gameObject);
            }
        }

        private IEnumerator DelayedRefresh()
        {
            yield return new WaitForEndOfFrame();
            coll.enabled = false;
            coll.enabled = true;
            SingleScene.Instance.Confiner.InvalidateBoundingShapeCache();
        }



        public void RemoveBound(IScenePrefab newScenePrefab)
        {
            var wb =coll.GetComponentsInChildren<IScenePrefabWorldBound>(true).FirstOrDefault(p => p.Owner == newScenePrefab);
            if (wb == null)
            {
                //  UnityEngine.Object.Destroy(tr.gameObject);
                return;
            }
            var tr = (wb as MonoBehaviour);
            if (wb.Owner == newScenePrefab)
            {
                tr.gameObject.transform.SetParent(newScenePrefab.GoWorld.transform);
            }
            if (wb.Owner.IsDestroying || (wb.Owner as MonoBehaviour).IsDestroyed())
            {
                UnityEngine.Object.Destroy(tr.gameObject);
            }
           
            SingleScene.Instance.Confiner.InvalidateBoundingShapeCache();

        }
        private void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (SingleScene.Instance != null)
            {
                SingleScene.Instance.Confiner.BoundingShape2D = coll;
                SingleScene.Instance.Confiner.InvalidateBoundingShapeCache();
            }
            DontDestroyOnLoad(gameObject);

        }
        private void Awake()
        {
            coll = GetComponent<CompositeCollider2D>();
            coll.geometryType = CompositeCollider2D.GeometryType.Polygons; // Hợp nhất diện tích
            coll.vertexDistance = 0.0005f; // Hàn kín các đỉnh
            //coll.edgeRadius = 0.02f; // Chỉ để một chút để tránh kẹt, không để bằng
            coll.offsetDistance = 15f;
           
            rigiBody = GetComponent<Rigidbody2D>();
            rigiBody.bodyType = RigidbodyType2D.Static;

            // Đảm bảo nó vẫn tham gia tính toán vật lý
            rigiBody.simulated = true;
            coll.isTrigger = true;
        }

        public void SetBound(IScenePrefab newScenePrefab)
        {
            var instace = SingleScene.Instance;
            Clear();
            newScenePrefab.WorldBound.Owner = newScenePrefab;
            newScenePrefab.WorldBound.refColl = newScenePrefab.WorldBound.Coll;
            if (newScenePrefab.WorkTracker == null)
            {
                var track = newScenePrefab.WorldBound.Coll.Clone(Constants.ObjectsConst.SCENE_TRACKER);
                track.transform.SetParent((newScenePrefab as MonoBehaviour).transform, true);
                track.AddComponent<WorldTrackerObject>();
            }
            
            newScenePrefab.WorldBound.DoScaleOnce(Constants.Settings.DEFAULT_WORLD_BOUND_SCALE);
            PolygonCollider2D newBounds = newScenePrefab.WorldBound.Coll;
            newBounds.useDelaunayMesh = DefautlUseDelaunayMesh;
            newBounds.SetMeOnLayer(Constants.Layers.WORLD_BOUND);
          
            //var track = newScenePrefab.RefColliderWorldBound.CloneWithMarginByScale(SingleSceneController.Instance.WorldBoundMrgin);
           
            if (newBounds != null)
            {
                newBounds.transform.SetParent(transform, true);

                // 3. Kích hoạt tính năng "Gộp"
                // Khi dòng này bật lên, Composite Collider ở cha sẽ tự động nới rộng ra
                newBounds.compositeOperation = Collider2D.CompositeOperation.Merge;

                // 4. BẮT BUỘC: Báo cho Cinemachine biết là cái khung đã thay đổi kích thước
                // Nếu không có dòng này, Camera vẫn sẽ bị kẹt ở vùng cũ
                coll.GenerateGeometry();
                instace.Confiner.InvalidateBoundingShapeCache();
                //this.storage.Add(newScenePrefab, newBounds);
            }
            Instance.ClearOrphanBound();
            //newScenePrefab.WorldBound.Disable();
            // newScenePrefab.WorldBound.Disable();

        }

        public void Clear()
        {
            foreach (Transform tr in GetComponentInChildren<Transform>())
            {
                //tr.SetParent(tr.GetComponent<IScenePrefabWorldBound>().Owner.GoWorld.transform);
                Destroy(tr.gameObject);
            }
        }

        public IScenePrefabWorldBound FindByOwner(IScenePrefab chunk)
        {
            return coll.GetComponentsInChildren<IScenePrefabWorldBound>(true).FirstOrDefault(p=>p.Owner==chunk);
        }
        //private void OnDestroy()
        //{
        //    Debug.Log("loi");
        //}
    }
}