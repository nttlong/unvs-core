using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace unvs.interfaces
{
    public interface IInventoryController
    {
        GameObject Owner { get; set; }
        Image Container { get; }
        public IDragAndDropContainer InteractContainer { get; }

        bool Add(MonoBehaviour source);
        void Delete();
    }
}
