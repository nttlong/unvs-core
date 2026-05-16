


using System;

using UnityEngine;
using unvs.data;

namespace unvs.actions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TargetActionAttribute : Attribute { }
    [Serializable]
    public struct InteractSourceDest
    {
        public MonoBehaviour source;
        public MonoBehaviour target;
        public InteractSourceDest(MonoBehaviour source, MonoBehaviour target)
        {
            this.source = source;
            this.target = target;
        }
    }
    // Dùng để đánh dấu hàm nào được phép chọn trong Inspector

   

    [CreateAssetMenu(fileName = "Interaciondata", menuName = "Unvs/Data/Interaction Data")]
    public class InteractionDefinition : ScriptableObject
    {
        
        [SerializeReference]
        public ActionBase[] actions;

    }
}