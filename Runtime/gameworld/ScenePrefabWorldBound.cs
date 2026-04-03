using System;
using Unity.VisualScripting;
using UnityEngine;
using unvs.ext;
using unvs.interfaces;
namespace unvs.gameword
{
    [ExecuteAlways]
    public class ScenePrefabWorldBound : MonoBehaviour, IScenePrefabWorldBound
    {
        public PolygonCollider2D coll;
        private PolygonCollider2D _refColl;
        public GameObject trOwer;
        private IScenePrefab owner;
        public bool isScale;

        public PolygonCollider2D refColl { get => _refColl; set => _refColl = value; }
        public PolygonCollider2D Coll
        {
            get
            {
                //if(_refColl!=null) coll=_refColl;
                //if(this.gameObject.IsDestroyed()) return null;
                //if(coll == null) coll = this.AddComponentIfNotExist<PolygonCollider2D>();
                return coll;
            }
            set { coll = value; }
        }

        public IScenePrefab Owner
        {
            get => owner; set
            {
                
                trOwer = value?.GoWorld;
                owner = value;
            }

        }

        public GameObject TrOWner => trOwer;

        public bool IsScale => isScale;

        //public GameObject Owner => gameObject;



        public void Disable()
        {

            //gameObject.GetComponent<PolygonCollider2D>()?.gameObject.SetActive(false);
            //this.enabled = false;
            //gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (coll == null)
            {
                coll = this.AddComponentIfNotExist<PolygonCollider2D>();
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            // Đảm bảo khi Play, nếu coll vẫn null thì tìm lại lần cuối
            if (coll == null) { coll = GetComponent<PolygonCollider2D>(); coll.isTrigger = true; }

        }

        public void Restore()
        {
            if(GlobalWorldBound.Instance.isMultiPolygon)
            Coll.transform.SetParent(owner.GoWorld.transform, false);
        }

        public void DoScaleOnce(float scale)
        {
            if (IsScale) return;
            this.coll.ClearRotationAndScale(scale, PolygonCollider2Extension.AxisScaleEnum.y);
        }
    }
}