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
        public static void FixFullLayoutChildren(this HorizontalOrVerticalLayoutGroup layout)
        {
            layout.childControlHeight = true;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;
            layout.childScaleHeight = true;
            layout.childScaleWidth = true;
            layout.childAlignment = UnityEngine.TextAnchor.MiddleCenter;
        }
    }
}