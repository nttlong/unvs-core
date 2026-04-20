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
    [CreateAssetMenu(fileName = "Object-Attributes", menuName = "Systems/Object Attributes")]
public class UnvsObjectAttributesData : ScriptableObject
{
    [Header("Visuals")]
    public Texture2D customCursor;
    public float interactDistance = 2f;
    [Header("Traits")]
    [SerializeReference, TraitSelector] // Giống như bộ Skill của bạn
    public InteractionTrait[] traits;
    [Header("Sequence")]
    public InteractionDefinition interactionLogic; // Các ActionBase cũ của bạn
}
}