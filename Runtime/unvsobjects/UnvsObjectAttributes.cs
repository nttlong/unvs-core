using UnityEngine;
using System;
using unvs.actions;

namespace unvs.unvsobjects
{
    [Serializable]
    public abstract class InteractionTrait
    {
        public string traitName;
        public virtual bool CanInteract(MonoBehaviour actor) => true;
    }

    public class TraitSelectorAttribute : PropertyAttribute { }

    public abstract class UnvsObjectAttributes
    {

    }
    
}