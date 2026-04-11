
//using Cysharp.Threading.Tasks.Triggers;
//using System.Collections;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Rendering.Universal;
//using unvs.interfaces;
//using unvs.ext;
//using unvs.shares;
//namespace unvs.gameword
//{
//    [ExecuteInEditMode]
//    public class WorldGlobalLight : MonoBehaviour, IWorldGlobalLight
//    {
//        public GlobalLightObject globalLight;

//        public IGlobalLightWapper GlobalLight
//        {
//            get
//            {
//                if (globalLight != null)
//                {
//                    globalLight.owner = this.GetComponent<IScenePrefab>();
//                    globalLight.goOwner = this.gameObject;
//                    return globalLight;
//                }
//                globalLight = this.GetComponentInChildrenByName<GlobalLightObject>(Constants.ObjectsConst.SCENE_GLOBAL_LIGHT);
//                //globalLight = this.GetComponentsInChildren<GlobalLightObject>().FirstOrDefault(

//                //    p => p.lightSource.lightType == Light2D.LightType.Global
//                //    );
//                if (globalLight == null)
//                {
//                    var go = new GameObject(Constants.ObjectsConst.SCENE_GLOBAL_LIGHT);
//                    go.transform.SetParent(this.transform, false);
//                    globalLight = go.AddComponent<GlobalLightObject>();
//                    go.gameObject.SetActive(false);

//                }
//                globalLight.owner = this.GetComponent<IScenePrefab>();
//                globalLight.goOwner = this.gameObject;
//                return globalLight;
//            }
//            set
//            {
//                if (value == null) return;
//                value.Light.transform.SetParent(transform, true);
//                globalLight = value as GlobalLightObject;
//            }
//        }

//        public void Off()
//        {
//            if (globalLight.lightSource != null)
//            {
//                globalLight.lightSource.gameObject.SetActive(false);
//                //globalLight.lightSource.lightType = Light2D.LightType.Global;
//            }
//        }

//        public void On()
//        {
            
//            if(globalLight.lightSource!=null)
//            {
//                globalLight.lightSource.gameObject.SetActive(true);
//                globalLight.lightSource.lightType = Light2D.LightType.Global;
//            }
            


//        }

//        private void Awake()
//        {
//            _ = this.GlobalLight;
//        }

//        // Use this for initialization



//    }
//}