using System;
using UnityEngine;
using unvs.ext;
using unvs.game2d.objects;
using unvs.shares;

namespace unvs.components
{
    public partial class UnvsBagger : UnvsComponent
    {
        public Transform bagger;
        public event Action<GameObject> OnAddItem;
        public override void InitRuntime()
        {
            bagger.gameObject.SetActive(false);
        }

        public void AddItem(UnvsCollectableItem item)
        {
            item.owner=this;
            item.transform.SetParent(bagger.transform);
            OnAddItem?.Invoke(item.gameObject);


        }

        public UnvsCollectableItem FindItem(string name)
        {
            return this.GetComponentInChildrenByName<UnvsCollectableItem>(name);
        }
    }
#if UNITY_EDITOR
    public partial class UnvsBagger : UnvsComponent
    {
        

        private void OnValidate()
        {
            this.bagger = this.AddChildComponentIfNotExist<Transform>("bagger");
        }
    }
#endif
}