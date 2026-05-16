namespace unvs.data
{
    using System;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.U2D;
    using unvs.actions;

    [CreateAssetMenu(fileName = "UnsvInteractableData", menuName = "Unvs/Data/Interactable Data")]
    public partial class UnsvInteractableData : UnvsScriptObject
    {

        [SerializeField]
        public Cursors cursors;
        [SerializeField]
        public InteractionDefinition definition;


    }
}