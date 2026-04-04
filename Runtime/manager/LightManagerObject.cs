using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.gameword;
using unvs.interfaces;
using unvs.shares;
namespace unvs.manager
{
    public class LightManagerObject : MonoBehaviour
    {
        public static LightManagerObject instance;
        Dictionary<IScenePrefab, Light2D> check = new Dictionary<IScenePrefab, Light2D>();
        public Light2D globalLight;
        private bool isDirty;
        private float timerClean = 0f;
        private float timerLight = 0f;
        public float intervalCleanDestroyed = 5f;
        public float interval = 0.3f;
        public Light2D GlobalLight
        {
            get
            {

                if (globalLight != null) return globalLight;
                globalLight = this.GetComponentInChildrenByName<Light2D>(Constants.ObjectsConst.GLOBAL_LIGHT);

                if (globalLight != null) return globalLight;
                var go = new GameObject(Constants.ObjectsConst.GLOBAL_LIGHT);
                globalLight = go.GetOrAddComponent<Light2D>();
                globalLight.transform.SetParent(transform);
                globalLight.lightType = Light2D.LightType.Global;
                return globalLight;
            }
        }

        internal static void Add(params IScenePrefab[] scene)
        {
            instance.DoAdd(scene);
        }

        public void DoAdd(IScenePrefab[] scenes)
        {
            GlobalLight.gameObject.SetActive(false);
            //CleanAllDestroyed();
            foreach (var scene in scenes)
            {
                if (scene == null || scene.IsDestroying) continue;
                var wt = (scene as MonoBehaviour).GetComponentInChildrenByName<IWorldTracker>(Constants.ObjectsConst.SCENE_TRACKER);
                if (wt != null)
                {
                    scene.Globalight.GlobalLight.Light.transform.position = wt.Coll.bounds.center;

                }
                else
                {
                    scene.Globalight.GlobalLight.Light.transform.position = scene.WorldBound.Coll.bounds.center;
                }
                
                scene.Globalight.GlobalLight.Light.transform.SetParent(GlobalLight.transform, true);
                //check[scene] = scene.Globalight.GlobalLight.Light;
            }

            
            RefreshLight();
            GlobalLight.gameObject.SetActive(true);
            isDirty = true;

        }

        private void CleanAllDestroyed()
        {
            // 1. Lọc và lưu vào một danh sách tạm thời (Tách biệt việc truy vấn và việc xóa)
            var targets = GlobalLight.GetComponentsInChildren<IGlobalLightWapper>()
                .Where(p => p.GoOwner != null && p.GoOwner.IsDestroyed())
                .Select(p => p as MonoBehaviour) // Ép kiểu sẵn
                .Where(m => m != null)
                .ToList();

            // 2. Duyệt qua danh sách tạm để xóa
            foreach (var mono in targets)
            {
                mono.transform.SetParent(null);
                Destroy(mono.gameObject);
            }

            // 3. Đợi 1 frame để Unity dọn dẹp xong (Nếu cần)

        }
        private async UniTask CleanAllDestroyedAsync()
        {
            // 1. Lọc và lưu vào một danh sách tạm thời (Tách biệt việc truy vấn và việc xóa)
            var targets = GlobalLight.GetComponentsInChildren<IGlobalLightWapper>(true)
                .Where(p => (p.GoOwner != null && p.GoOwner.IsDestroyed())||p.GoOwner==null)
                .Select(p => p as MonoBehaviour) // Ép kiểu sẵn
                .Where(m => m != null)
                .ToList();

            // 2. Duyệt qua danh sách tạm để xóa
            foreach (var mono in targets)
            {
                mono.transform.SetParent(null);
                Destroy(mono.gameObject);
            }

            // 3. Đợi 1 frame để Unity dọn dẹp xong (Nếu cần)
            await UniTask.Yield();
        }
        public void DoRemove(IScenePrefab scene)
        {
            List<IScenePrefab> keysToRemove = new List<IScenePrefab>();

            foreach (var p in check)
            {
                if (p.Key.IsDestroying)
                {
                    // Hủy Object trong Scene
                    if (p.Value != null || p.Value == GetComponent<Light>())
                    {
                        Destroy(p.Value.gameObject);
                    }
                    keysToRemove.Add(p.Key);
                }
            }

            // 2. Xóa sạch các bản ghi trong Dictionary sau khi duyệt xong
            foreach (var key in keysToRemove)
            {
                check.Remove(key);
            }
        }
        public static void Clean()
        {
            instance.DoClean();
        }

