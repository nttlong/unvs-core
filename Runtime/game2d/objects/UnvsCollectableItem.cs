using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using unvs.components;
using unvs.ext;

namespace unvs.game2d.objects {
    public class UnvsCollectableItem : UnvsPickableObject
    {
        public UnvsBagger owner;

        public Sprite GetInventoryIcon()
        {
            if(this.ViewIcon!=null) return this.ViewIcon;
            var sp = this.GetFirstComponent<SpriteRenderer>();
            if(sp!=null) return sp.sprite;
            return null;
        }
    }
}