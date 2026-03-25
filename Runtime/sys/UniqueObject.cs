
using UnityEngine;
using unvs.interfaces;
namespace unvs.sys
{
    
    public class UniqueObject : MonoBehaviour, IUniqueObject
    {
        IUniqueObject instance;

        public bool IsValidate { get; set; }

        // Start is called before the first frame update
        private void Awake()
        {
            instance=this;
            instance.IsValidate= PersitanceObjectManager.Validate(this);
        }
        private void OnEnable()
        {
            //PersitanceObjectManager.Validate(this);
        }
        private void OnDestroy()
        {
            var key = $"name={name},type={this.GetType().Name}";
            if (PersitanceObjectManager.map.TryGetValue(key, out var instance))
            {
                if (instance == this.gameObject)
                {
                    PersitanceObjectManager.Remove(key);
                }
            }
        }

    }
}