        public void DoClean()
        {
            
            this.enabled = false;
            foreach (var p in check)
            {
                if (p.Key.IsDestroying)
                {
                    // Hủy Object trong Scene
                    Destroy(p.Value.gameObject);
                }
            }
            check.Clear();
            foreach (var p in GlobalLight.GetComponentsInChildren<Light2D>(true))
            {
                Destroy(p.gameObject);
            }
            isDirty = false;
            this.enabled = true;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            GlobalApplication.LightManagerObjectInstance = this;
            instance = this;
            _ = this.GlobalLight;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            timerClean += dt;
            timerLight += dt;

            // 1. Dọn dẹp sau mỗi 5 giây
            if (timerClean >= intervalCleanDestroyed)
            {
                timerClean = 0f;
                CleanAllDestroyedAsync().Forget();
            }

            // 2. Cập nhật ánh sáng sau mỗi 1.5 giây
            if (timerLight >= interval || interval == 0)
            {
                timerLight = 0f;
                bool flowControl = RefreshLight();
                if (!flowControl)
                {
                    return;
                }
            }
        }

        private bool RefreshLight()
        {
            // Check null an toàn cho các singleton/global reference
            if (GlobalApplication.CamTracking == null || SettingsSingleScene.Instance == null) return false;

            // Vì bạn đảm bảo < 3 con, dùng thẳng GetComponentsInChildren là chấp nhận được
            var wrappers = this.GlobalLight.GetComponentsInChildren<IGlobalLightWapper>(true);

            if (wrappers.Length == 0) return false;

            // Sử dụng List tạm thời để tránh tạo rác (Garbage) từ LINQ ToArray()
            // Hoặc dùng List có sẵn để lọc nhanh
            List<Light2D> activeLights = new List<Light2D>();
            for (int i = 0; i < wrappers.Length; i++)
            {
                if (wrappers[i] != null && wrappers[i].Light != null)
                {
                    activeLights.Add(wrappers[i].Light);
                }
            }

            if (activeLights.Count > 0 && isDirty)
            {
                var data = Light2DExtension.MixGlobalLightSources(
                    GlobalApplication.CamTracking.Collider.bounds.center,
                    activeLights.ToArray() // Truyền mảng vào hàm xử lý
                );

                this.GlobalLight.intensity = data.Intensity;
                this.GlobalLight.color = data.Color;
            }

            return true;
        }

        public static void Remove(IScenePrefab scene)
        {
            instance.DoRemove(scene);
        }

        public void Off()
        {
            this.GlobalLight.enabled = false;
        }
        public void On()
        {
            this.GlobalLight.enabled = true;
        }

        public void SetLight(IGlobalLightWapper lightWrapper)
        {
            CleanAllDestroyed();
            lightWrapper.Light.transform.position = lightWrapper.Owner.WorldBound.Coll.bounds.center;

            foreach (var item in GlobalLight.GetComponentsInChildren<IGlobalLightWapper>(true))
            {
                (item as MonoBehaviour).transform.SetParent(null);
                Destroy((item as MonoBehaviour).gameObject);
            }
            GlobalLight.CopyFrom(lightWrapper.Light);
            GlobalLight.lightType = Light2D.LightType.Global;
            (lightWrapper as MonoBehaviour).gameObject.SetActive(false);
            (lightWrapper as MonoBehaviour).transform.SetParent(GlobalLight.transform, true);
            isDirty = true;
        }

        public IGlobalLightWapper FindLightByScene(IScenePrefab scene)
        {
            return GlobalLight.GetComponentsInChildren<IGlobalLightWapper>(true).FirstOrDefault(p => p.Owner == scene);
        }
    }
}