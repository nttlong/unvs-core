using System.Collections;
using UnityEngine;

namespace unvs.shares
{
    public class GlobalLocalizeApp : MonoBehaviour
    {

        public static GlobalLocalizeApp Instance;
        void Awake()
        {
            if(Instance!=null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
       

        
    }
}