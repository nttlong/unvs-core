using UnityEngine;
using System;
using unvs.actions;
namespace unvs.unvsobjects{
[CreateAssetMenu(fileName = "Object-Definition", menuName = "Systems/Object Definition")]
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