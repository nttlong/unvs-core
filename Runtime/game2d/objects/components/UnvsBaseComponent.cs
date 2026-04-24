using System;
using UnityEngine;

namespace unvs.game2d.objects.components
{
    
        public abstract class UnvsBaseComponent : MonoBehaviour
    {
        

#if UNITY_EDITOR
        public Action<string> OnEditorError;
        public void RaiseEditorError(string error)
        {
            OnEditorError?.Invoke(error);
        }
#endif


    }
}