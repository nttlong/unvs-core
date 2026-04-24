#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.game2d.objects.editor;

namespace unvs.accessories.components
{
    [Serializable]
    public class accessories_editor : unvs.types.UnvsEditableProperty
    {
        internal MonoBehaviour owner;
        [UnvsButton("ShadowCaster2D")]
        public void EditorShadowCaster2DAll()
        {
            if (!unvs.editor.utils.Dialogs.Confirm($"Do you want to apply all ShadowCaster2D for {owner.name}")) return;
            var compositeShadowCaster = owner.AddComponentIfNotExist<CompositeShadowCaster2D>();

            foreach (var sp in owner.GetComponentsInChildren<SpriteRenderer>(true))
            {
                var shadowGroup = sp.AddComponentIfNotExist<ShadowCaster2D>();
                shadowGroup.selfShadows = true;
                shadowGroup.castingOption = ShadowCaster2D.ShadowCastingOptions.CastAndSelfShadow;

            }
        }
        [UnvsButton]
        public void Generate()
        {



            var body = owner.AddComponentIfNotExist<Rigidbody2D>();
            body.freezeRotation = true;
            var camWatcher = owner.AddChildComponentIfNotExist<Transform>("cam-wacther");
            var coll = owner.GetComponent<Collider2D>();
            camWatcher.position = new Vector3(coll.bounds.center.x, coll.bounds.max.y, -10);
        }
    }
} 
#endif