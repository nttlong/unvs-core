using game2d.scenes;
using System;
using System.Globalization;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using unvs.ext;
using unvs.game2d.scenes;

namespace game2d.ext
{
    public class UnvsCamStaus : MonoBehaviour
    {
        internal Vector2 offSetValue;
        internal bool isInteruptValue;
        internal bool isInProgress;
    }
    public static class AppCinemaExt
    {
        public static void EditorSave(this UnityEngine.Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
        public static void EditorSetDirty(this UnityEngine.Object obj)
        {
            EditorUtility.SetDirty(obj);
            
        }
        
    }
}