using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.shares;
namespace unvs.interfaces
{
    
    public interface IInventoryController
    {
        float Size { get; }
        public DockDirection DockDirection { get; }
       
        GameObject Owner { get; set; }
        Image Container { get; }
        Canvas ContainerCanvas { get; }
        public IDragAndDropContainer InteractContainer { get; }

        bool Add(MonoBehaviour source);
        void Delete();
    }
}
