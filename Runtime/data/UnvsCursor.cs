namespace unvs.data
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.U2D;
    using unvs.components;
    using unvs.game2d.objects;
    using unvs.game2d.objects.editor;

    [CreateAssetMenu(fileName = "Cursor", menuName = "Unvs/Data/Cursor")]
    public partial class UnvsCursor : UnvsScriptObject
    {
       
        public types.IconInfo icon;
        public SpriteAtlas Icons;
        private void OnEnable()
        {
            if (Icons == null) return;


            Sprite[] allSprites = new Sprite[Icons.spriteCount];


            Icons.GetSprites(allSprites);
            icon.sprites = allSprites;
        }
        [UnvsButton]
        public Sprite[] GetAllSprites()
        {
            Sprite[] allSprites = new Sprite[Icons.spriteCount];
            Icons.GetSprites(allSprites);
            return allSprites;
        }
    }
    public partial class UnvsCursor : UnvsScriptObject
    {

        [UnvsButton]
        public void LoadAllSprites()
        {

        }
    }
}