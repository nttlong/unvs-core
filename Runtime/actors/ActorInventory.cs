using Cysharp.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using unvs.interfaces;
using unvs.shares;

namespace unvs.actors
{
    public class ActorInventoryObject : MonoBehaviour, IActorInventory
    {
        public GameObject bager;
        public GameObject Bager => bager;
        private IInventoryController inventoryController;
        public IInventoryController InventoryController => inventoryController;

        public void Add(MonoBehaviour source)
        {
            if (!inventoryController.Add(source))
            {
                GetComponent<ISpeakableObject>().SayIThisDoesNotDoAnythingAsync().Forget();
            }

        }
        private void Start()
        {
            if (bager == null)
            {

                bager = Commons.LoadPrefabs("UI/Inventory/Inventory");

                if (bager != null)
                {
                    this.inventoryController = bager.GetComponent<IInventoryController>();
                    if (this.inventoryController != null)
                    {
                        this.inventoryController.Owner = this.gameObject;
                    }
                    bager.transform.SetParent(transform);



                }
            }
        }
        private void OnDestroy()
        {
            if (bager != null)
            {
                if (this.inventoryController != null)
                {
                    this.inventoryController.Delete();
                }
                Destroy(bager);

            }
        }
    }
}