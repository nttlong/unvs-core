#if UNITY_EDITOR
namespace unvs.editor.components
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;
    using unvs.components;
    using unvs.game2d.objects.editor;
    using unvs.types;

    public class UnvsEditorShaderApply : UnvsBaseComponent
    {
        [SerializeField]
        public Material DefaultMat;
        [SerializeField]
        public SpriteRenderer defaultSr;
        [SerializeField]
        public SpriteMaterial[] Backups;
        [SerializeField]
        public Material material;
        [UnvsButton()]
        public void Backup()
        {
            var lst = new List<SpriteMaterial>();
            var sp = this.GetComponent<SpriteRenderer>();
            if (sp != null)
            {
                lst.Add(new SpriteMaterial
                {
                    spriteRenderer = sp,
                    material = sp.material,
                });
            }
            var ls = this.GetComponentsInChildren<SpriteRenderer>(true).Select(p => new SpriteMaterial
            {
                spriteRenderer = p,
                material = p.material,
            }).ToArray();
            lst.AddRange(ls);
            Backups = ls.ToArray();
        }
        [UnvsButton()]
        public void Restore()
        {
            foreach (var s in Backups)
            {
                s.spriteRenderer.material = DefaultMat;
            }
        }
        [UnvsButton("Apply Material")]
        public void ApplyMaterial()
        {
            foreach (var item in Backups)
            {
                item.spriteRenderer.material = material;
            }
        }
        [UnvsButton("Shadows Two Sided")]
        public void EditorApplyCastShadowsTwoSided()
        {
            unvs.editor.utils.SpriteTools.ApplyCastShadows2Side(this.gameObject);
            
        }
    }
}
#endif