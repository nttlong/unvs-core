using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
namespace unvs.interfaces
{
    public interface IActorInventory
    {
        public GameObject Bager { get; }
        public IInventoryController InventoryController { get; }

        void Add(MonoBehaviour source);
    }
    public interface IActorDiscovery
    {
        IDiscoveryDialog Dialog { get; }
        UniTask ShowDialogAsync(MonoBehaviour source);
    }
}