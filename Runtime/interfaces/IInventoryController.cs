using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using unvs.ext;
using unvs.shares;
using unvs.ui;
namespace unvs.interfaces
{
    
    public interface IInventoryController
    {
        HybridDragSystem DragSystem { get; }
        Dictionary<string,GameObject> Storage { get; }
        float Size { get; }
        public DockDirection DockDirection { get; }
       
        GameObject Owner { get; set; }
        Image Container { get; }
        Image Bagger { get; }
        Canvas ContainerCanvas { get; }
        public Image InteractContainer { get; }

        bool Add(MonoBehaviour source);
        void Delete();
    }
}
