namespace unvs.data
{
 
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.U2D;

    

    [CreateAssetMenu(fileName = "Cursor", menuName = "Unvs/Data/Cursor")]
    public partial class UnvsCursor : UnvsScriptObject
    {
       
        public types.IconInfo icon;
      
        private void OnEnable()
        {
            if (SpriteAtlasIcons == null) return;


            Sprite[] allSprites = new Sprite[SpriteAtlasIcons.spriteCount];


            SpriteAtlasIcons.GetSprites(allSprites);
            icon.sprites = allSprites;
        }
        
    }
#if UNITY_EDITOR
    public partial class UnvsCursor : UnvsScriptObject
    {
        public SpriteAtlas SpriteAtlasIcons;
        public string[] names;
        public string FolderPath;
        [game2d.objects.editor.UnvsButton]
        public void GetAllSprites()
        {
            if (SpriteAtlasIcons == null || SpriteAtlasIcons.IsDestroyed()) return;
            FolderPath = unvs.editor.utils.UnvsEditorUtils.EditorGetTrueAssetPath(SpriteAtlasIcons);
            unvs.editor.utils.Dialogs.Show($"Icons.spriteCount={SpriteAtlasIcons.spriteCount}");
            Sprite[] allSprites = new Sprite[SpriteAtlasIcons.spriteCount];
            SpriteAtlasIcons.GetSprites(allSprites);
            names = allSprites.Select(p => p.name).ToArray();
            icon.sprites = allSprites.Select(p => unvs.editor.utils.SpriteTools.GetOriginalSprite(p, FolderPath)).ToArray();
        }
    } 
#endif
}