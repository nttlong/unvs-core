using Cysharp.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;
using unvs.baseobjects;
using unvs.interfaces;
using unvs.shares;
using unvs.ui;

namespace unvs.actors
{
    public class ActorInventoryObject : MonoBehaviour, IActorInventory
    {
        public string prefabPath;
        private SettingsUIInventory inventoryController;

        public string PrefabPath => prefabPath;

        public IInventoryController InventoryController => inventoryController;
        private void Awake()
        {
            if (Application.isPlaying)
            {
                if (!string.IsNullOrEmpty(prefabPath) && inventoryController == null)
                {
                    inventoryController = Commons.LoadPrefab<SettingsUIInventory>(prefabPath, transform);
                    inventoryController.Owner = this.gameObject;
                }
            }
        }
    }
}