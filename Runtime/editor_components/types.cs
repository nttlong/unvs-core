#if UNITY_EDITOR
using System;

namespace unvs.editor.components
{
    [Serializable]
    public class EditorVector2
    {
        public int x;
        public int y;
    }
    [Serializable]
    public struct SceneInfoResut
    {
        public string FolderPath;
        public string AssetPath;
        public string Name;
    }
} 
#endif