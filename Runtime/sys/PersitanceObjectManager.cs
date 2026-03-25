using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace unvs.sys
{
    public static class PersitanceObjectManager
    {
        public static Dictionary<string, GameObject> map = new Dictionary<string, GameObject>();


        public static void Clear()
        {
            map.Clear();
        }

        public static void Remove(string name)
        {
            map.Remove(name);
        }

        public static bool Validate<T>(T character)
        {

            var obj = character as MonoBehaviour;
            if (obj == null) return true;
            if (obj.GetComponent<T>() == null) return true;
            var key = $"name={obj.name},type={typeof(T).Name}";
            GameObject instance = null;
            if (map.TryGetValue(key, out instance))
            {
                if (instance != obj.gameObject)
                {
                    UnityEngine.Object.Destroy(obj.gameObject);
                    return false;
                }
            }
            map.TryAdd(key, obj.gameObject);
            //map.Add(key, obj.gameObject);
            return true;

        }

    }
}
