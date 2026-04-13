using System;
using UnityEngine;
using unvs.ext;

namespace unvs.game2d.scenes.actors
{
    public partial class UnvsActorPhysical : UnvsBaseComponent
    {
        public float ArmLen;
        public Transform ArmTop;
        public Transform ArmRoot;
        public Transform handBack;
        public Transform handFront;
        public Transform socketHandBack;
        public Transform socketHandFront;
        public Vector2 GetReachPoint(Vector2 pos)
        {
            var dir=GetComponent<Collider2D>().bounds.center.GetDirectionTo(pos);
            if (dir < 0) return pos + new Vector2(this.ArmLen, 0);
            if(dir > 1) return pos + new Vector2(-this.ArmLen, 0);
            return pos;
        }
    }
#if UNITY_EDITOR
    public partial class UnvsActorPhysical : UnvsBaseComponent
    {
        
        [UnvsButton("Validate")]
        public void EditorVaildate()
        {
            if(ArmTop==null||ArmRoot==null)
            {
                this.RaiseEditorError($"ArmTop and ArmRoot must be set on {name}");
            }
            ArmLen=Vector2.Distance( ArmTop.GetSegment().End, ArmRoot.GetSegment().Start );
            if(this.handBack==null || this.handFront == null)
            {
                this.RaiseEditorError($"handBack and handFront must be set on {name}");
            }
            this.socketHandBack = this.handBack.transform.CreateIfNoExist<Transform>("soket-hand-back");
            this.socketHandBack.localPosition = new Vector3(0.5f, 0.5f, 0);
            this.socketHandFront = this.handFront.transform.CreateIfNoExist<Transform>("soket-hand-front");
            this.socketHandFront.localPosition = new Vector3(0.5f, 0.5f, 0);
        }
    }
    #endif
}