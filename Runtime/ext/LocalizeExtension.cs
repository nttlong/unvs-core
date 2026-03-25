using System;
using UnityEngine;
using UnityEngine.Localization;
namespace unvs.ext
{
    public static class LocalizeExtension
    {
        const string UNKNOWN_CONTENT = "unkow ???????";
        public static string GetText(this LocalizedString content)
        {
            if (content == null) return UNKNOWN_CONTENT;
            if (content.IsEmpty) return UNKNOWN_CONTENT;
            return content.GetLocalizedString();
        }
    }
}

