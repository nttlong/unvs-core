using Codice.Client.Common;
using game2d.scenes;
using UnityEngine;
namespace unvs.game2d.scenes
{
    public abstract class AppUIElemen<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static  T Instance;
        
        public abstract void InitRunTime();
        public abstract void InitEvents();
        public virtual void Show()
        {
            this.enabled = true;
            this.gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            this.enabled = false;
            this.gameObject.SetActive(false);
        }
        public virtual void Awake()
        {
            if (Application.isPlaying)
            {

                Instance = this as T;
                InitRunTime();
                InitEvents();
            }
        }
    }
}