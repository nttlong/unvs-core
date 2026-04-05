using System;
using UnityEngine.UI;

namespace unvs.ext{
    public static class LayoutGroupExt
    {
        public static void FixLayoutChildren(this HorizontalOrVerticalLayoutGroup layout)
        {
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight=false;
            layout.childForceExpandWidth=false;
            layout.childScaleHeight = true;
            layout.childScaleWidth = true;
            layout.childAlignment = UnityEngine.TextAnchor.MiddleCenter;
        }
    }
}