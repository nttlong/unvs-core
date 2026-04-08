using Codice.Client.Common;
using game2d.scenes;
using UnityEngine;
namespace unvs.game2d.scenes
{
    public abstract class AppUIElemen<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static  T Instance;
        public abstract void InitRunTime();
        
        public virtual void Awake()
        {
            if (Application.isPlaying)
            {

                Instance = this as T;
                InitRunTime();
            }
        }
    }
}