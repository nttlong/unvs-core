using System;
using System.Collections.Generic;
using UnityEngine;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.objects.editor;
using unvs.shares;

namespace unvs.actor_physical
{
    [Serializable]
    public partial class actor_physical : unvs.types.UnvsEditableProperty
    {
        public float ArmLen;
        public Transform rootBone;
        public MonoBehaviour owner;
        [SerializeField]
        public Transform[] ColliderPart;
       
    }
#if UNITY_EDITOR
    public partial class actor_physical : unvs.types.UnvsEditableProperty
    {
        [UnvsButton("Create Hit box collider")]
        public void EditorCreateHitBoxex()
        {
            OnCreateHitBoxes();
        }
        [UnvsButton("Create sokets")]
        public void CreateSokets()
        {
            OnCreateSokets();
        }

        public virtual void OnCreateSokets()
        {
            
        }

        public virtual void OnCreateHitBoxes()
        {
           
        }
    }
#endif

}