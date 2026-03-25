using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using unvs.interfaces;
using unvs.shares;

namespace unvs.gameworld
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CamTrackingObject : MonoBehaviour, ICamCenterCamTracking
    {
        [SerializeField] private BoxCollider2D coll;

        // Sử dụng List nội bộ để quản lý việc thêm/xóa linh hoạt
        private List<IScenePrefab> _scenesList = new List<IScenePrefab>();
        private IScenePrefab[] _scenesHit = new IScenePrefab[0];
        public Transform[] scenesHitTr;
        public BoxCollider2D Collider
        {
            get
            {
                if (coll != null) return coll;
                coll = GetComponent<BoxCollider2D>();
                return coll;
            }
        }
        private void Awake()
        {
            Collider.isTrigger = true;
            GlobalApplication.CamTracking = this;
        }
        // Trả về mảng để tuân thủ Interface nhưng vẫn đảm bảo dữ liệu mới nhất
        public IScenePrefab[] ScensHit => _scenesHit;

        public void AddScene(IScenePrefab sc)
        {
            if (sc == null || _scenesList.Contains(sc)) return;
            sc.OnDestroyMe = p =>
            {
                RemoveScene(p);
            };
            var check = _scenesList.Where(p => p != null && !p.IsDestroying).FirstOrDefault(p => p.Name == sc.Name);
            if (check != null) return;
            _scenesList.Add(sc);
            UpdateScenesArray();
            scenesHitTr = _scenesList.Select(p => p.GoWorld.transform).ToArray();


        }



        public void RemoveScene(IScenePrefab sc)
        {
            if (sc == null || !_scenesList.Contains(sc)) return;

            _scenesList.Remove(sc);
            UpdateScenesArray();
            scenesHitTr = _scenesList.Select(p => p.GoWorld.transform).ToArray();
        }

        private void UpdateScenesArray()
        {
            _scenesHit = _scenesList.ToArray();
        }



        private object getRect(PolygonCollider2D coll)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _scenesHit = new IScenePrefab[0];
        }
    }
}