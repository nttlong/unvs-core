using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;
using unvs.ext;
using unvs.game2d.objects;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;

namespace unvs.game2d.actors
{
    [Serializable]
    public partial class actor_physical2d<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        public float ArmLen;
        public Collider2D[] HitBoxesCollider;
        public GameObject currentHoldingItem;
        public SpriteRenderer ArmSprite;
        [SerializeField]
        public Transform HandBack;
        [SerializeField] public Transform HandFront;
        public Transform SocketBack;
        public Transform SocketFront;
        private SpriteSkin _handBackSpriteSkin;
        private SpriteRenderer _handBackSpriteRenderer;
        public Transform rootBone;
        public void HoldItemInBackHand(UnvsPickableObject st)
        {
            if (_handBackSpriteSkin == null)
            {
                _handBackSpriteSkin = this.Owner.GetComponentsInChildren<SpriteSkin>(true).FirstOrDefault(p => p.boneTransforms.Contains(this.HandBack));
                _handBackSpriteRenderer= _handBackSpriteSkin.GetFirstComponent<SpriteRenderer>();
                SocketBack.AttachItemToSocket(st.transform);
                var spr = st.transform.GetFirstComponent<SpriteRenderer>();
                if (spr != null && _handBackSpriteRenderer!=null)
                {
                    st.IncrementalSortingOrder(
                        _handBackSpriteRenderer.sortingOrder,
                        _handBackSpriteRenderer.sortingLayerName);
                }

            }
        }
    }
#if UNITY_EDITOR
    public partial class actor_physical2d<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        private SpriteSkin[] spriteSkins;
        [SerializeField]
        public Transform[] ColliderPart;
       

        [UnvsButton("Create Hit Boxes")]
        public  void EditorCreateHitBoxes()
        {
            this.spriteSkins = Owner.GetComponentsInChildren<SpriteSkin>();
            rootBone = spriteSkins.SelectMany(p => p.boneTransforms).GetRoot();

            var cc = Owner.AddComponentIfNotExist<CompositeCollider2D>();
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
                var propertyName = Owner.GetType().GetFields().FirstOrDefault(p => p.FieldType == this.GetType());
                unvs.editor.utils.Dialogs.Show($"Please, set ArmSprite for {Owner.name}.{propertyName.Name}");
                return;
            }
            ArmLen = this.ArmSprite.size.x;
        }
        [UnvsButton("Create Sokets")]
        public  void EditorCreateSokets()
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