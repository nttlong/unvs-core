#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.components;
using unvs.ext;
using unvs.game2d.objects.editor;

namespace unvs.accessories.components
{
    //public partial class ik_manager_controllers<T>: unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    [Serializable]
    public class accessories_editor<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        public Action OnGenerate;

        [UnvsButton("ShadowCaster2D")]
        public void EditorShadowCaster2DAll()
        {
            if (!unvs.editor.utils.Dialogs.Confirm($"Do you want to apply all ShadowCaster2D for {Owner.name}")) return;
            var compositeShadowCaster = Owner.AddComponentIfNotExist<CompositeShadowCaster2D>();

            foreach (var sp in Owner.GetComponentsInChildren<SpriteRenderer>(true))
            {
                var shadowGroup = sp.AddComponentIfNotExist<ShadowCaster2D>();
                shadowGroup.selfShadows = true;
                shadowGroup.castingOption = ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow;
                shadowGroup.enabled=true;

            }
        }
        [UnvsButton]
        public void Generate()
        {

            OnGenerate?.Invoke();

            var body = Owner.AddComponentIfNotExist<Rigidbody2D>();
            body.freezeRotation = true;
            var camWatcher = Owner.AddChildComponentIfNotExist<Transform>("cam-wacther");
            var coll = Owner.GetComponent<Collider2D>();
            //camWatcher.position = new Vector3(coll.bounds.center.x, coll.bounds.max.y, -10);
            //if (Owner.scanerBound == null)
            //{
            //    this.scanerBound = this.AddChildComponentIfNotExist<BoxCollider2D>("scaner-bound");
            //    this.scanerBound.size = this.GetComponent<Collider2D>().bounds.size;

            //}
        }
    }
} 
#endif