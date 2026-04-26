using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace unvs.controllers.inputs {
    
    [CreateAssetMenu(fileName = "ActorController", menuName = "Unvs/actors-controller")]
    public class UnvsActorController : ScriptableObject
    {
        public string[] actions;

        public UnvsActorController()
        {
            if (UnvsPlayerInputMap.Instance != null)
            {
                this.actions = UnvsPlayerInputMap.Instance.Players.ToArray();
            }
          
        }
    }
}