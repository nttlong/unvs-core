using System.Collections;
using UnityEngine;
using UnityEngine.Localization;

namespace Script.unvs.ext
{
    public static  class LocalizedStringExtension
    {
        public static LocalizedString ReadLocalizeStreing(this UnityEngine.Object obj, string tableName, string key)
        {
            return new LocalizedString(tableName, key);
        }
    }
}