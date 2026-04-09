using PlasticPipe;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

namespace unvs.ext
{
    public static class MonoBehaviourExtension
    {
        public static Canvas AddChildChildCanvasWithGraphicRaycasterIfNotExist(this MonoBehaviour obj, string name)
        {
            var go = obj.gameObject.transform.Find(name)?.gameObject;
            if (go == null)
            {
                go = new GameObject(name, typeof(RectTransform));
                go.transform.SetParent(obj.transform, false);
            }

            var ret = go.GetOrAddComponent<Canvas>();

            // MẸO QUAN TRỌNG: Thiết lập Render Mode ngay lập tức
            // Nếu để mặc định (Screen Space Overlay), Unity dễ bị nhầm lẫn trong Prefab Mode.
            // Chuyển sang World Space hoặc giữ nguyên nhưng phải gọi lệnh cập nhật.
            ret.renderMode = RenderMode.ScreenSpaceOverlay;

            go.GetOrAddComponent<CanvasScaler>();
            go.GetOrAddComponent<GraphicRaycaster>();

            // BÁO CHO UNITY: Đây là một sự thay đổi cấu trúc UI chính thức
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(obj.gameObject);
                // Nếu đang trong Prefab Mode, dùng lệnh này để lưu "vết"
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(stage.scene);
                }
            }

            return ret;
        }
        public static T AddChildComponentIfNotExist<T>(this MonoBehaviour obj, string name) where T : Component
        {
            var ret= obj.GetComponentInChildrenByName<T>(name);
            if (ret != null) return ret;
            var go=new GameObject(name);
            go.transform.SetParent(obj.transform);
            if (typeof(T) == typeof(Canvas))
            {
                go.SetMeOnLayer(unvs.shares.Constants.Layers.UI);
            }
            if (typeof(T)== typeof(Transform)){
                return go.transform as T;
            } else
            {
                return go.AddComponent<T>();
            }
           
        }
        public static T AddComponentIfNotExist<T>(this Component obj, Action<T> OnInit = null) where T : Component
        {
            // 1. Kiểm tra xem Component đã tồn tại trên GameObject chưa
            T component = obj.GetComponent<T>();

            // 2. Nếu chưa có thì mới Add
            if (component == null)
            {
                component = obj.gameObject.AddComponent<T>();
                OnInit?.Invoke(component);
            }
            if (typeof(T) == typeof(Canvas))
            {
                obj.gameObject.SetMeOnLayer(unvs.shares.Constants.Layers.UI);
            }
            return component;
        }
        public static bool CheckComponentIfNotExistCreate<T>(this MonoBehaviour obj, out T component) where T : Component
        {
            // 1. Kiểm tra xem Component đã tồn tại trên GameObject chưa
            component = obj.GetComponent<T>();
            var ret=false;
            // 2. Nếu chưa có thì mới Add
            if (component == null || component == default(T))
            {
                component = obj.AddComponent<T>();
                ret = true;
            }
            if (typeof(T) == typeof(Canvas))
            {
                obj.SetMeOnLayer(unvs.shares.Constants.Layers.UI);
            }
            return ret;
        }
        public static T AddComponentIfNotExist<T>(this MonoBehaviour obj, Action<T> OnInit = null) where T : Component
        {
            // 1. Kiểm tra xem Component đã tồn tại trên GameObject chưa
            T component = obj.GetComponent<T>();

            // 2. Nếu chưa có thì mới Add
            if (component == null || component==default(T))
            {
                component = obj.AddComponent<T>();
                OnInit?.Invoke(component);
            }
            if (typeof(T) == typeof(Canvas))
            {
                obj.SetMeOnLayer(unvs.shares.Constants.Layers.UI);
            }
            return component;
        }
        public static SpriteSkin GetSpriteSkin(this GameObject obj)
        {
            var anim = obj.GetComponentInParent<Animator>(true);
            if (anim == null) return null;
            var parentBone = obj.transform.parent;
            // get all skining sprites
            var spriteList = AnimExtension.ExtractAllSpriteSkins(anim);
            foreach (var sp in spriteList)
            {
                var check = sp.boneTransforms.FirstOrDefault(p => p == parentBone);
                if (check != null) return sp;
            }
            return null;
        }
        public static void AttachToSocket(this MonoBehaviour Socket, MonoBehaviour source)
        {
            //var physicalObject = GetComponentInParent<IPhysicalObject>();
            //if (physicalObject == null) return;
            ////var rootStg=(physicalObject as MonoBehaviour).GetComponent<SortingGroup>();
            var sg = source.AddComponentIfNotExist<SortingGroup>();
            sg.enabled = false;

            //var rendererBack = physicalObject.SocketBack.Skin.GetComponent<SpriteRenderer>();
            //var rendererFront = physicalObject.SocketFront.Skin.GetComponent<SpriteRenderer>();


            //source.transform.SetParent(this.transform, false);
            source.transform.localPosition = Vector3.zero;
            //source.transform.localRotation = Quaternion.identity;
            //source.transform.localPosition = new Vector3(0, 0, -0.01f);
            // 2. Lấy tất cả Sprite con của cây đèn (Prefab source)
            var lampSprites = source.GetComponentsInChildren<SpriteRenderer>();

            // Lấy thông số từ Renderer của tay
            //string targetLayer = rendererBack.sortingLayerName;
            //int orderBack = rendererBack.sortingOrder;
            //int orderFront = rendererFront.sortingOrder;

            //if (this == (physicalObject.SocketFront as MonoBehaviour))
            //{
            //    // TRƯỜNG HỢP GẮN VÀO TAY TRƯỚC: Đèn nằm giữa Back và Front
            //    foreach (var sr in lampSprites)
            //    {
            //        sr.sortingLayerName = targetLayer;
            //        // Kẹp giữa: Lớn hơn tay sau 1 đơn vị
            //        sr.sortingOrder = orderBack;
            //    }

            //    // Đảm bảo tay trước phải nhảy lên trên đèn (ít nhất là +2 so với tay sau)
            //    if (orderFront <= orderBack + 1)
            //    {
            //        rendererFront.sortingOrder = orderBack;
            //    }
            //}
            //else
            //{
            //    // TRƯỜNG HỢP GẮN VÀO TAY SAU: Đèn nằm sau cả tay sau
            //    foreach (var sr in lampSprites)
            //    {
            //        sr.sortingLayerName = targetLayer;
            //        sr.sortingOrder = orderBack;
            //    }
            //}

            //// 3. Logic Layer vật lý
            //source.SetMeOnLayer(Constants.Layers.HOLD_ITEM);
            //physicalObject.CurrentObject = source;
        }
    }



}