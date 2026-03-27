using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
namespace unvs.ext
{
    public static class LocalizeExtension
    {
        const string UNKNOWN_CONTENT = "unkow ???????";
        public static string GetText(this LocalizedString content)
        {
            if (!content.IsValid())
                return UNKNOWN_CONTENT;
            if (content == null) return UNKNOWN_CONTENT;
            if (content.IsEmpty) return UNKNOWN_CONTENT;
            return content.GetLocalizedString();
        }
        public static string GetTextSafe(this LocalizedString content)
        {
            if (!content.IsValid())
                return UNKNOWN_CONTENT;

            // nếu key/table sai thì GetLocalizedString vẫn có thể trả rỗng
            string text = content.GetLocalizedString();

            if (string.IsNullOrEmpty(text))
                return UNKNOWN_CONTENT;

            return text;
        }
        public static bool IsValid(this LocalizedString content)
        {
            if (content == null) return false;
            if (content.IsEmpty) return false;

            // TableReference phải có
            if (content.TableReference.ReferenceType == TableReference.Type.Empty)
                return false;

            // EntryReference phải có
            if (content.TableEntryReference.ReferenceType == TableEntryReference.Type.Empty)
                return false;

            return true;
        }

        public static LocalizedString GetFirstValid(this LocalizedString msg,params LocalizedString[] p)
        {
            if(msg.IsValid()) return msg;
            var ret= p.FirstOrDefault(p=>p.IsValid());
            if(ret == null) return msg;
            else return ret;
        }
    }
}

