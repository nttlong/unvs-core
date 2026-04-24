using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.objects.editor;

namespace unvs.game2d.actors
{
    [Serializable]
    public partial class actor_physical2d : unvs.actor_physical.actor_physical
    {
        public Collider2D[] HitBoxesCollider;
        public GameObject  currentHoldingItem;
        public SpriteRenderer ArmSprite;
        [SerializeField]
        public Transform HandBack;
        [SerializeField] public Transform HandFront;
        public Transform SocketBack;
        public Transform SocketFront;
        public void HoldItemInBackHand(UnvsPickableObject st)
        {
            throw new NotImplementedException();
        }
    }
#if UNITY_EDITOR
    public partial class actor_physical2d : unvs.actor_physical.actor_physical
    {
        private SpriteSkin[] spriteSkins;
        public override void OnCreateHitBoxes()
        {
            this.spriteSkins = owner.GetComponentsInChildren<SpriteSkin>();
            base.rootBone = spriteSkins.SelectMany(p => p.boneTransforms).GetRoot();

            var cc = owner.AddComponentIfNotExist<CompositeCollider2D>();
            cc.geometryType = CompositeCollider2D.GeometryType.Polygons;
            var lst = new List<Collider2D>();
            foreach (var footer in ColliderPart)
            {

                var c = footer.GetComponent<PolygonCollider2D>();
                if (c == null)
                {
                    c = footer.AddComponentIfNotExist<PolygonCollider2D>();

                    c.SetPath(0, footer.Collider2dGeneratePoints());
                    c.compositeOperation = Collider2D.CompositeOperation.Merge;
                }
                lst.Add(c);
            }

            HitBoxesCollider = lst.ToArray();
        }
        
        [UnvsButton("Calculate arm len")]
        public void EditpCalculateArmLen()
        {
            if (this.ArmSprite == null)
            {
                var propertyName = owner.GetType().GetFields().FirstOrDefault(p => p.FieldType == this.GetType());
                unvs.editor.utils.Dialogs.Show($"Please, set ArmSprite for {owner.name}.{propertyName.Name}");
                return;
            }
            ArmLen = this.ArmSprite.size.x;
        }
        public override void OnCreateSokets()
        {
            //if (this.HandBack == null || this.HandFront) {
            //    var propertyName = owner.GetType().GetFields().FirstOrDefault(p => p.FieldType == this.GetType());
            //    unvs.editor.utils.Dialogs.Show($"Please, set HandBack and HandFront for {owner.name}.{propertyName.Name}");
            //    return;
            //}
            this.SocketBack = this.HandBack.AddChildComponentIfNotExist<Transform>("socket-back");
            this.SocketFront = this.HandFront.AddChildComponentIfNotExist<Transform>("socket-front");
        }



    }
#endif
}